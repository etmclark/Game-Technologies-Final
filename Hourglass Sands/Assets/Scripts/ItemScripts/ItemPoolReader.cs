using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.TestTools;

public class ItemPoolReader : MonoBehaviour
{
    public TextAsset itemsFile;
    public ItemPool itemPool;
    // Start is called before the first frame update
    void Start()
    {
        Assert.IsNotNull(itemsFile);
        itemPool = JsonUtility.FromJson<ItemPool>(itemsFile.text);
        // foreach (GoodsItem item in itemPool.items) 
        // {
        //     Debug.Log(item.name);
        // }
    }
}