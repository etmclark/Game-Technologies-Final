using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class InventoryItem {
    public int id;
    public int amount;
    public InventoryItem(int id) {
        this.id = id;
        this.amount = 1;
    }
    public InventoryItem(int id, int amount) {
        this.id = id;
        this.amount = amount;
    }
}
