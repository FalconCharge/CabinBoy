using UnityEngine;

[RequireComponent(typeof(Animator))]
public class ProceduralMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Animator animator;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private PlayerLocomotion locomotion;
    [Tooltip("Bone transform used to tilt torso when grabbing")] 
    [SerializeField] private Transform torsoBone;

    [Header("Torso Tilt Settings")]
    [Tooltip("Max degrees torso tilts left/right when grabbing")] 
    [SerializeField] private float maxTorsoTilt = 20f;
    [Tooltip("Speed at which torso tilts toward target angle")] 
    [SerializeField] private float torsoTiltSpeed = 5f;

    [Header("Arm Grab Settings")]
    [Tooltip("How fast the arm layers blend on grab/ungrab")] 
    [SerializeField] private float armLayerBlendSpeed = 5f;
    private int leftArmLayer, rightArmLayer;

    [Header("Arm Height Settings")]
    [Tooltip("Speed at which arm height moves based on vertical look input")] 
    [SerializeField] private float armHeightSpeed = 1f;
    [Tooltip("Minimum and maximum arm height in animator (0-1)")]
    [SerializeField] private Vector2 armHeightLimits = new Vector2(0f, 1f);
    private float armHeight;

    private SyncPhysicsObject[] syncBones;
    private Quaternion torsoDefaultRotation;
    private Rigidbody rb;

    private static readonly int hashMoveSpeed  = Animator.StringToHash("movementSpeed");
    private static readonly int hashGrounded   = Animator.StringToHash("grounded");
    private static readonly int hashIsCarrying = Animator.StringToHash("isCarrying");
    private static readonly int hashArmHeight  = Animator.StringToHash("armHeight");

    private void Awake()
    {
        // Cache sync-bone components
        syncBones = GetComponentsInChildren<SyncPhysicsObject>();

        // Auto-assign references if missing
        if (animator     == null) animator     = GetComponent<Animator>();
        if (inputManager == null) inputManager = GetComponent<InputManager>();
        if (locomotion   == null) locomotion   = GetComponent<PlayerLocomotion>();

        if (torsoBone != null)
            torsoDefaultRotation = torsoBone.localRotation;

        rb = GetComponent<Rigidbody>();

        // Prepare arm layers for grabbing
        leftArmLayer  = animator.GetLayerIndex("LeftArm");
        rightArmLayer = animator.GetLayerIndex("RightArm");

        // Initialize arm height
        armHeight = (armHeightLimits.x + armHeightLimits.y) * 0.5f;
    }

    private void LateUpdate()
    {
        // Sync ragdoll joints after animation updates
        foreach (var bone in syncBones)
            bone.UpdateJointFromAnimation();
    }

    /// <summary>
    /// Call this once per frame to push parameters & procedural tilts.
    /// </summary>
    public void HandleAnims()
    {
        // 1) Movement speed (magnitude of horizontal velocity)
        float speedNorm = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z).magnitude;
        animator.SetFloat(hashMoveSpeed, speedNorm);

        // 2) Grounded flag from locomotion
        animator.SetBool(hashGrounded, locomotion.isGrounded);

        // 3) Grabbing state for animation and layers
        bool leftGrab   = inputManager.left_Input;
        bool rightGrab  = inputManager.right_Input;
        bool isCarrying = leftGrab || rightGrab;
        animator.SetBool(hashIsCarrying, isCarrying);

        // Blend animator layers for left/right grabs
        BlendArmLayer(leftArmLayer,  leftGrab);
        BlendArmLayer(rightArmLayer, rightGrab);

        float lookY = inputManager.lookInput.y;
        armHeight = Mathf.Clamp(
            armHeight + lookY * armHeightSpeed * Time.deltaTime,
            armHeightLimits.x,
            armHeightLimits.y
        );
        animator.SetFloat(hashArmHeight, armHeight);
        

        float tiltInput = isCarrying ? inputManager.horizontalInput : 0f;
        UpdateTorsoTilt(tiltInput);
    }

    private void BlendArmLayer(int layerIndex, bool active)
    {
        float target = active ? 1f : 0f;
        float current = animator.GetLayerWeight(layerIndex);
        animator.SetLayerWeight(
            layerIndex,
            Mathf.MoveTowards(current, target, armLayerBlendSpeed * Time.deltaTime)
        );
    }

    /// <summary>
    /// Smoothly tilts the torso bone around local Z for a grabbing pose.
    /// </summary>
    private void UpdateTorsoTilt(float inputX)
    {
        if (torsoBone == null) return;

        float tilt = -inputX * maxTorsoTilt;
        Quaternion target = torsoDefaultRotation * Quaternion.Euler(0f, 0f, tilt);

        torsoBone.localRotation = Quaternion.Slerp(
            torsoBone.localRotation,
            target,
            torsoTiltSpeed * Time.deltaTime
        );
    }
}
