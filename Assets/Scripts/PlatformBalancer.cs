using UnityEngine;

public class PlatformBalancer : MonoBehaviour
{
    private Rigidbody rb;

    [SerializeField] private float stablizationTorque = 10f;
    [SerializeField] private float maxCorrectAngle = 10f;
    [SerializeField] private float upForceMultipler = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        float tiltAngle = Vector3.Angle(Vector3.up, transform.up);

        if(tiltAngle < maxCorrectAngle){
            Vector3 correctiveTorque = -rb.angularVelocity * stablizationTorque;
            rb.AddTorque(correctiveTorque, ForceMode.Acceleration);

             Vector3 tiltAxis = Vector3.Cross(transform.up, Vector3.up);
             Vector3 uprightTorque = tiltAxis * tiltAngle * upForceMultipler;

             rb.AddTorque(uprightTorque, ForceMode.Acceleration);
        }
    }
}
