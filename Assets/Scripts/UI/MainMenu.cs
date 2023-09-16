using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class MainMenu : MonoBehaviour
{
    Button newGameBtn;
    Button quitBtn;
    public GameObject loadingScreen;
    public Slider slider;
    public TextMeshProUGUI percentText;
    public TextMeshProUGUI loadingText;
    public TextMeshProUGUI floorText;

    GameManager gameManager;

    void Awake()
    {
        newGameBtn = transform.GetChild(2).GetComponent<Button>();
        quitBtn = transform.GetChild(3).GetComponent<Button>();

        newGameBtn.onClick.AddListener(NewGame);
        quitBtn.onClick.AddListener(QuitGame);
    }


    private void Start()
    {
        loadingScreen.SetActive(false);
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("MainMenu"))
        {
            floorText.text = "Next Floor: 1";
        }
        else
        {
            floorText.text = "Next Floor: " + (GameManager.gameLevel + 1).ToString();
        }
    }

    private void updateLoadingText()
    {
        while (true)
        {
            loadingText.text = "Now Loading .";
            StartCoroutine(waitOneSceond());
            loadingText.text = "Now Loading ..";
            StartCoroutine(waitOneSceond());
            loadingText.text = "Now Loading ...";
            StartCoroutine(waitOneSceond());
        }
    }



    void NewGame()
    {
        StartCoroutine(LoadLevel("MainScene3D"));
    }

    IEnumerator LoadLevel(string scene)
    {
        if (scene != "")
        {
            // updateLoadingText();
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
            loadingScreen.SetActive(true);
            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / .9f);
                percentText.text = (progress * 99f).ToString("F0") + "%";
                slider.value = progress;
                yield return null;
            }
        }
    }

    void QuitGame()
    {
        // Debug.Log("Quit");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
    }

    IEnumerator waitOneSceond()
    {
        Debug.Log("Test");
        yield return new WaitForSecondsRealtime(1f);
    }
}
