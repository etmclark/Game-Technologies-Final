using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InventoryComponent : MonoBehaviour
{
    public UnityEvent e_InventoryUpdated;
    public UnityEvent<int> e_ItemRemoved;
    //perhaps this Inventory ought to be a dict, but until it has a probably displaysort 
    public List<InventoryItem> itemInventory;
    public virtual int AddItem(int id, int amount) {
        for (int i = 0; i < itemInventory.Count; i++) {
            if (itemInventory[i].id == id) {
                itemInventory[i].amount += amount;
                e_InventoryUpdated.Invoke();
                return -1;
            }
        }
        itemInventory.Add(new InventoryItem(id, amount));
        e_InventoryUpdated.Invoke();
        return itemInventory.Count - 1;
    }
    public virtual int RemoveItem(int id, int amount) {
        for (int i = 0; i < itemInventory.Count; i++) {
            if (itemInventory[i].id == id) {
                if (itemInventory[i].amount <= amount) {
                    itemInventory.RemoveAt(i);
                    e_ItemRemoved.Invoke(i);
                    e_InventoryUpdated.Invoke();
                    return i; 
                } else {
                    itemInventory[i].amount -= amount;
                    e_InventoryUpdated.Invoke();
                    return i;
                }
            }
        }
        return -1;
    }
}
