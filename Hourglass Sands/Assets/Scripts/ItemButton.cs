using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    public GameObject imageObject;
    public ContentMediator conMed;
    private GoodsItem item = null;
    private int count = 0;
    public int butIndex = 0;
    private RawImage imageComponent;
    // Start is called before the first frame update
    void Start()
    {
        imageComponent = imageObject.GetComponent<RawImage>();
    }

    public void LoadItem(GoodsItem item, int itemCount) {
        this.item = item;
        count = itemCount;
        SetTexture(item.iconFile);
    }

    public void SetTexture(string fileName) {
        Texture2D tex = Resources.Load<Texture2D>("Art/Icons/Free-Fruit-Vector-Icon-Pack-for-RPG/PNG/without_shadow/" + fileName);
        imageComponent.texture = tex;
    }

    public void SetCount() {

    }
}
