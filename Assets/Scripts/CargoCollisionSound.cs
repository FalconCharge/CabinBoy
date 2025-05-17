using UnityEngine;

public class CargoCollisionSound : MonoBehaviour
{
    [SerializeField] float soundStrength = 1f; 
    [SerializeField] float minImpulse = 1f;     // Minimum force to trigger sound
    [SerializeField] float maxImpulse = 10f;    // Max force that maps to full volume

    private void OnCollisionEnter(Collision collision)
    {
        // Impact strength
        float impulse = collision.relativeVelocity.magnitude;

        // weak collisions
        if (impulse < minImpulse) return;

        // Normalize strength
        float normalizedVolume = Mathf.InverseLerp(minImpulse, maxImpulse, impulse);

        float finalVolume = normalizedVolume * soundStrength;

        AudioManager.Instance.PlayRandomCargoHit(finalVolume);
    }
}
