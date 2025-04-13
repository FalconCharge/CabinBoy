using UnityEngine;

public class ShipBouyancy : MonoBehaviour
{

    [SerializeField] private float buoyancyForce = 5f;

    [SerializeField] private Transform[] bouyancyPoints;
    [SerializeField] private float waveHeightOffset = 0f;

    [SerializeField] private float maxUpwardVelocity = 2f; 
    [SerializeField] private float maxDownwardVelocity = 1f;

    [SerializeField] private float maxTiltAngle = 15f; // Degrees
    [SerializeField] private float rotationDamping = 5f;

    private Rigidbody rb;
    private MeshDisplacer waterDisplacer;

    [SerializeField] private Vector3 graivty = new Vector3(0, -5f, 0f);


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {   
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        waterDisplacer = FindFirstObjectByType<MeshDisplacer>();

    }

    void FixedUpdate()
    {
        if(waterDisplacer == null) return;

        foreach(Transform point in bouyancyPoints) {
            float waveHeight = GetWaveHeight(point.position);
            float adjustedWaterLevel = waveHeight - waveHeightOffset;

            if(point.position.y < adjustedWaterLevel){

                float submersion = adjustedWaterLevel - point.position.y;
                Vector3 force = Vector3.up * buoyancyForce * submersion;
                ApplyLimitedForce(point.position, force);
            }
        }
        
        // Graity
        rb.AddForce(graivty, ForceMode.Force);
        LimitVerticalVelocity();
        LimitRotation();
    }

    private void ApplyLimitedForce(Vector3 position, Vector3 force)
    {
        // Get current velocity at this point
        Vector3 pointVelocity = rb.GetPointVelocity(position);
        
        // Only apply force if under velocity limits
        if(pointVelocity.y < maxUpwardVelocity)
        {
            rb.AddForceAtPosition(force, position, ForceMode.Force);
        }
    }

    private void LimitVerticalVelocity()
    {
        Vector3 velocity = rb.linearVelocity;
        
        // Clamp vertical velocity
        velocity.y = Mathf.Clamp(
            velocity.y,
            -maxDownwardVelocity,
            maxUpwardVelocity
        );
        
        rb.linearVelocity = velocity;
    }

    private void LimitRotation()
    {
        // Get current rotation angles
        Vector3 euler = rb.rotation.eulerAngles;
        
        // Normalize angles to -180/180 range
        euler.x = (euler.x > 180) ? euler.x - 360 : euler.x;
        euler.z = (euler.z > 180) ? euler.z - 360 : euler.z;
        
        // Apply limits (pitch and roll only)
        euler.x = Mathf.Clamp(euler.x, -maxTiltAngle, maxTiltAngle);
        euler.z = Mathf.Clamp(euler.z, -maxTiltAngle, maxTiltAngle);
        
        // Smoothly reorient
        Quaternion targetRot = Quaternion.Euler(euler.x, rb.rotation.eulerAngles.y, euler.z);
        rb.MoveRotation(Quaternion.Slerp(
            rb.rotation, 
            targetRot, 
            rotationDamping * Time.fixedDeltaTime
        ));
    }

    private float GetWaveHeight(Vector3 position)
    {
        return waterDisplacer.GetWaveHeight(position);
    }
}
