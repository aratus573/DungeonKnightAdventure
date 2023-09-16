using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharacterStatsUI : MonoBehaviour, IEndGameObserver
{

    TMP_Text levelText;
    TMP_Text EXPText;
    TMP_Text STRText;
    TMP_Text AGIText;
    TMP_Text CONText;
    TMP_Text healthText;
    TMP_Text xpbarText;
    TMP_Text ATKText;
    TMP_Text CritChanceText;
    TMP_Text CritMultText;
    TMP_Text MSText;
    GameObject STRButton;
    GameObject AGIButton;
    GameObject CONButton;
    GameObject LVButton;
    Slider healthbar;
    Image fill;
    Slider xpbar;
    Image[] EquipImage;
    public bool UIActive;
    public bool CharUIActive;
    bool MenuUIActive;
    public Sprite EmptySlotSprite;
    public bool GameEnd;

    void Start()
    {
        STRText = this.gameObject.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>();
        AGIText = this.gameObject.transform.GetChild(1).GetChild(2).GetComponent<TMP_Text>();
        CONText = this.gameObject.transform.GetChild(1).GetChild(4).GetComponent<TMP_Text>();
        levelText = this.gameObject.transform.GetChild(1).GetChild(6).GetComponent<TMP_Text>();
        EXPText = this.gameObject.transform.GetChild(1).GetChild(8).GetComponent<TMP_Text>();
        ATKText = this.gameObject.transform.GetChild(1).GetChild(9).GetComponent<TMP_Text>();
        CritChanceText = this.gameObject.transform.GetChild(1).GetChild(10).GetComponent<TMP_Text>();
        CritMultText = this.gameObject.transform.GetChild(1).GetChild(11).GetComponent<TMP_Text>();
        MSText = this.gameObject.transform.GetChild(1).GetChild(12).GetComponent<TMP_Text>();
        STRButton = this.gameObject.transform.GetChild(1).GetChild(1).gameObject;
        AGIButton = this.gameObject.transform.GetChild(1).GetChild(3).gameObject;
        CONButton = this.gameObject.transform.GetChild(1).GetChild(5).gameObject;
        LVButton = this.gameObject.transform.GetChild(1).GetChild(7).gameObject;
        healthbar = this.gameObject.transform.GetChild(5).GetComponent<Slider>();
        healthText = this.gameObject.transform.GetChild(5).GetChild(2).GetComponent<TMP_Text>();
        fill = this.gameObject.transform.GetChild(5).GetChild(1).GetChild(0).GetComponent<Image>();
        xpbar = this.gameObject.transform.GetChild(7).GetComponent<Slider>();
        xpbarText = this.gameObject.transform.GetChild(7).GetChild(2).GetComponent<TMP_Text>();
        EquipImage = new Image[3];
        EquipImage[0] = this.gameObject.transform.GetChild(1).GetChild(14).GetComponent<Image>();
        EquipImage[1] = this.gameObject.transform.GetChild(1).GetChild(15).GetComponent<Image>();
        EquipImage[2] = this.gameObject.transform.GetChild(1).GetChild(16).GetComponent<Image>();
        UIActive = false;
        GameEnd = false;
        GameManager.Instance.AddObserver(this);
        GameManager.Instance.playerStats.updateStats();
    }

    void OnDisable()
    {
        if (!GameManager.IsInitialized)
            return;
        GameManager.Instance.RemoveObserver(this);
    }

    void Update()
    {
        if (GameEnd)
            return;

        if (CharUIActive)
        {
            RefreshCharUI();
            RefreshEquipmentUI();
        }

        RefreshHealthBar();
        RefreshXPBar();


        if (Input.GetKeyDown("i") || Input.GetKeyDown("c"))
        {
            SetActiveCharUI();
        }

        if (Input.GetKeyDown((KeyCode.Escape)))
        {
            SetActiveMenuUI();
        }

        if ( MenuUIActive || CharUIActive )
        {
            UIActive = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            UIActive = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        if (!GameManager.Instance.bossAlive)
        {
            this.gameObject.transform.GetChild(0).gameObject.SetActive(true);
            if (Input.GetKeyDown("l"))
            {
                GameManager.Instance.NextLevel();
            }
        }
        //this.gameObject.transform.GetChild(0).gameObject.SetActive(!GameManager.Instance.bossAlive);
    }

    public void EndNotify()
    {
        Cursor.lockState = CursorLockMode.None;
        GameEnd = true;
        RefreshHealthBar();

        this.gameObject.transform.GetChild(1).gameObject.SetActive(false);//Char Panel
        this.gameObject.transform.GetChild(2).gameObject.SetActive(false);//Inventory Panel
        this.gameObject.transform.GetChild(3).gameObject.SetActive(false);//Inventory Desc
        this.gameObject.transform.GetChild(4).gameObject.SetActive(false);// Cheat Item Button
        this.gameObject.transform.GetChild(5).gameObject.SetActive(false);//HP
        this.gameObject.transform.GetChild(6).gameObject.SetActive(false);//HP
        this.gameObject.transform.GetChild(7).gameObject.SetActive(false);//XP
        this.gameObject.transform.GetChild(8).gameObject.SetActive(false);// XP
        this.gameObject.transform.GetChild(9).gameObject.SetActive(false);//Quit Panel
        //gameObject.SetActive(false);
    }

    public void SetActiveCharUI()
    {
        CharUIActive = !CharUIActive;
        this.gameObject.transform.GetChild(1).gameObject.SetActive(CharUIActive);//Char Panel
        this.gameObject.transform.GetChild(2).gameObject.SetActive(CharUIActive);//Inventory Panel
        this.gameObject.transform.GetChild(3).gameObject.SetActive(CharUIActive);//Inventory Desc
        this.gameObject.transform.GetChild(4).gameObject.SetActive(CharUIActive);// Cheat Item Button
    }

    public void SetActiveMenuUI()
    {
        MenuUIActive = !MenuUIActive;
        if(MenuUIActive)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
        this.gameObject.transform.GetChild(9).gameObject.SetActive(MenuUIActive);// Menu Button
    }

    void RefreshCharUI()
    {
        levelText.text = "LV " + GameManager.Instance.playerStats.CurrentLevel;
        EXPText.text = "EXP " + GameManager.Instance.playerStats.CurrentEXP + " / " + 100 * GameManager.Instance.playerStats.CurrentLevel;
        ATKText.text = "ATK " + GameManager.Instance.playerStats.attackData.minDamage + " - " + GameManager.Instance.playerStats.attackData.maxDamage;
        CritChanceText.text = "Crit Chance " + GameManager.Instance.playerStats.attackData.criticalChance * 100 + " %";
        CritMultText.text = "Crit Multiplier " + GameManager.Instance.playerStats.attackData.criticalMultiplier * 100 + " %";
        MSText.text = "Speed " + GameManager.Instance.playerStats.MoveSpeed;
        STRText.text = "STR " + GameManager.Instance.playerStats.STR;
        AGIText.text = "AGI " + GameManager.Instance.playerStats.AGI;
        CONText.text = "CON " + GameManager.Instance.playerStats.CON;
        if (GameManager.Instance.playerStats.Points > 0)
        {
            STRButton.SetActive(true);
            AGIButton.SetActive(true);
            CONButton.SetActive(true);
        }
        else
        {
            STRButton.SetActive(false);
            AGIButton.SetActive(false);
            CONButton.SetActive(false);
        }
    }

    void RefreshEquipmentUI()
    {
        // equipment image
        for (int i = 0; i < 3; ++i)
        {
            if (Inventory.instance.playerInventory.equipList[i] != null)
                EquipImage[i].sprite = Inventory.instance.playerInventory.equipList[i].itemImage;
            else
                EquipImage[i].sprite = EmptySlotSprite;
        }
    }

    void RefreshHealthBar()
    {
        healthbar.value = (float)GameManager.Instance.playerStats.CurrentHealth / (float)GameManager.Instance.playerStats.MaxHealth;
        healthText.text = GameManager.Instance.playerStats.CurrentHealth + " / " + GameManager.Instance.playerStats.MaxHealth;
        fill.color = Color.Lerp(Color.red, Color.green, (float)GameManager.Instance.playerStats.CurrentHealth / (float)GameManager.Instance.playerStats.MaxHealth);
        //change color between red and green
    }

    private void RefreshXPBar()
    {
        xpbar.value = (float)GameManager.Instance.playerStats.CurrentEXP / (float)(100 * GameManager.Instance.playerStats.CurrentLevel);
        xpbarText.text = GameManager.Instance.playerStats.CurrentEXP + " / " + 100 * GameManager.Instance.playerStats.CurrentLevel;
    }

}
