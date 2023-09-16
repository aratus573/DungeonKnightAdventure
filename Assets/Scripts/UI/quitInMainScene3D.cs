using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using System;

public class quitInMainScene3D : MonoBehaviour
{
    Button quitBtn;


    void Awake()
    {
        quitBtn = GetComponent<Button>();

        quitBtn.onClick.AddListener(returnMainMenu);
    }

    void returnMainMenu()
    {
        PlayerPrefs.DeleteAll();
        GameManager.Instance.endGame();
        StartCoroutine(LoadLevel("Main Menu"));
    }

    IEnumerator LoadLevel(string scene)
    {
        if (scene != "")
        {
            AsyncOperation operation = SceneManager.LoadSceneAsync(scene);
            yield return null;
        }
    }
}
