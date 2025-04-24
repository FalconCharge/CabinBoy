using System.Collections;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
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
    InputManager inputManager;
    PlayerLocomotion playerLocomotion;
    public string currentStatee;
    public bool isInteracting;
    [SerializeField] private bool inputs;
    [SerializeField] private bool movement;
    [SerializeField] private bool visuals;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }
    private void Start()
    {
    }

    private void Update()
    {
        if (inputs)
        {
            inputManager.HandleAllInputs();
        }
    }

    private void FixedUpdate()
    {

        if (movement)
        {
            playerLocomotion.HandleAllMovement();
        }

        if (visuals)
        {
            //proceduralAnimate.HandleAllVisuals();
        }
    }
    private void LateUpdate()
    {
        // isInteracting = ani.GetBool("isInteracting");
        // playerLocomotion.isJumping = ani.GetBool("isJumping");
    }
}
