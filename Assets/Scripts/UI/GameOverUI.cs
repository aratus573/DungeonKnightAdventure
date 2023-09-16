using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameOverUI : MonoBehaviour, IEndGameObserver
{
    PlayerLocomotion playerLocomotion;
    InputManager inputManager;
    EnemyController enemyController;
    CharacterStats characterStats;

    //GameManager gameManager;
    Button newGameBtn;
    Button mainMenuBtn;
    public TextMeshProUGUI CurrentFloor;
    public TextMeshProUGUI CurrentLv;

    void Awake()
    {
        playerLocomotion = GameObject.FindWithTag("Player").GetComponent<PlayerLocomotion>();
        inputManager = GameObject.FindWithTag("Player").GetComponent<InputManager>();
        characterStats = GameObject.FindWithTag("Player").GetComponent<CharacterStats>();

        newGameBtn = transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<Button>();
        mainMenuBtn = transform.GetChild(0).GetChild(0).GetChild(2).GetComponent<Button>();

        newGameBtn.onClick.AddListener(NewGame);
        mainMenuBtn.onClick.AddListener(QuitGame);
    }

    private void Start()
    {
        transform.GetChild(0).gameObject.SetActive(false);
        GameManager.Instance.AddObserver(this);
    }

    void OnDisable()
    {
        if (!GameManager.IsInitialized)
            return;
        GameManager.Instance.RemoveObserver(this);
    }

    public void EndNotify()
    {
        GameOver();
    }

    public void GameOver()
    {
        inputManager.OnDisable();
        inputManager.movementInput.x = 0;
        inputManager.movementInput.y = 0;
        transform.GetChild(0).gameObject.SetActive(true);
        CurrentFloor.text = "Current Floor: " + (GameManager.gameLevel + 1).ToString();
        CurrentLv.text = "Current Lv: " + (characterStats.CurrentLevel).ToString();
        //GameManager.Instance
    }

    public void NewGame()
    {
        //PlayerPrefs.DeleteAll();
        GameManager.Instance.endGame();
        StartCoroutine(LoadLevel("MainScene3D"));
    }

    public void QuitGame()
    {
        PlayerPrefs.DeleteAll();
        GameManager.Instance.endGame();
        StartCoroutine(LoadLevel("Main Menu"));
    }

    IEnumerator LoadLevel(string scene)
    {
        if (scene != "")
        {
            // updateLoadingText();
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
            yield return null;
        }
    }
}
