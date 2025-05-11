using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HandGrabHandler : MonoBehaviour
{
    [Header("References")]
    [Tooltip("Player's root manager")] public PlayerManager player;
    [Tooltip("Animator for grabbing feedback")] public Animator anim;
    [Tooltip("Is this the left arm? (else right)")] public bool leftArm = false;

    [Header("Joint Settings")]
    [Tooltip("Spring strength for rotation drive")] [SerializeField] private float rotationalSpring = 1000f;
    [Tooltip("Damping for rotation drive")] [SerializeField] private float rotationalDamper = 50f;
    [Tooltip("Limit angle (degrees) on the configurable joint)")] [SerializeField] private float limitAngle = 15f;
    [Tooltip("Mass scale to make object lightweight on hand")] [SerializeField] private float connectedMassScale = 0.01f;

    private ConfigurableJoint cfgJoint;
    private Rigidbody rb;
    private Cargo grabbedCargo;
    

    private void Awake()
    {
        player = transform.root.GetComponent<PlayerManager>();
        rb = GetComponent<Rigidbody>();
        rb.solverIterations = 50;
    }

    private void Update()
    {
        TryLetGo();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (player.inputManager.left_Input || player.inputManager.right_Input)
            TryGrab(collision);
    }

    private void TryLetGo()
    {
        bool isHolding = cfgJoint != null;
        bool release = leftArm ? !player.inputManager.left_Input : !player.inputManager.right_Input;
        if (isHolding && release)
        {
            // Reset color
            if (grabbedCargo != null)
                grabbedCargo.ResetColor();

            // Apply small throw impulse
            Rigidbody otherRb = cfgJoint.connectedBody;
            otherRb.AddForce((player.transform.forward + Vector3.up * 0.25f) * 0.1f, ForceMode.Impulse);

            Destroy(cfgJoint);
            cfgJoint = null;
            grabbedCargo = null;

            // Animator grab-off
            // anim.SetBool(leftArm ? "LeftGrab" : "RightGrab", false);
        }
    }

    private void TryGrab(Collision collision)
    {
        if (cfgJoint != null) return; // already holding
        if (collision.transform.root == player.transform) return;
        if (!collision.collider.TryGetComponent<Rigidbody>(out var otherRb)) return;

        // Record cargo
        if (collision.collider.TryGetComponent<Cargo>(out var cargo))
        {
            if(!cargo.colored)
                cargo.ApplyPickupColor();
                
            Debug.Log("colorapply");
            grabbedCargo = cargo;
            
            // AudioManager.Instance.PlayGrab();
        }

        // Create ConfigurableJoint
        cfgJoint = gameObject.AddComponent<ConfigurableJoint>();
        cfgJoint.connectedBody = otherRb;
        cfgJoint.autoConfigureConnectedAnchor = false;
        // anchor at hand's local origin, connected at contact point
        cfgJoint.anchor = Vector3.zero;
        cfgJoint.connectedAnchor = transform.InverseTransformPoint(collision.GetContact(0).point);

        // Lock linear motion
        cfgJoint.xMotion = ConfigurableJointMotion.Locked;
        cfgJoint.yMotion = ConfigurableJointMotion.Locked;
        cfgJoint.zMotion = ConfigurableJointMotion.Locked;

        // Limit angular motion with small cone
        SoftJointLimit limit = new SoftJointLimit { limit = limitAngle };
        cfgJoint.angularXMotion = ConfigurableJointMotion.Limited;
        cfgJoint.angularYMotion = ConfigurableJointMotion.Limited;
        cfgJoint.angularZMotion = ConfigurableJointMotion.Limited;
        cfgJoint.lowAngularXLimit = limit;
        cfgJoint.highAngularXLimit = limit;
        cfgJoint.angularYLimit = limit;
        cfgJoint.angularZLimit = limit;

        // Setup Slerp drive for smooth rotation
        JointDrive slerpDrive = new JointDrive
        {
            positionSpring = rotationalSpring,
            positionDamper = rotationalDamper,
            maximumForce   = Mathf.Infinity
        };
        cfgJoint.slerpDrive = slerpDrive;
        cfgJoint.rotationDriveMode = RotationDriveMode.Slerp;

        // Reduce mass effect
        cfgJoint.connectedMassScale = connectedMassScale;
        cfgJoint.massScale = connectedMassScale;

        // Animator grab-on
        // anim.SetBool(leftArm ? "LeftGrab" : "RightGrab", true);
    }
}
