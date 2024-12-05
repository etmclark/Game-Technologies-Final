using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemButton : MonoBehaviour
{
    public ContentMediator conMed;
    public GoodsItem item = null;
    public int count = 0;
    public int butIndex = 0;
    public InventoryComponent inventoryFrom;
    private RawImage imageComponent;
    private TMP_Text textComponent;
    public delegate void HoverDel(ItemButton button, RectTransform rectTransform);
    public delegate void ExitDel(ItemButton button);
    public HoverDel CallPopup;
    public ExitDel DismissPopup;
    private RectTransform myRectTransform;
    // Start is called before the first frame update
    void Awake() {
        imageComponent = gameObject.GetComponentInChildren<RawImage>();
        textComponent = gameObject.GetComponentInChildren<TMP_Text>();
    }

    void Start() {
        myRectTransform = GetComponent<RectTransform>();
    }

    public void LoadItem(GoodsItem item, int itemCount, InventoryComponent inventoryFrom) {
        this.item = item;
        this.inventoryFrom = inventoryFrom;
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
            DismissPopup?.Invoke(this);
            conMed.RemoveButton(butIndex);
        }
    }

    public void OnHover() {
        CallPopup?.Invoke(this, myRectTransform);
    }

    public void OnUnhover() {
        DismissPopup?.Invoke(this);
    }
}
