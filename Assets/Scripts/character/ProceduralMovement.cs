using UnityEditor.Search;
using UnityEngine;

public class ProceduralMovement : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Animator component controlling your character")] 
    [SerializeField] private Animator animator;
    [Tooltip("InputManager for reading stick/buttons")] 
    private InputManager input;
    [Tooltip("PlayerLocomotion for grounded state and max speed")] 
    private PlayerLocomotion locomotion;
    [Tooltip("Bone transform used to tilt torso when grabbing")] 
    [SerializeField] private Transform torsoBone;

    [Header("Torso Tilt Settings")]
    [Tooltip("Max degrees torso tilts left/right when grabbing")] 
    [SerializeField] private float maxTorsoTilt = 20f;
    [Tooltip("Speed at which torso tilts toward target angle")] 
    [SerializeField] private float torsoTiltSpeed = 5f;

    private SyncPhysicsObject[] _syncBones;


    private Rigidbody _rb;

    // cached default rotation of the torso bone
    private Quaternion _torsoDefaultRot;

    private static readonly int _hashMoveSpeed   = Animator.StringToHash("movementSpeed");
    private static readonly int _hashGrounded    = Animator.StringToHash("grounded");
    private static readonly int _hashIsCarrying  = Animator.StringToHash("isCarrying");
    private static readonly int _hashArmHeight   = Animator.StringToHash("armHeight");

    private void Awake()
    {
        _syncBones = GetComponentsInChildren<SyncPhysicsObject>();

        if (input    == null) input    = GetComponent<InputManager>();
        if (locomotion == null) locomotion = GetComponent<PlayerLocomotion>();
        if (torsoBone != null)
            _torsoDefaultRot = torsoBone.localRotation;
            
        _rb = GetComponent<Rigidbody>();
    }

    void LateUpdate()
    {
        foreach (var x in _syncBones)
        {
            x.UpdateJointFromAnimation();
        }
    }

    public void HandleAnims()
    {
        // 1) Send movement speed (0-1 analog) to animator
        float speedNorm = _rb.linearVelocity.magnitude;
        animator.SetFloat("movementSpeed", speedNorm);

        // 2) Grounded flag from locomotion
        bool grounded = locomotion.isGrounded;
        animator.SetBool("grounded", grounded);

        // 3) Grabbing state if either grab input is held
        bool carrying = input.left_Input || input.right_Input;
        animator.SetBool("isCarrying", carrying);

        // 4) Arm height from vertical look input
        float armHeight = input.lookInput.y;
        animator.SetFloat("armHeight", armHeight);

        // 5) Tilt torso when grabbing
        if (torsoBone != null)
            UpdateTorsoTilt(carrying);
    }

    /// <summary>
    /// Smoothly tilts the torso bone around its local Z axis based on input horizontal
    /// only when carrying; otherwise returns to default rotation.
    /// </summary>
    private void UpdateTorsoTilt(bool carrying)
    {
        // target local rotation
        Quaternion target = _torsoDefaultRot;
        if (carrying)
        {
            float tiltAngle = input.horizontalInput * maxTorsoTilt;
            // negative tilt so positive input tilts right shoulder down
            target = _torsoDefaultRot * Quaternion.Euler(0f, 0f, -tiltAngle);
        }

        // lerp current toward target
        torsoBone.localRotation = Quaternion.Slerp(
            torsoBone.localRotation,
            target,
            torsoTiltSpeed * Time.deltaTime
        );
    }
}
