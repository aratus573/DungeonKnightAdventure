using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Data" , menuName =  "Character Stats/Data") ]
public class CharacterData_SO : ScriptableObject
{
    [Header("Stats Info")]
    //basic stats
    public int maxHealth;
    public int currentHealth;
    public int maxLevel;
    public int currentLevel;
    public int currentEXP;
    public float moveSpeed;

    //three attributes
    public int strength;
    public int agility;
    public int constitution;
    // unassigned attributes
    public int points;
}
