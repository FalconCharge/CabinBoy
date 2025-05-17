using UnityEngine;


public class PlayerManager : MonoBehaviour
{
    public enum PlayerState
    {
        Idle,
        Move,
        Jog,
        Crawl,
    }

    [SerializeField] private Animator ani;
    [HideInInspector] public InputManager inputManager;
    [HideInInspector] PlayerLocomotion playerLocomotion;
    [HideInInspector] ProceduralMovement proceduralMovement;
    public string currentStatee;
    public bool isInteracting = false;
    [SerializeField] private bool inputs;
    [SerializeField] private bool movement;
    [SerializeField] private bool visuals;

    
    [SerializeField] private float layerBlendSpeed = 5f;
    [SerializeField] private float armHeightSpeed = 1f;
    private int leftArmLayer, rightArmLayer;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        proceduralMovement = GetComponent<ProceduralMovement>();
        
        leftArmLayer  = ani.GetLayerIndex("LeftArm");
        rightArmLayer = ani.GetLayerIndex("RightArm");
    }

    private void Update()
    {
        if (isInteracting)
        {
            ani.SetLayerWeight(leftArmLayer, 1);
            ani.SetLayerWeight(rightArmLayer, 1);

            ani.SetFloat("armHeight", 1f);
            inputs = false;
            movement = false;
            // visuals = false;

        }
        else if (inputs)
        {
            inputManager.HandleAllInputs();

            float targetL = inputManager.left_Input ? 1f : 0f;
            float targetR = inputManager.right_Input ? 1f : 0f;

            float wL = ani.GetLayerWeight(leftArmLayer);
            float wR = ani.GetLayerWeight(rightArmLayer);

            ani.SetLayerWeight(leftArmLayer,
                Mathf.MoveTowards(wL, targetL, Time.deltaTime * layerBlendSpeed));
            ani.SetLayerWeight(rightArmLayer,
                Mathf.MoveTowards(wR, targetR, Time.deltaTime * layerBlendSpeed));

            UpdateArmHeight();
        }
    }
    
    private void UpdateArmHeight()
    {
        float current = ani.GetFloat("armHeight");
        float deltaY = inputManager.lookInput.y;
        
        current += deltaY * armHeightSpeed * Time.deltaTime;
        current = Mathf.Clamp01(current);
        
        ani.SetFloat("armHeight", current);
    }

    private void FixedUpdate()
    {
        if (movement)
        {
            Debug.Log("Movement");
            playerLocomotion.HandleAllMovement();
        }

        if (visuals)
        {
            proceduralMovement.HandleAnims();
            //proceduralAnimate.HandleAllVisuals();
        }
    }
    private void LateUpdate()
    {
        // isInteracting = ani.GetBool("isInteracting");
        // playerLocomotion.isJumping = ani.GetBool("isJumping");
    }
}
