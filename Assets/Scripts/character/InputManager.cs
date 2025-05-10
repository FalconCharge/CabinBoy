using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.InputSystem;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerManager playerMan;
    PlayerControls playerControls;
    PlayerLocomotion playerLocomotion;
    public Vector2 movementInput;

    public float moveSpeed;
    public float horizontalInput;
    public float verticalInput;

    public Vector2 lookInput;

    public bool sprint_Input;
    public bool left_Input;
    public bool right_Input;
    public bool crouch_Input;
    public bool crouch_Pressed;
    public bool jump_Input = false;
    public float jumpBufferTime = 0.1f;
    public float lastJumpPressTime = -999f;

    private void Awake()
    {
        playerLocomotion = GetComponent<PlayerLocomotion>();
        playerMan = GetComponent<PlayerManager>();
    }

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Move.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerMovement.Move.canceled += i => movementInput = Vector2.zero;

            playerControls.PlayerMovement.Camera.performed += ctx => lookInput = ctx.ReadValue<Vector2>();
            playerControls.PlayerMovement.Camera.canceled  += ctx => lookInput = Vector2.zero;

            playerControls.PlayerActions.jump.performed += i => jump_Input = true;

            playerControls.PlayerActions.sprint.performed += i => sprint_Input = true;
            playerControls.PlayerActions.sprint.canceled += i => sprint_Input = false;

            //Grabbing (Hold)
            playerControls.PlayerActions.leftGrab.performed += i => left_Input = true;
            playerControls.PlayerActions.leftGrab.canceled += i => left_Input = false;
            playerControls.PlayerActions.rightGrab.performed += i => right_Input = true;
            playerControls.PlayerActions.rightGrab.canceled += i => right_Input = false;

            /*
                        if(!playerMan.isInteracting){
                        playerControls.PlayerActions.crouch.performed += i => crouch_Input = true;

                        }
            */
        }

        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }

    public void HandleAllInputs()
    {
        HandleMovementInput();
        HandleJumpingInput();
        HandleGrabInput();
        //HandleSprintingInput();
        //HandleCrouchInput();
    }
    private void HandleGrabInput()
    {
        if(left_Input)
        {
            
        }

        if(right_Input)
        {

        }
    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        // moveSpeed = movementInput.magnitude;
        moveSpeed = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
    }

    private void HandleSprintingInput()
    {
        // if (sprint_Input && moveSpeed > 0.35f)
        // {
        //     playerLocomotion.isSprinting = true;
        // }
        // else
        // {
        //     playerLocomotion.isSprinting = false;
        // }
    }
    //private void HandleCrouchInput()
    //{
    //    if (crouch_Input && !crouch_Pressed)
    //    {
    //        playerLocomotion.isCrouched = !playerLocomotion.isCrouched;
    //        crouch_Input = false;
    //    }
    //    else if (!crouch_Input)
    //    {
    //        crouch_Pressed = false;
    //    }
    //}
    private void HandleJumpingInput()
    //jump buffer
    {
        if (jump_Input)
        {
            lastJumpPressTime = Time.time;
            jump_Input = false;
        }

        // if (Time.time - lastJumpPressTime <= jumpBufferTime)
        // {
        //     if (playerLocomotion.HandleJumping())
        //     {
        //         lastJumpPressTime = -999f;
        //     }
        // }
    }
}
