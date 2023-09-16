using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    PlayerControls playerControls;
    PlayerLocomotion playerLocomotion;
    AnimatorManager animatorManager;
    CharacterStats characterStats;
    PlayerController playerController;
    AbilitySystem abilitySystem;

    public Vector2 movementInput;
    public float moveAmount;
    public float verticalInput;
    public float horizonalInput;

//    [HideInInspector]
    public bool jump_Input;
//    [HideInInspector]
    public bool Alt_Input;
//    [HideInInspector]
    public bool attack_Input;

    private void Awake()
    {
        playerLocomotion = GetComponent<PlayerLocomotion>();
        animatorManager = GetComponent<AnimatorManager>();
        characterStats = GetComponent<CharacterStats>();
        playerController = GetComponent<PlayerController>();
        abilitySystem = GameObject.FindWithTag("Game Canvas").GetComponent<AbilitySystem>();
    }

    private void Update()
    {
        if (playerLocomotion.remainDodgeCD > 0)
            playerLocomotion.remainDodgeCD -= Time.deltaTime;
        else
            playerLocomotion.remainDodgeCD = 0;
        /*
        if (playerLocomotion.remainAttackCD > 0)
            playerLocomotion.remainAttackCD -= Time.deltaTime;
        else
            playerLocomotion.remainAttackCD = 0;
        */
        if (playerController.isDead)
            OnDisable();
    }

    IEnumerator waitThreeSeconds()
    {
        yield return new WaitForSecondsRealtime(3);
    }

    public void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerActions.Jump.performed += i => jump_Input = true;
            playerControls.PlayerActions.Alt.performed += i => Alt_Input = true;
            playerControls.PlayerActions.Attack.performed += i => attack_Input = true;
        }

        playerControls.Enable();
    }

    public void OnDisable()
    {
        playerControls.PlayerActions.Jump.canceled += i => jump_Input = false;
        playerControls.PlayerActions.Alt.canceled += i => Alt_Input = false;
        playerControls.PlayerActions.Attack.canceled += i => attack_Input = false;
        playerControls.Disable();
    }

    public void HandleAllInputs()
    {
        HandleDodgeInput();

        if (playerLocomotion.isStopAllInputExcludeDodge)
            return;

        HandleMovevmentInput();

        if (playerLocomotion.isStopAllMovement)
            return;

        HandleJumpingInput();
        HandleAttackInput();
    }

    private void HandleMovevmentInput()
    {
        verticalInput = movementInput.y;
        horizonalInput = movementInput.x;
        moveAmount = Mathf.Clamp01(Mathf.Abs(horizonalInput) + Mathf.Abs(verticalInput));
        animatorManager.UpdateAnimatorValues(0, moveAmount);
    }

    private void HandleJumpingInput()
    {
        if (jump_Input)
        {
            OnDisable();
            jump_Input = false;
            if(!playerLocomotion.isStopAllMovement)
                playerLocomotion.HandleJumping();
            OnEnable();
        }
    }

    private void HandleDodgeInput()
    {
        if (playerLocomotion.remainDodgeCD > 0)
            Alt_Input = false;
        else if (Alt_Input)
        {
            Alt_Input = false;
            abilitySystem.abilityImage3.fillAmount = 1;
            //abilitySystem.isCooldown3 = true;
            playerLocomotion.HandleDodge();
        }
    }

    private void HandleAttackInput()
    {
        if (attack_Input/* && playerLocomotion.remainAttackCD <= 0*//* && readyToAttack()*/)
        {
            OnDisable();
            attack_Input = false;
            if (!playerLocomotion.isStopAllMovement)
            {
                characterStats.isCritical = UnityEngine.Random.value < characterStats.attackData.criticalChance;
                playerLocomotion.HandleAttack();
            }
            OnEnable();
            // playerLocomotion.remainAttackCD = playerLocomotion.attackCD;
        }
    }

    /*
    private bool readyToAttack()
    {
        // judge radius
        var colliders = Physics.OverlapSphere(transform.position, Mathf.Max(characterStats.attackData.attackRange, characterStats.attackData.skillRange));

        foreach (var target in colliders)
        {
            if (target.CompareTag("Enemy"))
            {
                return true;
            }
        }
        return false;
    }

    */
}
