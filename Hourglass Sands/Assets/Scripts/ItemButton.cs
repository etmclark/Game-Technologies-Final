using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    public ContentMediator conMed;
    private GoodsItem item = null;
    private int count = 0;
    public int butIndex = 0;
    private RawImage imageComponent;
    private TMP_Text textComponent;
    // Start is called before the first frame update
    void Awake()
    {
        imageComponent = gameObject.GetComponentInChildren<RawImage>();
        textComponent = gameObject.GetComponentInChildren<TMP_Text>();
    }

    public void LoadItem(GoodsItem item, int itemCount) {
        this.item = item;
        SetCount(itemCount);
        SetTexture(item.iconFile);
    }

    public void SetTexture(string fileName) {
        Texture2D tex = Resources.Load<Texture2D>("Art/Icons/Free-Fruit-Vector-Icon-Pack-for-RPG/PNG/without_shadow/" + fileName);
        imageComponent.texture = tex;
    }

    public void SetCount(int itemCount) {
        count = itemCount;
        textComponent.text = itemCount.ToString();
    }

    public void Click() {
        SetCount(count - 1);
        if(count == 0) {
            conMed.RemoveButton(butIndex);
        }
    }
}
