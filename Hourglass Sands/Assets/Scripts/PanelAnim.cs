///The code for using coroutine to manipulate the animation curve is a tutorial I found on the Internet: https://www.bilibili.com/video/BV11Y4y1a7hV/?spm_id_from=333.337.search-card.all.click&vd_source=5a7c3e147f0b6dc323e06605e69008fb

using System.Collections;
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
    private bool animating = false;
    private GameObject openPanel = null;
    private MenuType openType = MenuType.NONE;
    private bool tooltipEnabled = true;

    void Start() {
        HideTooltip();
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
        Debug.Log("reached");
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
        Debug.Log("reached");
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

    public void OpenInventory(InventoryComponent pInventory) {
        Debug.Log("animating: " + animating.ToString());
        Debug.Log("openPanel == null: " + (openPanel == null).ToString());
        if(!animating && openPanel == null) {
            pInventoryPanel.SetActive(true);
            pInventoryPanel.GetComponent<ContentMediator>().LoadFromInventory(pInventory);
            StartCoroutine(ShowPanel(pInventoryPanel));
            Time.timeScale = 0; 
            openPanel = pInventoryPanel;
            openType = MenuType.PLAYER_INVENTORY;
        } else if (!animating && openType == MenuType.PLAYER_INVENTORY) {
            CloseMenu();
        }
    }

    public void OpenMerchantMenu(InventoryComponent pInventory, InventoryComponent mInventory) {
        Debug.Log("trade?");
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
}
