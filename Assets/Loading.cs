using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class Loading : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI percentText;
    public TextMeshProUGUI floorText;
    public GameObject loadingscreen;

    private void Start()
    {
        loadingscreen.SetActive(false);
    }

    public IEnumerator LoadLevel(string scene)
    {
        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("MainMenu"))
        {
            floorText.text = "Next Floor: 1";
        }
        else
        {
            floorText.text = "Next Floor: " + (GameManager.gameLevel + 2).ToString();
        }

        if (scene != "")
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
            loadingscreen.SetActive(true);
            while (!operation.isDone)
            {
                float progress = Mathf.Clamp01(operation.progress / .9f);
                percentText.text = (progress * 99f).ToString("F0") + "%";
                slider.value = progress;
                yield return null;
            }
        }
    }
}
