using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    PlayerManager playerManager;
    AnimatorManager animatorManager;
    InputManager inputManager;
    CharacterStats characterStats;
    PlayerController playerController;
    [HideInInspector]
    public Rigidbody playerRigidbody;

    Vector3 moveDirection;
    Transform cameraObject;

    [Header("Falling")]
    [HideInInspector]
    public float inAirTimer;
    public float leapingVelocity;
    public float fallingVelocity;
    public float rayCastHeightOffset = 0.5f;
    public LayerMask groundLayer;

    [Header("Movement Flags")]
    [HideInInspector]
    public bool isSprinting;
    [HideInInspector]
    public bool isJumping;
    [HideInInspector]
    public bool isGrounded;
    [HideInInspector]
    public bool isAttacking;
//    [HideInInspector]
    public bool isStopAllInputExcludeDodge;
//    [HideInInspector]
    public bool isStopAllMovement;

    [Header("Movement Speed")]
    public float movementSpeed = 5;
    public float rotationSpeed = 15;
    public float runLowerLimit = 6;
    public float sprintLowerLomit = 16;

    [Header("Jump Speed")]
    public float jumpHeight = 3;
    public float gravityIntensity = -15;

    [Header("CD time")]
    /*
    public float attackCD;
    public float remainAttackCD;
    */
    public float dodgeCD;
    public float remainDodgeCD;

    [Header("Prefab")]
    public GameObject RangedSpellPrefab;
    public GameObject LightningPrefab;
    public GameObject LevelUpPrefab;

    public float FireBallTime = 3f;

    [HideInInspector]
    public Vector3 originPosition;

    private void Awake()
    {
        playerManager = GetComponent<PlayerManager>();
        animatorManager = GetComponent<AnimatorManager>();
        inputManager = GetComponent <InputManager>();
        characterStats = GetComponent<CharacterStats>();
        playerController = GetComponent<PlayerController>();
        playerRigidbody = GetComponent<Rigidbody>();
        cameraObject = Camera.main.transform;
        //remainDodgeCD = dodgeCD;
        remainDodgeCD = 0;
        movementSpeed = characterStats.MoveSpeed;
    }

    public void HandleAllMovement()
    {
        HandleFallingAndLanding();
        if (playerManager.isInteracting)
            return;

        HandleMovement();
        HandleRotation();
    }

    public void HandleRangedSpell()
    {
        Vector3 SpawnSpellLoc = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        GameObject clone;
        clone = Instantiate(RangedSpellPrefab , SpawnSpellLoc + transform.forward, Quaternion.identity);
        originPosition = transform.position;
    }

    public void HandleLightning()
    {
        Vector3 SpawnSpellLoc = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
        GameObject clone;
        clone = Instantiate(LightningPrefab, SpawnSpellLoc, Quaternion.identity);
        originPosition = transform.position;
    }

    public void HandleLevelUp()
    {
        // add particle
        Vector3 SpawnSpellLoc = new Vector3(transform.position.x, transform.position.y, transform.position.z);

        GameObject clone;
        clone = Instantiate(LevelUpPrefab, SpawnSpellLoc + transform.forward, Quaternion.identity);
        originPosition = transform.position;
    }
    /*
        void FoundEnemy()
        {
            // judge radius
            Collider[] colliders = Physics.OverlapBox(transform.position + new Vector3(0, 1, 1), new Vector3(1.5f, 2, 1.5f), transform.rotation);

            foreach (var target in colliders)
            {
                if (target.CompareTag("Enemy") || target.CompareTag("Attackable"))
                {
                    playerController.attackTarget = target.gameObject;
                    playerController.Hit();
                    //stop hitting multiple target
                    break;
                }
            }
        }
    */
    private void HandleMovement()
    {
        if (isJumping)
            return;
        moveDirection = cameraObject.forward * inputManager.verticalInput;
        moveDirection = moveDirection + cameraObject.right * inputManager.horizonalInput;
        moveDirection.Normalize();
        moveDirection.y = 0;

        moveDirection = moveDirection * movementSpeed;

        Vector3 movementVelocity = moveDirection;
        playerRigidbody.velocity = movementVelocity;
    }

    private void HandleRotation()
    {
        if (isJumping)
            return;

        Vector3 targetDirection = Vector3.zero;

        targetDirection = cameraObject.forward * inputManager.verticalInput;
        targetDirection = targetDirection + cameraObject.right * inputManager.horizonalInput;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero)
            targetDirection = transform.forward;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }

    private void HandleFallingAndLanding()
    {
        RaycastHit hit;
        Vector3 rayCastOrigin = transform.position;
        Vector3 targetPosition;
        rayCastOrigin.y = rayCastOrigin.y + rayCastHeightOffset;
        targetPosition = transform.position;

        if (!isGrounded && !isJumping)
        {
            if (!playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Falling", true);
            }

            animatorManager.animator.SetBool("isUsingRootMotion", false);
            inAirTimer = inAirTimer + Time.deltaTime;
            playerRigidbody.AddForce(transform.forward * leapingVelocity);
            playerRigidbody.AddForce(-Vector3.up * fallingVelocity * inAirTimer);
        }

        if(Physics.SphereCast(rayCastOrigin, 0.2f, -Vector3.up, out hit, groundLayer))
        {
            if(!isGrounded && !playerManager.isInteracting)
            {
                animatorManager.PlayTargetAnimation("Land", true);
            }

            Vector3 rayCastHitPoint = hit.point;
            targetPosition.y = rayCastHitPoint.y;
            inAirTimer = 0;
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }

        if(isGrounded && !isJumping)
        {
            if(playerManager.isInteracting || inputManager.moveAmount > 0)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime / 0.1f);
            }
            else
            {
                transform.position = targetPosition;
            }
        }
    }

    public void HandleJumping()
    {
        if (isGrounded)
        {
            animatorManager.animator.SetBool("isJumping", true);
            animatorManager.PlayTargetAnimation("Jump", false);

            float jumpingVelocity = Mathf.Sqrt(-2 * gravityIntensity * jumpHeight);
            Vector3 playerVelocity = moveDirection;
            playerVelocity.y = jumpingVelocity;
            playerRigidbody.velocity = playerVelocity;
        }
    }
    public void HandleDodge()
    {
        if (playerManager.isInteracting)
            return;
        remainDodgeCD = dodgeCD;

        animatorManager.PlayTargetAnimation("Dodge", true, true);

    }

    public void HandleAttack()
    {
        if (playerManager.isInteracting)
            return;
        animatorManager.animator.SetBool("Critical", characterStats.isCritical);
        animatorManager.animator.SetTrigger("isAttacking");
    }

    public void StopMove()
    {
        movementSpeed = 0;
    }

    public void ResetMovespeed()
    {
        movementSpeed = characterStats.MoveSpeed;
    }


}
