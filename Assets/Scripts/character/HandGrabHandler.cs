using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HandGrabHandler : MonoBehaviour
{
    [Header("References")]
    public PlayerManager player;
    public Animator      anim;
    public bool          leftArm = false;

    [Header("Joint Settings")]
    [SerializeField] private float rotationalSpring = 1000f;
    [SerializeField] private float rotationalDamper = 50f;
    [SerializeField] private float limitAngle       = 15f;
    [SerializeField] private float connectedMassScale = 0.01f;

    [Header("Grabbed Object Damping")]
    
    [SerializeField] private float grabbedDrag         = 5f;
    
    [SerializeField] private float grabbedAngularDrag  = 5f;

    private ConfigurableJoint cfgJoint;
    private Rigidbody         rb;
    private Rigidbody         heldRb;
    private Cargo             grabbedCargo;
    private float             origDrag, origAngularDrag;

    void Awake()
    {
        player = transform.root.GetComponent<PlayerManager>();
        rb     = GetComponent<Rigidbody>();
        rb.solverIterations = 50;
    }

    void Update()
    {
        TryLetGo();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (player.inputManager.left_Input || player.inputManager.right_Input)
            TryGrab(collision);
    }

    private void TryLetGo()
    {
        if (cfgJoint == null) return;
        bool release = leftArm ? !player.inputManager.left_Input
                               : !player.inputManager.right_Input;
        if (!release) return;

        // restore drag
        if (heldRb != null)
        {
            heldRb.linearDamping        = origDrag;
            heldRb.angularDamping = origAngularDrag;
        }

        heldRb.GetComponent<Cargo>().ResetColor();
        

        // destroy joint
        Destroy(cfgJoint);
        cfgJoint   = null;
        heldRb     = null;
    }

    private void TryGrab(Collision collision)
    {
        if (cfgJoint != null) return;
        if (collision.transform.root == player.transform) return;
        if (!collision.collider.TryGetComponent<Rigidbody>(out var otherRb)) return;


        if (collision.collider.TryGetComponent<Cargo>(out var cargo))
        {
            cargo.ApplyPickUpDetail(grabbedDrag ,grabbedAngularDrag);
            if(AudioManager.Instance != null)
            {
                AudioManager.Instance.PlayGrab();
            }
        }


        // store & bump drag
        heldRb          = otherRb;
        origDrag        = otherRb.linearDamping;
        origAngularDrag = otherRb.angularDamping;
        otherRb.linearDamping  = grabbedDrag;
        otherRb.angularDamping = grabbedAngularDrag;

        // create joint
        cfgJoint = gameObject.AddComponent<ConfigurableJoint>();
        cfgJoint.connectedBody = otherRb;
        cfgJoint.autoConfigureConnectedAnchor = false;
        cfgJoint.anchor                = Vector3.zero;
        cfgJoint.connectedAnchor       = transform.InverseTransformPoint(collision.GetContact(0).point);
        cfgJoint.xMotion = cfgJoint.yMotion = cfgJoint.zMotion = ConfigurableJointMotion.Locked;

        // angular limits
        SoftJointLimit limit = new SoftJointLimit { limit = limitAngle };
        cfgJoint.angularXMotion = ConfigurableJointMotion.Limited;
        cfgJoint.angularYMotion = ConfigurableJointMotion.Limited;
        cfgJoint.angularZMotion = ConfigurableJointMotion.Limited;
        cfgJoint.lowAngularXLimit  = limit;
        cfgJoint.highAngularXLimit = limit;
        cfgJoint.angularYLimit     = limit;
        cfgJoint.angularZLimit     = limit;

        // slerp drive
        JointDrive drive = new JointDrive
        {
            positionSpring = rotationalSpring,
            positionDamper = rotationalDamper,
            maximumForce   = Mathf.Infinity
        };
        cfgJoint.slerpDrive        = drive;
        cfgJoint.rotationDriveMode = RotationDriveMode.Slerp;

        // mass scale
        cfgJoint.connectedMassScale = connectedMassScale;
        cfgJoint.massScale          = connectedMassScale;
    }
}