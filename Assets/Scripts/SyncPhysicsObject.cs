using UnityEngine;

public class SyncPhysicsObject : MonoBehaviour
{
    Rigidbody _rb;
    ConfigurableJoint joint;

    [SerializeField]
    Rigidbody animatedRigidBody;

    [SerializeField]
    bool syncAnimation = false;

    Quaternion startLocalRotation;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        joint = GetComponent<ConfigurableJoint>();

        startLocalRotation = transform.localRotation;
    }
    
    public void UpdateJointFromAnimation()
    {
        if (!syncAnimation)
            return;

        ConfigurableJointExtensions.SetTargetRotationLocal(joint, animatedRigidBody.transform.localRotation, startLocalRotation);


    }

}
