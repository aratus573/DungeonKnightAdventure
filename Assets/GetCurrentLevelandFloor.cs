using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GetCurrentLevelandFloor : MonoBehaviour
{
    public TextMeshProUGUI CurrentLevel;
    public TextMeshProUGUI CurrentFloor;
    CharacterStats characterStats;


    void Update()
    {
        CurrentLevel.text = "LV : " + GameManager.Instance.playerStats.CurrentLevel.ToString();
        CurrentFloor.text = "Floor : " + (GameManager.gameLevel + 1).ToString();
    }
}
