using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using UnityEngine;
using UnityEngine.Events;

public class GenerativeInventory : InventoryComponent
{
    private MerchantInteractable mInt;
    private CacheInteractable cInt;
    private float mBaseAppearance = 0.4f;
    private float mBaseCount = 10f;

    private float cBaseAppearance = 0.3f;
    private float cBaseCount = 4f;

    private float weightCoeff = 1.5f;
    private ItemPoolReader itemPoolReader;
    public void Start() {
        mInt = GetComponent<MerchantInteractable>();
        cInt = GetComponent<CacheInteractable>();
        itemPoolReader = FindObjectOfType<ItemPoolReader>();
    }

    public void ClearInventory() {
        foreach (var item in itemInventory) {
            RemoveItem(item.id, item.amount);
        }
    }

    public void Regenerate(Dictionary<string, Dictionary<int, float>> scarcityMap) {
        if(mInt == null) {
            Debug.Log("Incroyable");
            mInt = GetComponent<MerchantInteractable>();
        }
        if(cInt == null) {
            cInt = GetComponent<CacheInteractable>();
        }
        if(itemPoolReader == null) {
            itemPoolReader = FindObjectOfType<ItemPoolReader>();
        }
        Debug.Log("Regenerating");
        if(mInt != null) {
            Debug.Log("Trade");
            ClearInventory();
            if(scarcityMap.ContainsKey(mInt.location)) {
                Dictionary<int, float> innerDict = scarcityMap.GetValueOrDefault(mInt.location);
                foreach (GoodsItem item in itemPoolReader.itemPool.items) {
                    float scarcity = innerDict[item.id];
                    if(Random.Range(0f, 1f) > mBaseAppearance * scarcity) {
                        int count = Mathf.FloorToInt(Random.Range(1f, mBaseCount / (scarcity * item.weight * weightCoeff) + 1f));
                        AddItem(item.id, count);
                        Debug.Log(item.id + ": " + count);
                    }
                }
            } else {
                foreach (GoodsItem item in itemPoolReader.itemPool.items) {
                    if(Random.Range(0f, 1f) > cBaseAppearance) {
                        int count = Mathf.FloorToInt(Random.Range(0f, cBaseCount / (item.weight * weightCoeff) + 1f));
                        AddItem(item.id, count);
                    }
                }
            }
        }
        if(cInt != null) {
            ClearInventory();
            foreach (GoodsItem item in itemPoolReader.itemPool.items) {
                if(Random.Range(0f, 1f) > cBaseAppearance) {
                    int count = Mathf.FloorToInt(Random.Range(0f, cBaseCount / (item.weight * weightCoeff) + 1f));
                    AddItem(item.id, count);
                }
            }
        }
    }
}
