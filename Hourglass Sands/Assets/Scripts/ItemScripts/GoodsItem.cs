using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class GoodsItem {
    public int id;
    public string name;
    public string description;
    public string iconFile;
    public float basePrice;
    public float thirstRegen = -1f;
}
