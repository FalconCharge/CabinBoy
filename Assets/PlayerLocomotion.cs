using Unity.VisualScripting;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    [SerializeField] private Vector3 inputDir;
    PlayerManager playerMan;
    InputManager inputMan;
    Rigidbody _rb;
    public Transform camObj;

    [SerializeField]
    ConfigurableJoint mainJoint;

    static int playerLayer;// = LayerMask.NameToLayer("Player");
    static int ignorePlayerLM;// = 1 << playerLayer;     // bit mask for just the Player layer
    static int everythingButPlayerMask;// = ~ignorePlayerLM;

    [Header("Stable Movement")]
    public float CurrentMoveSpeed = 0f;
    public float MoveSpeed = 10f;
    public float maxSpeed = 50f;
    public float turnSpeed = 10f;

    [Header("Air Movement")]
    public bool isGrounded = false;
    public float jumpForce = 10f;
    //public float AirAccelerationSpeed = 5f;
    //public float Drag = 0.1f;

    SyncPhysicsObject[] syncPhysicsObjects;

    //Raycasts
    RaycastHit[] raycastHits = new RaycastHit[10];

    [Header("Misc")]
    public bool RotationObstruction;
    public bool isSprinting;

    public Vector3 moveInputVector;


    private void Awake()
    {
        playerMan = GetComponent<PlayerManager>();
        inputMan = GetComponent<InputManager>();
        _rb = GetComponent<Rigidbody>();

        syncPhysicsObjects = GetComponentsInChildren<SyncPhysicsObject>();


        playerLayer = LayerMask.NameToLayer("player");
        ignorePlayerLM = 1 << playerLayer;     // bit mask for just the Player layer
        everythingButPlayerMask = ~ignorePlayerLM;
    }

    public void HandleAllMovement()
    {
        if (!playerMan.isInteracting)
        {
            HandleInputToCam();
            AddVelocityToPlayer();
        }
    }
    public void HandleJumping()
    {
        //velocity up
        if(isGrounded)
        {
            _rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
    private void HandleInputToCam()
    {
        //Debug.Log("AAA");
        Vector3 camForward = camObj.forward;
        camForward.y = 0f;
        camForward.Normalize();

        inputDir = camForward * inputMan.verticalInput;
        inputDir += camObj.right * inputMan.horizontalInput;
        inputDir.Normalize();
        inputDir.y = 0;
        moveInputVector = inputDir;
    }
    void FixedUpdate()
    {
        CurrentMoveSpeed = _rb.linearVelocity.magnitude;
        isGrounded = false;

        int numberOfHits = Physics.SphereCastNonAlloc(
            _rb.position, 0.1f, transform.up * -1,
            raycastHits, 0.5f,
            everythingButPlayerMask,
            QueryTriggerInteraction.Ignore
            );

        for (int i = 0; i < numberOfHits; i++)
        {
            if (raycastHits[i].transform.root == transform)
                continue;

            isGrounded = true;
            break;
        }
        
        if (!isGrounded)
            _rb.AddForce(Vector3.down * 10);

        if (moveInputVector.sqrMagnitude > 0.001f)
        {
            // 1) Rotate your joint toward the real X,Z vector:
            Quaternion desiredDir = Quaternion.LookRotation(
              new Vector3(moveInputVector.x * -1, 0, moveInputVector.z),
              Vector3.up
            );

            //mainJoint.targetRotation = Quaternion.RotateTowards(
            //  mainJoint.targetRotation,
            //  desiredDir,
            //  turnSpeed * Time.fixedDeltaTime
            //);
            mainJoint.targetRotation = desiredDir;

            // 2) Measure speed along your input direction:
            Vector3 norm = moveInputVector.normalized;
            float speedAlongInput = Vector3.Dot(_rb.linearVelocity, norm);

            // 3) If you�re under top-speed, push along that same vector:
            if (speedAlongInput < maxSpeed)
                _rb.AddForce(norm * MoveSpeed, ForceMode.Acceleration);
        }

        for (int i = 0; i < syncPhysicsObjects.Length; i++)
        {
            syncPhysicsObjects[i].UpdateJointFromAnimation();
        }

    }

    private void AddVelocityToPlayer()
    {
        //given ridibody, add velocity from input to it.
    }

}
