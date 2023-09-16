using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/New Inventory")]
public class Inventory_SO : ScriptableObject
{
    public List<Item_SO> itemList = new List<Item_SO>();

    public  List<Equipment_SO> equipList = new List<Equipment_SO>();
}
