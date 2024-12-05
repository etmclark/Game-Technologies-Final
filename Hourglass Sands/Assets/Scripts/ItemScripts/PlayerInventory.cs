using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : InventoryComponent
{
    public float currency;
    public UnityEvent e_CurrencyUpdated;
    public void PurchaseItem(int id, int amount, float costPer) {
        currency -= amount * costPer;
        e_CurrencyUpdated.Invoke();
        AddItem(id, amount);
    }
    public void SellItem(int id, int amount, float costPer) {
        currency += amount * costPer;
        e_CurrencyUpdated.Invoke();
        RemoveItem(id, amount);
    }
}
