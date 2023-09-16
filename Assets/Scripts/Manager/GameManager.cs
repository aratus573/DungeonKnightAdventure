using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public CharacterStats playerStats;
    public bool bossAlive;
    public static int gameLevel;
    public static bool firstGame = true;
    public static float CameraSensitivity = 1;
    public static float Volume = 0.5f;
    public Loading loadingscreen;
    List<IEndGameObserver> endGameObserves = new List<IEndGameObserver>();

    void Start()
    {
        Debug.Log("firstgame = " + firstGame);
        if (firstGame)
        {
            ResetLevel();
            firstGame = false;
        }
        Time.timeScale = 1;
    }

    public void RigisterPlayer(CharacterStats player)
    {
        playerStats = player;
    }

    public void ResetLevel()
    {
        gameLevel = 0;
        playerStats.CurrentLevel = 1;
        playerStats.CurrentEXP = 0;
        playerStats.CurrentHealth = 100;
        playerStats.MaxHealth = 100;
        playerStats.Points = 3;
        playerStats.STR = 0;
        playerStats.AGI = 0;
        playerStats.CON = 0;
        Inventory.instance.UnequipAll();
        Inventory.instance.RemoveAllItem();
        playerStats.updateStats();
    }

    public void endGame()
    {
        firstGame = true;
        Inventory.instance.UnequipAll();
        Inventory.instance.RemoveAllItem();
    }

    public void NextLevel()
    {
        StartCoroutine(loadingscreen.LoadLevel("MainScene3D"));
        gameLevel += 1;
        //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void AddObserver(IEndGameObserver observer)
    {
        endGameObserves.Add(observer);
    }

    public void RemoveObserver(IEndGameObserver observer)
    {
        endGameObserves.Remove(observer);
    }


    public void NotifyObservers()
    {
        foreach (var observer in endGameObserves)
        {
            observer.EndNotify();
        }
    }
}
