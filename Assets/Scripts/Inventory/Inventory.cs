using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    public Inventory_SO playerInventory;
    public GameObject InventoryGrid;
    public List<GameObject> slots = new List<GameObject>();
    public GameObject emptySlot;
    public TMP_Text itemDesc;
    public List<Item_SO> Loot0 = new List<Item_SO>();
    public List<Item_SO> Loot1 = new List<Item_SO>();
    public List<Item_SO> Loot2 = new List<Item_SO>();
    public List<Item_SO> Loot3 = new List<Item_SO>();

    void Awake()
    {
        if (instance != null)
            Destroy(this);
        instance = this;
    }
    void OnEnable()
    {
        RefreshItem();
    }

    public void UpdateItemDesc(string desc)
    {
        itemDesc.text = desc;
    }


    public void AddItem(int BossLevel = 0)
    {
        Debug.Log("add item");
        int Itemtype = 0;
        int rand = 0;
        if (BossLevel != 0)
        {
            rand = Random.Range(80, 100);
        }
        else
            rand = Random.Range(0, 100);

        switch (rand)
        {
            //80% LV0 item(heal potion), 12% LV1 equipment, 6% LV2 equipment, 2% LV3 equipment
            case int n when (n < 80):
                Itemtype = n % Loot0.Count;
                AddItem(Loot0[Itemtype]);
                break;

            case int n when (n < 92 && n >= 80):
                Itemtype = n % Loot1.Count;
                AddItem(Loot1[Itemtype]);
                break;

            case int n when (n < 98 && n >= 92):
                Itemtype = n % Loot2.Count;
                AddItem(Loot2[Itemtype]);
                break;

            case int n when (n >= 98):
                Itemtype = n % Loot3.Count;
                AddItem(Loot3[Itemtype]);
                break;

            default:
                AddItem(Loot0[0]);
                break;
        }
    }

    public void AddItem(Item_SO Item)
    {
        if (playerInventory.itemList.Contains(Item))
        {
            Item.itemHeld += 1;
        }
        else
        {
            for (int i = 0; i < playerInventory.itemList.Count; ++i)
            {
                if(playerInventory.itemList[i] == null)
                {
                    playerInventory.itemList[i] = Item;
                    Item.itemHeld = 1;
                    break;
                }
            }
        }
        GameManager.Instance.playerStats.ShowFloatingText("Get " + Item.itemName, Color.blue, true);
        RefreshItem();
    }

    public void RemoveItem(Item_SO Item)
    {
        Debug.Log("RemoveItem " );
        if (Item.itemHeld > 1)
        {
            Item.itemHeld -= 1;
        }
        else if(Item.itemHeld == 1)
        {
            Item.itemHeld -= 1;
            playerInventory.itemList[playerInventory.itemList.IndexOf(Item)] = null;
        }
        else
        {
            Debug.Log("error, no item");
        }

        RefreshItem();
    }

    public void RefreshItem()
    {
        for(int i = 0; i < InventoryGrid.transform.childCount; ++i)
        {
            if (InventoryGrid.transform.childCount == 0)
                break;
            Destroy(InventoryGrid.transform.GetChild(i).gameObject);
            slots.Clear();
        }
        for(int i=0;i<playerInventory.itemList.Count; ++i)
        {
            if (playerInventory.itemList[i] != null && playerInventory.itemList[i].itemHeld == 0)
            {
                playerInventory.itemList[i] = null;
            }
            slots.Add(Instantiate(emptySlot, InventoryGrid.transform));
            slots[i].GetComponent<ItemSlot>().SetupSlot(playerInventory.itemList[i]);

        }
    }

    public void RemoveAllItem()
    {
        for (int i = 0; i < playerInventory.itemList.Count; ++i)
        {
            if(playerInventory.itemList[i] != null)
            {
                playerInventory.itemList[i].itemHeld = 0;
                playerInventory.itemList[i] = null;
            }
        }
        RefreshItem();
    }


    //equipment manager

    public void UnequipAll()
    {
        for (int i = 0; i < playerInventory.equipList.Count; ++i)
        {
            if (playerInventory.equipList[i] != null)
            {
                playerInventory.equipList[i].Unequip();
                Inventory.instance.AddItem(playerInventory.equipList[i]);
                playerInventory.equipList[i] = null;
            }
        }
    }
}
