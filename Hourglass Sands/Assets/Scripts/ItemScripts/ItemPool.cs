using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class ItemPool {
    public GoodsItem[] items;
    public float ComputeWeight(InventoryItem[] invItems) {
        float outWeight = 0f;
        foreach (InventoryItem invItem in invItems) {
            outWeight += items[invItem.id].weight * invItem.amount;
        }
        return outWeight;
    }
}
