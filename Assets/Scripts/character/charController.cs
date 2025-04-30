using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class charController : MonoBehaviour
{
    [Header("Tuning")]
    [Tooltip("How fast the center ball pulls the ragdoll around.")]
    public float moveSpeed = 10f;
    [Tooltip("How quickly the ragdoll will face the move direction.")]
    public float turnSpeed = 15f;

    Rigidbody _rb;
    Vector2 _moveInput;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        // Optional: lock unwanted axes so your ragdoll doesn't tip over
        //_rb.constraints = RigidbodyConstraints.FreezeRotationX
                        //| RigidbodyConstraints.FreezeRotationZ;
    }

    // This method name must match the Action name "Move" in the Input Actions asset.
    // UnityEvent hookup is automatic when you select "Invoke Unity Events" in PlayerInput.
    public void OnMove(InputValue value)
    {
        Debug.Log("OnMove: " + value.Get<Vector2>());
        _moveInput = value.Get<Vector2>();
    }

    void FixedUpdate()
    {
        Vector3 raw = new Vector3(_moveInput.x, 0, _moveInput.y);
        if (raw.sqrMagnitude > 0.001f)
        {
            // 1) move
            Vector3 desiredVel = raw.normalized * moveSpeed;
            _rb.AddForce(desiredVel, ForceMode.Acceleration);

            // 2) turn smoothly toward movement direction
            Quaternion targetRot = Quaternion.LookRotation(raw, Vector3.up);
            _rb.MoveRotation(
                Quaternion.Slerp(_rb.rotation, targetRot, turnSpeed * Time.fixedDeltaTime)
            );
        }
    }
}
