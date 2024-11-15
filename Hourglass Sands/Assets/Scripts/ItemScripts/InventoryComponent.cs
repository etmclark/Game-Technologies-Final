using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryComponent : MonoBehaviour
{
    UnityEvent e_InventoryUpdated;
    //perhaps this Inventory ought to be a dict, but until it has a probably displaysort 
    public List<InventoryItem> itemInventory;
    public virtual void AddItem(int id, int amount) {
        for (int i = 0; i < itemInventory.Count; i++) {
            if (itemInventory[i].id == id) {
                itemInventory[i].amount += amount;
                e_InventoryUpdated.Invoke();
                return;
            }
        }
        itemInventory.Add(new InventoryItem(id, amount));
        e_InventoryUpdated.Invoke();
    }
    public virtual void RemoveItem(int id, int amount) {
        for (int i = 0; i < itemInventory.Count; i++) {
            if (itemInventory[i].id == id) {
                if (itemInventory[i].amount <= amount) {
                    itemInventory.RemoveAt(i);
                    e_InventoryUpdated.Invoke();
                    return; 
                } else {
                    itemInventory[i].amount -= amount;
                    e_InventoryUpdated.Invoke();
                    return;
                }
            }
        }
    }
}