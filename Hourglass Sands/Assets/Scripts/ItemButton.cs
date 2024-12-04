using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemButton : MonoBehaviour
{
    public ContentMediator conMed;
    private GoodsItem item = null;
    private int count = 0;
    public int butIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void LoadItem(GoodsItem item, int itemCount) {
        this.item = item;
        count = itemCount;
        SetTexture(item.iconFile);
    }

    public void SetTexture(string fileName) {
        Texture2D tex = Resources.Load<Texture2D>(fileName);
    }

    public void SetCount() {

    }
}
