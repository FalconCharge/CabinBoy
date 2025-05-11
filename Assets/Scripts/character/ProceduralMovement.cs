using UnityEngine;

public class ProceduralMovement : MonoBehaviour
{

    
    [Header("Grab Box Settings")]
    [Tooltip("Local-space center of the grab box")]
    [SerializeField] private Vector3     grabBoxCenter = new Vector3(0, 1f, 1f);
    [Tooltip("Size of the grab box (width, height, depth)")]
    [SerializeField] private Vector3     grabBoxSize   = new Vector3(1f, 1f, 2f);
    [Tooltip("LayerMask of objects you can grab")]
    [SerializeField] private LayerMask   grabbableLayer;
    [Tooltip("World-space transforms of your hand bones")]
    [SerializeField] private Transform   leftHandTransform;
    [SerializeField] private Transform   rightHandTransform;
    [Tooltip("Speed at which the hand moves to the grab point")]

    [SerializeField] private Rigidbody leftHandRb;
    [SerializeField] private Rigidbody rightHandRb;

    private Vector3 leftClosestPoint, rightClosestPoint;
    public Rigidbody leftClosestObject, rightClosestObject;
    private const float grabAttachThreshold = 0.1f;


    [SerializeField] private float       pullSpeed     = 5f;
    
    private Rigidbody                        closestObject;
    private Vector3                          closestPoint;
    
    [Header("Limb Pull Settings")]
    [SerializeField] private float     limbPullForce   = 35f;
    [SerializeField] private float     maxPullDistance = 3f;
    [SerializeField] private float     velocityDamper  = 0.85f;
    [SerializeField] private float     angularDragBoost= 2f;
#region  other
    [Header("References")]
    [SerializeField] private Animator animator;
    private InputManager inputManager;
    private PlayerLocomotion locomotion;
    [SerializeField] private Transform torsoBone;

    [Header("Torso Tilt Settings")]
    [SerializeField] private float maxTorsoTilt = 20f;
    [SerializeField] private float torsoTiltSpeed = 5f;

    [Header("Arm Grab Settings")]
    [SerializeField] private float armLayerBlendSpeed = 5f;
    private int leftArmLayer, rightArmLayer;

    [Header("Arm Height Settings")]
    [SerializeField] private float armHeightSpeed = 1f;
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
        syncBones = GetComponentsInChildren<SyncPhysicsObject>();

        if (animator     == null) animator     = GetComponent<Animator>();
        if (inputManager == null) inputManager = GetComponent<InputManager>();
        if (locomotion   == null) locomotion   = GetComponent<PlayerLocomotion>();

        if (torsoBone != null)
            torsoDefaultRotation = torsoBone.localRotation;

        rb = GetComponent<Rigidbody>();

        leftArmLayer  = animator.GetLayerIndex("LeftArm");
        rightArmLayer = animator.GetLayerIndex("RightArm");

        armHeight = (armHeightLimits.x + armHeightLimits.y) * 0.5f;
    }
    private void LateUpdate()
    {
        foreach (var bone in syncBones)
            bone.UpdateJointFromAnimation();
    }
    #endregion
    public void HandleAnims()
    {
        ProcessGrabDetection();
        
        float speedNorm = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z).magnitude;
        animator.SetFloat(hashMoveSpeed, speedNorm);
        animator.SetBool(hashGrounded, locomotion.isGrounded);
        bool leftGrab   = inputManager.left_Input;
        bool rightGrab  = inputManager.right_Input;
        bool isCarrying = leftGrab || rightGrab;
        animator.SetBool(hashIsCarrying, isCarrying);
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

        
        HandleLimbPulling();
    }

    private void ProcessGrabDetection()
    {


        // compute the world‐space box
        Vector3 worldCenter = transform.TransformPoint(grabBoxCenter);
        Vector3 halfExtents = grabBoxSize * 0.5f;
        Quaternion worldRot = transform.rotation;

        // find all grabbable colliders
        Collider[] hits = Physics.OverlapBox(worldCenter, halfExtents, worldRot, grabbableLayer);

        // reset
        leftClosestObject  = null;
        rightClosestObject = null;
        leftClosestPoint   = Vector3.zero;
        rightClosestPoint  = Vector3.zero;

        // find closest for LEFT hand
        float bestDist = float.MaxValue;
        foreach (var col in hits)
        {
            var body = col.attachedRigidbody;
            if (body == null) continue;
            Vector3 pt = col.ClosestPoint(worldCenter);
            float dist = Vector3.Distance(leftHandTransform.position, pt);
            if (dist < bestDist)
            {
                bestDist           = dist;
                leftClosestObject  = body;
                leftClosestPoint   = pt;
            }
        }

        // find closest for RIGHT hand
        bestDist = float.MaxValue;
        foreach (var col in hits)
        {
            var body = col.attachedRigidbody;
            if (body == null) continue;
            Vector3 pt = col.ClosestPoint(worldCenter);
            float dist = Vector3.Distance(rightHandTransform.position, pt);
            if (dist < bestDist)
            {
                bestDist            = dist;
                rightClosestObject  = body;
                rightClosestPoint   = pt;
            }
        }

        // debug lines
        if (leftClosestObject != null)
            Debug.DrawLine(leftHandTransform.position, leftClosestPoint, Color.green);
        if (rightClosestObject != null)
            Debug.DrawLine(rightHandTransform.position, rightClosestPoint, Color.green);

        #if UNITY_EDITOR
        Debug.DrawLine(worldCenter + worldRot * new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z),
                       worldCenter + worldRot * new Vector3( halfExtents.x, -halfExtents.y, -halfExtents.z), Color.yellow);
        Debug.DrawLine(worldCenter + worldRot * new Vector3( halfExtents.x, -halfExtents.y, -halfExtents.z),
                       worldCenter + worldRot * new Vector3( halfExtents.x, -halfExtents.y,  halfExtents.z), Color.yellow);
        Debug.DrawLine(worldCenter + worldRot * new Vector3( halfExtents.x, -halfExtents.y,  halfExtents.z),
                       worldCenter + worldRot * new Vector3(-halfExtents.x, -halfExtents.y,  halfExtents.z), Color.yellow);
        Debug.DrawLine(worldCenter + worldRot * new Vector3(-halfExtents.x, -halfExtents.y,  halfExtents.z),
                       worldCenter + worldRot * new Vector3(-halfExtents.x, -halfExtents.y, -halfExtents.z), Color.yellow);

        // Repeat for top face
        Debug.DrawLine(worldCenter + worldRot * new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z),
                       worldCenter + worldRot * new Vector3( halfExtents.x, halfExtents.y, -halfExtents.z), Color.yellow);
        Debug.DrawLine(worldCenter + worldRot * new Vector3( halfExtents.x, halfExtents.y, -halfExtents.z),
                       worldCenter + worldRot * new Vector3( halfExtents.x, halfExtents.y,  halfExtents.z), Color.yellow);
        Debug.DrawLine(worldCenter + worldRot * new Vector3( halfExtents.x, halfExtents.y,  halfExtents.z),
                       worldCenter + worldRot * new Vector3(-halfExtents.x, halfExtents.y,  halfExtents.z), Color.yellow);
        Debug.DrawLine(worldCenter + worldRot * new Vector3(-halfExtents.x, halfExtents.y,  halfExtents.z),
                       worldCenter + worldRot * new Vector3(-halfExtents.x, halfExtents.y, -halfExtents.z), Color.yellow);

        // Connect vertical edges
        for (int i = 0; i < 4; i++)
        {
            Vector3 cornerA = new Vector3(
                ((i & 1) == 0 ? -halfExtents.x : halfExtents.x),
                -halfExtents.y,
                ((i & 2) == 0 ? -halfExtents.z : halfExtents.z)
            );
            Vector3 cornerB = new Vector3(cornerA.x, halfExtents.y, cornerA.z);
            Debug.DrawLine(worldCenter + worldRot * cornerA,
                           worldCenter + worldRot * cornerB, Color.yellow);
        }
        #endif

    }

    private void HandleLimbPulling()
    {
        if (inputManager.left_Input && leftClosestObject != null)
        {
            Vector3 dir = (closestPoint - leftHandRb.position).normalized;
            leftHandRb.AddForce(dir * pullSpeed, ForceMode.VelocityChange);
        }
        if (inputManager.right_Input && rightClosestObject != null)
        {
            Vector3 dir = (closestPoint - rightHandRb.position).normalized;
            rightHandRb.AddForce(dir * pullSpeed, ForceMode.VelocityChange);
        }

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
