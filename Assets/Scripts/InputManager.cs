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

    public bool sprint_Input;
    public bool crouch_Input;
    public bool crouch_Pressed;
    public bool jump_Input = false;

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

            playerControls.PlayerActions.jump.performed += i => jump_Input = true;

            playerControls.PlayerActions.sprint.performed += i => sprint_Input = true;
            playerControls.PlayerActions.sprint.canceled += i => sprint_Input = false;

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
        //HandleSprintingInput();
        //HandleCrouchInput();
    }

    private void HandleMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        moveSpeed = Mathf.Clamp01(Mathf.Abs(horizontalInput) + Mathf.Abs(verticalInput));
    }

    private void HandleSprintingInput()
    {
        if (sprint_Input && moveSpeed > 0.35f)
        {
            playerLocomotion.isSprinting = true;
        }
        else
        {
            playerLocomotion.isSprinting = false;
        }
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
    {
        if (jump_Input)
        {
            playerLocomotion.HandleJumping();
            jump_Input = false;
        }
    }
}
