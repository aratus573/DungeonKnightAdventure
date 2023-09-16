using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public AudioControl Audio;
    PlayerLocomotion playerLocomotion;

    public event Action<int, int> UpdateHealthBarOnAttack;
    public CharacterData_SO templateData;
    public AttackData_SO AttackTemplateData;
    public CharacterData_SO characterData;
    public AttackData_SO attackData;
    public GameObject FloatingTextPrefab;
    [HideInInspector]
    public bool isCritical;


    private void Awake()
    {
        if (templateData != null)
            characterData = Instantiate(templateData);
        if (AttackTemplateData != null)
            attackData = Instantiate(AttackTemplateData);
        if (gameObject.CompareTag("Player"))
        {
            playerLocomotion = GetComponent<PlayerLocomotion>();
        }
        Audio = GameObject.FindWithTag("Audio").GetComponent<AudioControl>();
    }

    #region Read from Data_SO
    public int MaxHealth
    {
        get { if (characterData != null) return characterData.maxHealth; else return 0; }
        set { characterData.maxHealth = value; }
    }

    public int CurrentHealth
    {
        get { if (characterData != null) return characterData.currentHealth; else return 0; }
        set { characterData.currentHealth = value; }
    }

    public int MaxLevel
    {
        get { if (characterData != null) return characterData.maxLevel; else return 0; }
        set { characterData.maxLevel = value; }
    }

    public int CurrentLevel
    {
        get { if (characterData != null) return characterData.currentLevel; else return 0; }
        set { characterData.currentLevel = value; }
    }

    public int CurrentEXP
    {
        get { if (characterData != null) return characterData.currentEXP; else return 0; }
        set { characterData.currentEXP = value; }
    }

    public float MoveSpeed
    {
        get { if (characterData != null) return characterData.moveSpeed; else return 0; }
        set { characterData.moveSpeed = value; }
    }

    public int STR
    {
        get { if (characterData != null) return characterData.strength; else return 0; }
        set { characterData.strength = value; }
    }

    public int AGI
    {
        get { if (characterData != null) return characterData.agility; else return 0; }
        set { characterData.agility = value; }
    }

    public int CON
    {
        get { if (characterData != null) return characterData.constitution; else return 0; }
        set { characterData.constitution = value; }
    }

    public int Points
    {
        get { if (characterData != null) return characterData.points; else return 0; }
        set { characterData.points = value; }
    }



    #endregion

    #region Character Combat

    public void TakeDamage(CharacterStats attacker, CharacterStats defender)
    {
        int damage = attacker.CurrentDamage();


    CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);

        if (attacker.isCritical)
        {
            ////defender.GetComponent<Animator>().SetTrigger("Hit");
        }

        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);

        if(defender != GameManager.Instance.playerStats)
        {
            if (attacker.isCritical)
                ShowFloatingText(damage.ToString(), Color.yellow);
            else
                ShowFloatingText(damage.ToString(), Color.white);
        }
        else
        {
            ShowFloatingText(damage.ToString(), Color.red);
        }
    }

    public void TakeDamage(int damage, CharacterStats defener)
    {
        CurrentHealth = Mathf.Max(CurrentHealth - damage, 0);
        UpdateHealthBarOnAttack?.Invoke(CurrentHealth, MaxHealth);

        ShowFloatingText(damage.ToString(), Color.yellow);
        
    }

    private int CurrentDamage()
    {
        float coreDamage = UnityEngine.Random.Range(attackData.minDamage, attackData.maxDamage);

        if (isCritical)
        {
            coreDamage *= attackData.criticalMultiplier;
            Debug.Log("Critical Damage: " + coreDamage);
        }

        return (int)coreDamage;
    }

    public void ShowFloatingText(string text, Color color, bool longer = false)
    {
        GameObject go = Instantiate(FloatingTextPrefab, this.gameObject.transform.position, Quaternion.identity, transform);
        go.GetComponent<FloatingText>().ChangeText(text, color);
        if(longer)
            go.GetComponent<Animator>().SetBool("Long",true);
    }

    #endregion

    #region Player Level Up


    public void GainXP(int xp)
    {
        GameManager.Instance.playerStats.CurrentEXP += xp;

        // Lv 1: exp 100, Lv2: exp 200, Lv3 : exp 300 , ...
        if(GameManager.Instance.playerStats.CurrentEXP >= 100 * GameManager.Instance.playerStats.CurrentLevel)
        {
            LevelUp();
            GameManager.Instance.playerStats.CurrentEXP = 0;
        }
    }

    public void LevelUp()
    {
        //TODO : Level Up Audio
        Audio.Levelup();
        playerLocomotion.HandleLevelUp();

        GameManager.Instance.playerStats.CurrentLevel += 1;
        GameManager.Instance.playerStats.Points += 3;
        GameManager.Instance.playerStats.CurrentHealth = GameManager.Instance.playerStats.MaxHealth;
    }
    public void STRUp()
    {
        GameManager.Instance.playerStats.STR += 1;
        GameManager.Instance.playerStats.Points -= 1;
        updateStats();
    }
    public void AGIUp()
    {
        GameManager.Instance.playerStats.AGI += 1;
        GameManager.Instance.playerStats.Points -= 1;
        updateStats();
    }
    public void CONUp()
    {
        GameManager.Instance.playerStats.CON += 1;
        GameManager.Instance.playerStats.Points -= 1;
        updateStats();
        GameManager.Instance.playerStats.CurrentHealth += 20;
    }

    public void updateStats()
    {
        //CON effects health, STR effects damage, AGI effects movement speed and dodge CD
        GameManager.Instance.playerStats.MaxHealth = 100 + GameManager.Instance.playerStats.CON * 20;
        GameManager.Instance.playerStats.attackData.minDamage = 10 + GameManager.Instance.playerStats.STR * 2;
        GameManager.Instance.playerStats.attackData.maxDamage = 12 + GameManager.Instance.playerStats.STR * 2;
        GameManager.Instance.playerStats.MoveSpeed = 2 + 0.35f * (float)GameManager.Instance.playerStats.AGI;
        GameManager.Instance.playerStats.gameObject.GetComponent<PlayerLocomotion>().ResetMovespeed();
        if (GameManager.Instance.playerStats.AGI < 17)
            GameManager.Instance.playerStats.gameObject.GetComponent<PlayerLocomotion>().dodgeCD = 5 - 0.3f * (float)GameManager.Instance.playerStats.AGI;
        else
            GameManager.Instance.playerStats.gameObject.GetComponent<PlayerLocomotion>().dodgeCD = 0.1f;

        //GameManager.Instance.playerStats.attackData.criticalChance = 0.2f +0.05f * (float) GameManager.Instance.playerStats.DEX ;

        if (GameManager.Instance.playerStats.CurrentHealth > GameManager.Instance.playerStats.MaxHealth)
        {
            GameManager.Instance.playerStats.CurrentHealth = GameManager.Instance.playerStats.MaxHealth;
        }
    }
    #endregion

}