using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilitySystem : MonoBehaviour
{
    PlayerLocomotion playerLocomotion;
    PlayerController playerController;
    InputManager inputManager;

    //DRAG THIS SCRIPT INTO YOUR CANVAS OBJECT!
    [Header("Ability1")]
    public Image abilityImage1;
    public float cooldown1 = 8f;
    public float remainCoolDown1;

    [Header("Ability2")]
    public Image abilityImage2;
    public float cooldown2 = 10f;
    public float remainCoolDown2;

    [Header("Ability3")]
    public Image abilityImage3;

    private void Awake()
    {
        if (GameObject.FindGameObjectWithTag("Player"))
            playerLocomotion = GameObject.FindWithTag("Player").GetComponent<PlayerLocomotion>();
        if (GameObject.FindGameObjectWithTag("Player"))
            playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        if (GameObject.FindGameObjectWithTag("Player"))
            inputManager = GameObject.FindWithTag("Player").GetComponent<InputManager>();
    }

    void Start()
    {
        abilityImage1.fillAmount = 1;
        abilityImage2.fillAmount = 1;
        abilityImage3.fillAmount = 1;
    }

    void Update()
    {
        if (remainCoolDown1 > 0)
            remainCoolDown1 -= Time.deltaTime;
        else
            remainCoolDown1 = 0;

        if (remainCoolDown2 > 0)
            remainCoolDown2 -= Time.deltaTime;
        else
            remainCoolDown2 = 0;

        abilityImage1.fillAmount = remainCoolDown1 / cooldown1;
        abilityImage2.fillAmount = remainCoolDown2 / cooldown2;


        //Ability3
        if (playerLocomotion.remainDodgeCD > 0)
        {
            abilityImage3.fillAmount = playerLocomotion.remainDodgeCD / playerLocomotion.dodgeCD;
        }

        if ((Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) && Input.GetKey(KeyCode.Mouse1))
        {
            if (remainCoolDown2 == 0)
                Ability2();
        }
        else if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (remainCoolDown1 == 0)
                Ability1();
        }
    }

    public void Ability1()
    {
        if (playerController.isDead)
            return;
        playerLocomotion.HandleRangedSpell();    //You can replace this with any ability you want :)
        remainCoolDown1 = cooldown1;
        abilityImage1.fillAmount = 1;
    }

    public void Ability2()
    {
        if (playerController.isDead)
            return;
        playerLocomotion.HandleLightning();    //You can replace this with any ability you want :)
        remainCoolDown2 = cooldown2;
        abilityImage2.fillAmount = 1;
    }
}