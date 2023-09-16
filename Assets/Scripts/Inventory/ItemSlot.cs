using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    public Item_SO slotItem;
    public Image slotImage;
    public TMP_Text slotNum;
    public string slotDesc;

    public void ItemOnClicked()
    {
        Inventory.instance.UpdateItemDesc(slotDesc);
    }

    public void SetupSlot(Item_SO Item)
    {
        if (Item == null )
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {

            slotItem = Item;
            slotImage.sprite = Item.itemImage;
            slotNum.text = Item.itemHeld.ToString();
            transform.GetChild(0).gameObject.SetActive(true);
            slotDesc = Item.itemDesc;
        }
    }

    public void Use()
    {
        if (slotItem == null)
            return;
        Debug.Log("Use Item");
        slotItem.UseItem();
        Inventory.instance.RemoveItem(slotItem);
    }
    
}
