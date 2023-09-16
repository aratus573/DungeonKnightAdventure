using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item_SO : ScriptableObject
{
    public string itemName;
    public Sprite itemImage;
    public int itemHeld;
    [TextArea]
    public string itemDesc;
    public int value;
    public virtual void UseItem()
    {
    }
}
