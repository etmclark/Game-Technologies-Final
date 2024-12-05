///The code for using coroutine to manipulate the animation curve is a tutorial I found on the Internet: https://www.bilibili.com/video/BV11Y4y1a7hV/?spm_id_from=333.337.search-card.all.click&vd_source=5a7c3e147f0b6dc323e06605e69008fb

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PanelAnim : MonoBehaviour
{
    public AnimationCurve showCurve;
    public AnimationCurve hideCurve;
    public float animationSpeed;
    public GameObject toolTipObject;
    public GameObject pInventoryPanel;
    public GameObject merchantPanel;
    public GameObject mInventoryPanel;
    public GameObject mShopPanel;
    public GameObject lootPanel;
    public GameObject lInventoryPanel;
    public GameObject lCachePanel;
    public GameObject popupWindow;
    public ItemButton popupHovering;
    private float popupMargin = 8;
    private bool animating = false;
    private GameObject openPanel = null;
    private MenuType openType = MenuType.NONE;
    private bool tooltipEnabled = true;
    private ItemPoolReader itemReader;

    void Start() {
        HideTooltip();
        itemReader = FindObjectOfType<ItemPoolReader>();
        popupWindow.SetActive(false);
    }

    IEnumerator ShowPanel(GameObject gameObject)
    {
        float timer = 0;
        animating = true;
        if(tooltipEnabled) {
            HideTooltip(false);
        }
        while (timer <= 1) {
            gameObject.transform.localScale = Vector3.one * showCurve.Evaluate(timer);
            timer += Time.unscaledDeltaTime * animationSpeed;
            yield return null;
        }
        animating = false;
    }

    IEnumerator HidePanel(GameObject gameObject)
    {
        float timer = 0;
        animating = true;
        while (timer <= 1) {
            gameObject.transform.localScale = Vector3.one * hideCurve.Evaluate(timer);
            timer += Time.unscaledDeltaTime * animationSpeed;
            yield return null;
        }
        FinishClose();
        animating = false;
    }

    public void SetTooltip(string tooltip) {
        toolTipObject.GetComponentInChildren<TMP_Text>(true).text = tooltip;
    }

    public void ShowTooltip(bool enableTooltip = true) {
        toolTipObject.SetActive(true);
        if(enableTooltip) {
            tooltipEnabled = true;
        }
    }

    public void HideTooltip(bool disableTooltip = true) {
        toolTipObject.SetActive(false);
        if(disableTooltip) {
            tooltipEnabled = false;
        }
    }

    public void AssignPopupDelegates(List<GameObject> buttons) {
        foreach (GameObject button in buttons) {
            ItemButton ib = button.GetComponent<ItemButton>();
            ib.CallPopup = DisplayPopup;
            ib.DismissPopup = StowPopup;
        }
    }

    public void OpenInventory(InventoryComponent pInventory) {
        if(!animating && openPanel == null) {
            pInventoryPanel.SetActive(true);
            List<GameObject> buttons = pInventoryPanel.GetComponent<ContentMediator>().LoadFromInventory(pInventory);
            AssignPopupDelegates(buttons);
            StartCoroutine(ShowPanel(pInventoryPanel));
            Time.timeScale = 0; 
            openPanel = pInventoryPanel;
            openType = MenuType.PLAYER_INVENTORY;
        } else if (!animating && openType == MenuType.PLAYER_INVENTORY) {
            CloseMenu();
        }
    }

    public void OpenMerchantMenu(InventoryComponent pInventory, InventoryComponent mInventory) {
        if(!animating && openPanel == null) {
            merchantPanel.SetActive(true);
            StartCoroutine(ShowPanel(merchantPanel));
            Time.timeScale = 0; 
            openPanel = merchantPanel;
            openType = MenuType.MERCHANT;
        } else if (!animating && openType == MenuType.MERCHANT) {
            CloseMenu();
        }
    }

    public void OpenSettings() {
        //Unimplemented
    }

    void CloseMenu() {
        StartCoroutine(HidePanel(openPanel));
    }

    void FinishClose() {
        if(tooltipEnabled) {
            ShowTooltip(false);
        }
        switch (openType) {
            case MenuType.MERCHANT:
                Time.timeScale = 1;
                break;
            case MenuType.PLAYER_INVENTORY:
                Time.timeScale = 1;
                pInventoryPanel.GetComponent<ContentMediator>().ClearButtons();
                openPanel.SetActive(false);
                break;
            case MenuType.LOOT_CRATE:
                Time.timeScale = 1;
                break;
            case MenuType.SETTINGS:
                Time.timeScale = 1;
                break;
            default:
                break;
        }
        openPanel = null;
        openType = MenuType.NONE;
    }

    public void StartMenu() {
        if(!animating) {
            if(openPanel == null) {
                OpenSettings();
            } else {
                CloseMenu();
            }
        }
    }

    public void DisplayPopup(ItemButton itemButton, RectTransform rT) {
        popupWindow.SetActive(true);
        popupHovering = itemButton;

        //Load Items Features
        GoodsItem itemFeatures = itemReader.itemPool.items[itemButton.item.id];
        GameObject namePanel = popupWindow.transform.Find("Name").gameObject;
        GameObject descPanel = popupWindow.transform.Find("Description").gameObject;
        GameObject horizPanel = popupWindow.transform.Find("Horiz").gameObject;
        GameObject thirstPanel = horizPanel.transform.Find("Thirst").gameObject;
        GameObject weightPanel = horizPanel.transform.Find("Weight").gameObject;
        GameObject valuePanel = horizPanel.transform.Find("Value").gameObject;
        namePanel.GetComponent<TMP_Text>().text = itemFeatures.name;
        descPanel.GetComponent<TMP_Text>().text = itemFeatures.description;
        if(itemFeatures.thirstRegen == 0) {
            thirstPanel.SetActive(true);
            thirstPanel.GetComponent<TMP_Text> ().text = "Thirst: " + itemFeatures.thirstRegen.ToString();
        } else {
            thirstPanel.SetActive(false);
        }
        weightPanel.GetComponent<TMP_Text>().text = "Weight: " + itemFeatures.weight.ToString();
        valuePanel.GetComponent<TMP_Text>().text = "Value: " + itemFeatures.basePrice.ToString();

        //Align Window
        RectTransform pT = popupWindow.GetComponent<RectTransform>();
        alignWindow(pT, rT);
    }

    void alignWindow(RectTransform pT, RectTransform rT) {
        float newX;
        if(rT.position.x + rT.anchorMax.x * rT.sizeDelta.x + popupMargin + pT.sizeDelta.x < Screen.width) {
            newX = rT.position.x + rT.anchorMax.x * rT.sizeDelta.x + popupMargin + pT.anchorMin.x * pT.sizeDelta.x;
        } else {
            newX = rT.position.x - rT.anchorMin.x * rT.sizeDelta.x - popupMargin - pT.anchorMax.x * pT.sizeDelta.x;
        }
        //waits until next frame so the panel can size refit first
        StartCoroutine(WaitRT(pT, rT));
        pT.position = new Vector3(newX, rT.position.y + rT.anchorMax.y * rT.sizeDelta.y, rT.position.z);
    }

    IEnumerator WaitRT(RectTransform pT, RectTransform rT) {
        yield return 0;
        float newY;
        if(rT.position.y - rT.anchorMin.y * rT.sizeDelta.y - pT.anchorMin.y * pT.sizeDelta.y > 0) {
            newY = rT.position.y + rT.anchorMax.y * rT.sizeDelta.y;
        } else {
            newY = pT.anchorMin.y * pT.sizeDelta.y;
        }
        pT.position = new Vector3(pT.position.x, newY, pT.position.z);
    }
    
    public void StowPopup(ItemButton itemButton) {
        if(itemButton == popupHovering) {
            popupWindow.SetActive(false);
        }
    }
}
