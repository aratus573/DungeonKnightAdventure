using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    Animator animator;
    InputManager inputManager;
    PlayerLocomotion playerLocomotion;

    public bool isInteracting;
    public bool isUsingRootMotion;


    private void Awake()
    {
        animator = GetComponent<Animator>();
        inputManager = GetComponent<InputManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
    }


    private void Update()
    {
        inputManager.HandleAllInputs();
    }

    private void FixedUpdate()
    {
        playerLocomotion.HandleAllMovement();
    }
    private void LateUpdate()
    {
        // cameraManager.FollowTarget();

        isInteracting = animator.GetBool("isInteracting");
        playerLocomotion.isJumping = animator.GetBool("isJumping");
        isUsingRootMotion = animator.GetBool("isUsingRootMotion");
        animator.SetBool("isGrounded", playerLocomotion.isGrounded);
    }
}
