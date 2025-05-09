using Unity.Mathematics;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    #region Variables

    // References
    private InputManager _input;
    private PlayerManager _player;
    private Rigidbody _rb;
    public Transform camObj;

    [Header("Orientation Settings")]
    public float uprightSpringStrength = 150f;
    public float uprightSpringDamper = 20f;
    private Quaternion _uprightTargetRot;


    [Header("Hover Settings")]
    public float RideSpringStrength = 10.0f;
    public float RideSpringDamper = 5.0f;
    public float rideHeight = 2.0f;
    public float rayLen = 2.0f;


    [Header("Jump Settings")]
    public float jumpForce = 10.0f;

    
    [Header("Movement Settings")]
    public float MaxSpeed = 8f;
    public float Acceleration = 20f;
    public AnimationCurve AccelerationFactorFromDot = AnimationCurve.Linear(-1, 2, 1, 1);
    public float MaxAccelForce = 120f;
    public AnimationCurve MaxAccelForceFactorFromDot  = AnimationCurve.Linear(-1, 2, 1, 1);
    public float MaxAccelForceFactor = 1f;
    public Vector3 ForceScale = new Vector3(1f, 0f, 1f);

    private Vector3 m_UnitGoal;
    private Vector3 m_GoalVel;

    public Vector3 downDir = Vector3.down;
    public LayerMask groundMask;

    
    [Header("Debug Settings")]
    public bool showRay = false;
    public Color debugRayColor = Color.red;
    
    void Awake()
    {
        _input = GetComponent<InputManager>();
        _player = GetComponent<PlayerManager>();
        _rb = GetComponent<Rigidbody>();
        _rb.angularDamping = uprightSpringDamper;
        _uprightTargetRot = transform.rotation;
        // m_GoalVel = Vector3.zero;
    }

    #endregion
    public void HandleAllMovement()
    {
        ReadInput();
        HoverRay();
        Locomotion();
        UpdateUprightForce();

    }
    private void ReadInput()
    {
        // Convert 2D movement input into world‐space direction
        Vector3 raw = new Vector3(_input.horizontalInput, 0f, _input.verticalInput);
        Vector3 dir = camObj.TransformDirection(raw);
        dir.y = 0f;

        m_UnitGoal = dir.sqrMagnitude > 1f ? dir.normalized : dir;

    }

    private void Locomotion()
    {
        Vector3 unitVel = m_GoalVel.normalized;

        float velDot = Vector3.Dot(m_UnitGoal, unitVel);
        float accel = Acceleration * AccelerationFactorFromDot.Evaluate(velDot);
        
        Vector3 goalVel = m_UnitGoal * MaxSpeed;// * speedFactor;

        m_GoalVel = Vector3.MoveTowards(m_GoalVel,
                                        (goalVel),// + (groundVel),
                                        accel * Time.fixedDeltaTime);

        Vector3 neededAccel = (m_GoalVel - _rb.linearVelocity) / Time.fixedDeltaTime;

        float maxAccel = MaxAccelForce * MaxAccelForceFactorFromDot.Evaluate(velDot) * MaxAccelForceFactor;

        neededAccel = Vector3.ClampMagnitude(neededAccel, maxAccel);

        
        Vector3 flatVelDir = _rb.linearVelocity;
        flatVelDir.y = 0f;
        if (flatVelDir.sqrMagnitude > 0.0001f)
            _uprightTargetRot = Quaternion.LookRotation(flatVelDir.normalized, Vector3.up);

        _rb.AddForce(Vector3.Scale(neededAccel * _rb.mass, ForceScale));
    }


    private void UpdateUprightForce()
    {
        Quaternion curr = transform.rotation;
        // Quaternion goal = _uprightTargetRot * Quaternion.Inverse(curr); //UtilsMath.ShortestRotation;
        Quaternion goal = ShortestRotation(curr, _uprightTargetRot);
        // Quaternion goal = _uprightTargetRot * Quaternion.Inverse(curr);

        goal.ToAngleAxis(out float rotDegrees, out Vector3 rotAxis);
        rotAxis.Normalize();

        float rotRadians = rotDegrees * Mathf.Deg2Rad;

        _rb.AddTorque(rotAxis * (rotRadians * uprightSpringStrength) - (_rb.angularVelocity * uprightSpringDamper));
    }
    private void HoverRay()
    {

        RaycastHit hit;

        if(showRay)
            Debug.DrawRay(transform.position, downDir * rayLen, debugRayColor);

        if (Physics.Raycast(transform.position, downDir, out hit, rayLen, groundMask))
        {
            Vector3 vel = _rb.linearVelocity;
            Vector3 rayDir = transform.TransformDirection(downDir);

            Vector3 otherVel = Vector3.zero;
            Rigidbody hitBody = hit.rigidbody;
            if(hitBody != null)
            {
                otherVel = hitBody.linearVelocity;
            }

            float rayDirVel = Vector3.Dot(rayDir, vel);
            float otherVelVel = Vector3.Dot(rayDir, otherVel);

            float relVel = rayDirVel - otherVelVel;
            float x = hit.distance - rideHeight;
            float springForce = (x * RideSpringStrength) - (relVel * RideSpringDamper);

            _rb.AddForce(rayDir * springForce);

            if(hitBody != null)
            {
                hitBody.AddForceAtPosition(rayDir * -springForce, hit.point);
            }
        }
    }
    public void HandleJumping()
    {
        _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse); //AnalogJumpUpForce.Evaluate(1f)

        // bool canBuffer = _jumpPressed && (Time.time - _lastJumpPressTime <= jumpBufferTime);
        // bool canCoyote = Time.time - _lastGroundedTime <= coyoteTime;
        // if (canBuffer && canCoyote && isGrounded)
        // {
            // _rb.linearVelocity = new Vector3(
            //     _rb.linearVelocity.x,
            //     JumpUpVel * JumpUpVelFactorFromExistingY.Evaluate(_rb.linearVelocity.y),
            //     _rb.linearVelocity.z
            // );
            // _rb.AddForce(Vector3.up * AnalogJumpUpForce.Evaluate(1f), ForceMode.Impulse);
        //     _jumpPressed = false;
        //     _lastGroundedTime = -999f;
        // }
    }

    private static Quaternion ShortestRotation(Quaternion from, Quaternion to)
    {
        // If the dot product is negative, the quaternions
        // have opposite handedness and the long way around
        // will be chosen.  Flip one to ensure the short way.
        if (Quaternion.Dot(from, to) < 0f)
            to = new Quaternion(-to.x, -to.y, -to.z, -to.w);

        // Delta = target * inverse(current)
        return to * Quaternion.Inverse(from);
    }
}
