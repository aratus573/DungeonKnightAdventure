using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equipment", menuName = "Inventory/New Equipment")]
public class Equipment_SO : Item_SO
{
    public EquipmentSlot equipSlot;
    public EquipOperation operation;
    public override void UseItem()
    {

        Equipment_SO oldItem = Inventory.instance.playerInventory.equipList[(int)equipSlot];
        if (oldItem != null)
        {
            Inventory.instance.AddItem(oldItem);
            oldItem.Unequip();
        }

        int V = base.value;
        switch (operation)
        {
            case EquipOperation.STR:
                STROp(V);
                break;
            case EquipOperation.AGI:
                AGIOp(V);
                break;
            case EquipOperation.CON:
                CONOp(V);
                break;
            default:
                break;
        }
        Inventory.instance.playerInventory.equipList[(int)equipSlot] = this;
        Debug.Log("equip slot = " + (int)equipSlot);
        GameManager.Instance.playerStats.updateStats();
        //EquipmentManager.instance.Equip(this);
    }

    public void Unequip()
    {
        int V = -base.value;
        Debug.Log("unequip value = " + V);

        switch (operation)
        {
            case EquipOperation.STR:
                STROp(V);
                break;
            case EquipOperation.AGI:
                AGIOp(V);
                break;
            case EquipOperation.CON:
                CONOp(V);
                break;
            default:
                break;
        }
        Inventory.instance.playerInventory.equipList[(int)equipSlot] = null;
        //GameManager.Instance.playerStats.updateStats();
    }

    void STROp(int value)
    {
        GameManager.Instance.playerStats.STR += value;
    }
    void AGIOp(int value)
    {
        GameManager.Instance.playerStats.AGI += value;
    }
    void CONOp(int value)
    {
        GameManager.Instance.playerStats.CON += value;
    }

}

public enum EquipmentSlot { Head, Chest, Weapon }
public enum EquipOperation { STR, AGI, CON }