using UnityEditor.Callbacks;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 5f;
    [SerializeField] private float gravity = -9.81f;



    private InputSystem_Actions playerControls;

    private CharacterController controller;

    private InputAction move;

    private Vector2 moveInput;
    private Vector3 velocity;

    private bool isGrounded;
    [SerializeField] private float groundDistance = 0.4f;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private Transform groundCheck;

    private Rigidbody rb;





    void Awake()
    {
        playerControls = new InputSystem_Actions();  
        rb = GetComponent<Rigidbody>(); 
    }
    void OnEnable()
    {
        move = playerControls.Player.Move;
        move.Enable();
    }
    void OnDisable()
    {
        move.Disable();
    }

    void Start()
    {
        controller = GetComponent<CharacterController>();
        

    }

    void FixedUpdate()
    {
        // Check if grounded
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Read movement input
        moveInput = move.ReadValue<Vector2>();
        Vector3 moveDirection = transform.right * moveInput.x + transform.forward * moveInput.y;

        // Apply movement
        if (isGrounded)
        {
            rb.AddForce(moveDirection * movementSpeed, ForceMode.Impulse);
        }
        else
        {
            rb.AddForce(moveDirection * 0.5f * movementSpeed, ForceMode.Force); // Reduced control in air
        }



        // Cap horizontal speed to prevent excessive acceleration
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (flatVel.magnitude > movementSpeed)
        {
            flatVel = flatVel.normalized * movementSpeed;
            rb.linearVelocity = new Vector3(flatVel.x, rb.linearVelocity.y, flatVel.z);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }





}
