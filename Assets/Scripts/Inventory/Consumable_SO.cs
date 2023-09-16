using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable", menuName = "Inventory/New Consumable")]
public class Consumable_SO : Item_SO
{
    public ConsumeOperation operation;

    public override void UseItem()
    {
        switch (operation)
        {
            case ConsumeOperation.Heal:
                HealOp(base.value);
                break;
            default:
                break;
        }
        GameManager.Instance.playerStats.updateStats();
    }
    void HealOp(int v)
    {
        GameManager.Instance.playerStats.CurrentHealth += v;
    }

}

public enum ConsumeOperation { Heal }