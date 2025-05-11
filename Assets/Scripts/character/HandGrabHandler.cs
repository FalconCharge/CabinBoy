using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class HandGrabHandler : MonoBehaviour
{
    [Header("References")]
    public PlayerManager player;
    public Animator      anim;
    public bool          leftArm = false;

    [Header("Joint Settings")]
    [SerializeField] private float rotationalSpring    = 1000f;
    [SerializeField] private float rotationalDamper    = 50f;
    [SerializeField] private float limitAngle          = 15f;
    [SerializeField] private float connectedMassScale  = 0.01f;

    [Header("Grabbed Object Damping")]
    [SerializeField] private float grabbedDrag         = 5f;
    [SerializeField] private float grabbedAngularDrag  = 5f;

    private ConfigurableJoint       cfgJoint;
    private Rigidbody               rb;
    private Rigidbody               heldRb;
    private Cargo                   grabbedCargo;

    // static maps so all arms share the same info
    private static Dictionary<Rigidbody,int>   grabCounts     = new Dictionary<Rigidbody,int>();
    private static Dictionary<Rigidbody,float> origDrags      = new Dictionary<Rigidbody,float>();
    private static Dictionary<Rigidbody,float> origAngDrags   = new Dictionary<Rigidbody,float>();

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

    private void TryGrab(Collision collision)
    {
        // already holding from this hand?
        if (cfgJoint != null) return;
        if (collision.transform.root == player.transform) return;
        if (!collision.collider.TryGetComponent<Rigidbody>(out var otherRb)) return;
        

        // increment grab count
        grabCounts.TryGetValue(otherRb, out int count);
        grabCounts[otherRb] = count + 1;

        // if this is the first hand on that object, store & apply drag/color
        if (count == 0)
        {
            // store original drags
            origDrags[otherRb]      = otherRb.linearDamping;
            origAngDrags[otherRb]   = otherRb.angularDamping;

            // bump drags
            otherRb.linearDamping            = grabbedDrag;
            otherRb.angularDamping     = grabbedAngularDrag;

            // color
            if (collision.collider.TryGetComponent<Cargo>(out var cargo))
            {
                cargo.ApplyPickUpDetail();
            }
            grabbedCargo = collision.collider.GetComponent<Cargo>();

        }

        // now create your joint (same as before)
        cfgJoint = gameObject.AddComponent<ConfigurableJoint>();
        cfgJoint.connectedBody             = otherRb;
        cfgJoint.autoConfigureConnectedAnchor = false;
        cfgJoint.anchor                   = Vector3.zero;
        cfgJoint.connectedAnchor          = transform.InverseTransformPoint(collision.GetContact(0).point);
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

    private void TryLetGo()
    {
        if (cfgJoint == null) return;

        bool release = leftArm ? !player.inputManager.left_Input
                               : !player.inputManager.right_Input;
        if (!release) return;

        var otherRb = cfgJoint.connectedBody;
        Destroy(cfgJoint);
        cfgJoint = null;
        
        if (grabbedCargo != null)
        {
            Debug.Log("AA");
            grabbedCargo.ResetColor();
            grabbedCargo = null;
        }
        return;
        
        // decrement grab count
        if (grabCounts.TryGetValue(otherRb, out int count))
        {
            int newCount = Mathf.Max(0, count - 1);
            grabCounts[otherRb] = newCount;

            // only restore drags & color on final release
            if (newCount == 0)
            {
                
                if (origDrags.TryGetValue(otherRb, out float od))
                    otherRb.linearDamping = od;
                if (origAngDrags.TryGetValue(otherRb, out float oa))
                    otherRb.angularDamping = oa;

                if (origDrags.ContainsKey(otherRb))  origDrags.Remove(otherRb);
                if (origAngDrags.ContainsKey(otherRb)) origAngDrags.Remove(otherRb);
                grabCounts.Remove(otherRb);

                if (grabbedCargo != null)
                {
                    Debug.Log("AA");
                    grabbedCargo.ResetColor();
                    grabbedCargo = null;
                }
            }
        }
    }

    // private void helperReset(Rigidbody otherRb, )
}
