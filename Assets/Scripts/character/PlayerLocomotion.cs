using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerLocomotion : MonoBehaviour
{
    // References
    private InputManager _input;
    private PlayerManager _player;
    private Rigidbody _rb;
    public Transform camObj;

    [Header("Floating Settings")]
    public float rideHeight = 1f;
    public float rideCheck = 1.5f;
    public float rideSpringStrength = 300f;
    public float rideSpringDamper = 25f;
    public Vector3 downDir = Vector3.down;
    public LayerMask groundMask;
    public float downForce = 30f;
    public Color debugRayColor = Color.yellow;

    [Header("Orientation Settings")]
    public float uprightSpringStrength = 150f;
    public float uprightSpringDamper = 20f;
    private Quaternion _uprightTargetRot;

    [Header("Movement Settings")]
    public float MaxSpeed = 8f;
    public float Acceleration = 200f;
    public AnimationCurve AccelerationFactorFromDot = AnimationCurve.Linear(-1, 1, 1, 1);
    public float MaxAccelForce = 150f;
    public AnimationCurve MaxAccelForceFactorFromDot = AnimationCurve.Linear(-1, 1, 1, 1);
    public float MaxAccelForceFactor = 1f;
    public Vector3 ForceScale = new Vector3(1f, 0f, 1f);

    private Vector3 m_UnitGoal;
    private Vector3 m_GoalVel;

    [Header("Gravity Scale Drop")]
    public float GravityScaleDrop = 10f;

    [Header("Jump Settings")]
    public float JumpUpVel = 7.5f;
    public bool isGrounded;
    public AnimationCurve JumpUpVelFactorFromExistingY = AnimationCurve.Linear(0, 1, 1, 1);
    public AnimationCurve AnalogJumpUpForce = AnimationCurve.Linear(0, 0, 1, 1);
    public float JumpTerminalVelocity = 22.5f;
    public float JumpDuration = 0.6667f;
    public float coyoteTime = 0.2f;
    public float jumpBufferTime = 0.1f;

    private float _lastGroundedTime;
    private float _lastJumpPressTime;
    private bool _jumpPressed;

    void Awake()
    {
        _input = GetComponent<InputManager>();
        _player = GetComponent<PlayerManager>();
        _rb = GetComponent<Rigidbody>();
        _rb.angularDamping = uprightSpringDamper;
        _uprightTargetRot = transform.rotation;
        m_GoalVel = Vector3.zero;
    }

    /// <summary>
    /// Apply torque to restore upright orientation and face the movement target.
    /// </summary>
    public void UpdateUprightForce(float elapsed)
    {
        Quaternion characterCurrent = transform.rotation;
        Quaternion toGoal = _uprightTargetRot * Quaternion.Inverse(characterCurrent);

        toGoal.ToAngleAxis(out float rotDegrees, out Vector3 rotAxis);
        if (rotDegrees > 180f) rotDegrees -= 360f;
        rotAxis.Normalize();
        float rotRad = rotDegrees * Mathf.Deg2Rad;

        Vector3 torque = rotAxis * (rotRad * uprightSpringStrength)
                         - _rb.angularVelocity * uprightSpringDamper;
        _rb.AddTorque(torque, ForceMode.Force);
    }

    private void Update()
    {
        // Player input
        Vector3 rawInput = new Vector3(_input.horizontalInput, 0f, _input.verticalInput);
        Vector3 move = camObj.TransformDirection(rawInput);
        if (move.magnitude > 1f) move.Normalize();
        m_UnitGoal = move;

        // Jump buffering
        if (_input.jump_Input)
        {
            _lastJumpPressTime = Time.time;
            _input.jump_Input = false;
            _jumpPressed = true;
        }
    }

    private void FixedUpdate()
    {
        if (_player.isInteracting)
            return;

        // --- Rotate to face movement direction ---
        Vector3 flatVelDir = _rb.linearVelocity;
        flatVelDir.y = 0f;
        if (flatVelDir.sqrMagnitude > 0.0001f)
            _uprightTargetRot = Quaternion.LookRotation(flatVelDir.normalized, Vector3.up);

        // --- Floating Capsule (Ground Spring) ---
        RaycastHit hit;
        Vector3 worldDown = downDir;
        Debug.DrawRay(transform.position, worldDown * rideCheck, debugRayColor);

        isGrounded = false;
        if (Physics.Raycast(transform.position, worldDown, out hit, rideCheck, groundMask))
        {
            isGrounded = true;

            // relative spring velocity
            Vector3 vel = _rb.linearVelocity;
            Vector3 otherVel = hit.rigidbody != null ? hit.rigidbody.linearVelocity : Vector3.zero;
            float relVel = Vector3.Dot(worldDown, vel) - Vector3.Dot(worldDown, otherVel);

            // spring displacement
            float x = hit.distance - rideHeight;
            float springForce = (x * rideSpringStrength) - (relVel * rideSpringDamper);

            // apply spring force
            _rb.AddForce(worldDown * springForce, ForceMode.Force);
            if (hit.rigidbody != null)
                hit.rigidbody.AddForceAtPosition(-worldDown * springForce, hit.point, ForceMode.Force);

            _lastGroundedTime = Time.time;
        }
        else
        {
            // fall faster when off the ground
            _rb.AddForce(worldDown * downForce, ForceMode.Acceleration);
        }

        // --- Upright Orientation ---
        UpdateUprightForce(Time.fixedDeltaTime);

        // --- Physics-Driven Running ---
        // 1) Ramp Goal Velocity toward desired input
        float velDot2 = flatVelDir.sqrMagnitude > 0.0001f
            ? Vector3.Dot(m_UnitGoal, flatVelDir.normalized)
            : 1f;
        Vector3 desiredVel = m_UnitGoal * MaxSpeed;
        m_GoalVel = Vector3.MoveTowards(
            m_GoalVel,
            desiredVel,
            Acceleration * AccelerationFactorFromDot.Evaluate(velDot2) * Time.fixedDeltaTime
        );

        // 2) Compute required acceleration and clamp
        Vector3 neededAccel = (m_GoalVel - _rb.linearVelocity) / Time.fixedDeltaTime;
        float maxAccel = MaxAccelForce
            * MaxAccelForceFactorFromDot.Evaluate(velDot2)
            * MaxAccelForceFactor;
        neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccel);

        // 3) Apply mass-scaled force with per-axis scaling
        Vector3 appliedForce = Vector3.Scale(neededAccel * _rb.mass, ForceScale);
        _rb.AddForce(appliedForce, ForceMode.Force);

        // --- Jumping (Coyote Time & Buffer) ---
        bool canBuffer = _jumpPressed && (Time.time - _lastJumpPressTime <= jumpBufferTime);
        bool canCoyote = Time.time - _lastGroundedTime <= coyoteTime;
        if (canBuffer && canCoyote && isGrounded)
        {
            _rb.linearVelocity = new Vector3(
                _rb.linearVelocity.x,
                JumpUpVel * JumpUpVelFactorFromExistingY.Evaluate(_rb.linearVelocity.y),
                _rb.linearVelocity.z
            );
            _rb.AddForce(Vector3.up * AnalogJumpUpForce.Evaluate(1f), ForceMode.Impulse);
            _jumpPressed = false;
            _lastGroundedTime = -999f;
        }

        // --- Gravity Scale Drop for Faster Falls ---
        if (_rb.linearVelocity.y < 0f)
            _rb.AddForce(Vector3.up * Physics.gravity.y * (GravityScaleDrop - 1f), ForceMode.Acceleration);

        // --- Cap Downward Velocity ---
        if (_rb.linearVelocity.y < -JumpTerminalVelocity)
            _rb.linearVelocity = new Vector3(_rb.linearVelocity.x, -JumpTerminalVelocity, _rb.linearVelocity.z);
    }
}
