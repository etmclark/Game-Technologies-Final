///The code for using coroutine to manipulate the animation curve is a tutorial I found on the Internet: https://www.bilibili.com/video/BV11Y4y1a7hV/?spm_id_from=333.337.search-card.all.click&vd_source=5a7c3e147f0b6dc323e06605e69008fb
//used for the basic Pop-up animation
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PanelAnim : MonoBehaviour
{
    public AnimationCurve showCurve;//easing effect for showing and hiding the panel
    public AnimationCurve hideCurve;
    public float animationSpeed;//  Speed for the animations
    public GameObject toolTipObject;//panels
    public GameObject pInventoryPanel;
    public GameObject merchantPanel;
    public GameObject mInventoryPanel;
    public GameObject mShopPanel;
    public GameObject lootPanel;
    public GameObject lInventoryPanel;
    public GameObject lCachePanel;
    public GameObject popupWindow;
    private ItemButton popupHovering;
    public GameObject actionsWindow;
    private ItemButton actionsClicked;
    private PlayerInventory playerInventory;
    [NonSerialized] public InventoryComponent inventoryInteracting;
    private float popupMargin = 8;
    private bool animating = false;
    private GameObject openPanel = null;
    private MenuType openType = MenuType.NONE;
    private bool tooltipEnabled = true;
    private ItemPoolReader itemReader;
    private bool popupSuppressed = false;
    private bool popupHovered = false;
    private GameObject buyPanel;
    private GameObject sellPanel;
    private GameObject withdrawPanel;
    private GameObject depositPanel;
    private GameObject consumePanel;
    private GameObject discardPanel;


    void Start() {
        HideTooltip();
        itemReader = FindObjectOfType<ItemPoolReader>();
        popupWindow.SetActive(false);
        actionsWindow.SetActive(false);
        playerInventory = FindObjectOfType<PlayerInventory>();
        buyPanel = actionsWindow.transform.Find("Buy").gameObject;
        sellPanel = actionsWindow.transform.Find("Sell").gameObject;
        withdrawPanel = actionsWindow.transform.Find("Withdraw").gameObject;
        depositPanel = actionsWindow.transform.Find("Deposit").gameObject;
        consumePanel = actionsWindow.transform.Find("Consume").gameObject;
        discardPanel = actionsWindow.transform.Find("Discard").gameObject;
    }

    IEnumerator ShowPanel(GameObject gameObject)
    {
        float timer = 0; // set the timer
        animating = true;//flag that animation is in progress
        if (tooltipEnabled) {
            HideTooltip(false);
        }
        // Gradually scale the panel until the animation finishs.
        while (timer <= 1) {
            // Adjust the panel's scale with the showcurve
            gameObject.transform.localScale = Vector3.one * showCurve.Evaluate(timer);
            timer += Time.unscaledDeltaTime * animationSpeed;
            yield return null;
        }
        animating = false;
        // Mark the animation done.
    }

    IEnumerator HidePanel(GameObject gameObject)
    {
        float timer = 0;// set the timer
        animating = true;//flag the anim is working 
        while (timer <= 1) {
            //sclae with the curve
            gameObject.transform.localScale = Vector3.one * hideCurve.Evaluate(timer);
            timer += Time.unscaledDeltaTime * animationSpeed;
            yield return null;
        }
        FinishClose();
        animating = false;
        // Mark the animation done.
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
            ib.CallActions = DisplayActions;
            ib.DismissActions = StowActions;
        }
    }

    public void RefreshInventory(InventoryComponent inventoryComp, GameObject inventoryPanel) {
        ContentMediator cm = inventoryPanel.GetComponent<ContentMediator>();
        int buttIndex = actionsClicked.butIndex;
        cm.ClearButtons();
        List<GameObject> buttons = cm.LoadFromInventory(inventoryComp);
        AssignPopupDelegates(buttons);
        if(actionsWindow.activeSelf) {
            actionsClicked = buttons[buttIndex].GetComponent<ItemButton>();
        }
    }

    public void CloseMatchingActions(GameObject inventoryPanel, int i) {
        if(actionsClicked.transform.IsChildOf(inventoryPanel.transform) && actionsClicked.butIndex == i) {
            StowActions(actionsClicked);
        }
    }

    public void RefreshBuy() {
        if(buyPanel.activeSelf) {
            GoodsItem itemFeatures = itemReader.itemPool.items[actionsClicked.item.id];
            if(itemFeatures.basePrice < playerInventory.currency) {
                buyPanel.GetComponent<Button>().enabled = false;
            }
        }
    }

    public void OpenInventory(PlayerInventory pInventory) {
        if(!animating && openPanel == null) {
            playerInventory = pInventory;
            pInventoryPanel.SetActive(true);
            pInventory.e_InventoryUpdated.AddListener(() => {RefreshInventory(pInventory, pInventoryPanel);});
            pInventory.e_ItemRemoved.AddListener((int i) => CloseMatchingActions(pInventoryPanel, i));
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

    public void OpenMerchantMenu(PlayerInventory pInventory, InventoryComponent mInventory) {
        if(!animating && openPanel == null) {
            playerInventory = pInventory;
            inventoryInteracting = mInventory;
            merchantPanel.SetActive(true);
            StartCoroutine(ShowPanel(merchantPanel));
            Time.timeScale = 0; 
            openPanel = merchantPanel;
            openType = MenuType.MERCHANT;
        } else if (!animating && openType == MenuType.MERCHANT) {
            CloseMenu();
        }
    }

    public void OpenCacheMenu(PlayerInventory pInventory, InventoryComponent cInventory) {
        if(!animating && openPanel == null) {
            playerInventory = pInventory;
            inventoryInteracting = cInventory;
            merchantPanel.SetActive(true);
            StartCoroutine(ShowPanel(lootPanel));
            Time.timeScale = 0; 
            openPanel = merchantPanel;
            openType = MenuType.LOOT_CRATE;
        } else if (!animating && openType == MenuType.LOOT_CRATE) {
            CloseMenu();
        }
    }

    public void OpenSettings() {
        //Unimplemented
    }

    void CloseMenu() {
        StartCoroutine(HidePanel(openPanel));
        StowActions(actionsClicked);
        StowPopup(popupHovering);
    }

    void FinishClose() {
        if(tooltipEnabled) {
            ShowTooltip(false);
        }
        switch (openType) {
            case MenuType.MERCHANT:
                Time.timeScale = 1;
                playerInventory.e_InventoryUpdated.RemoveAllListeners();
                playerInventory.e_ItemRemoved.RemoveAllListeners();
                break;
            case MenuType.PLAYER_INVENTORY:
                Time.timeScale = 1;
                pInventoryPanel.GetComponent<ContentMediator>().ClearButtons();
                openPanel.SetActive(false);
                playerInventory.e_InventoryUpdated.RemoveAllListeners();
                playerInventory.e_ItemRemoved.RemoveAllListeners();
                break;
            case MenuType.LOOT_CRATE:
                playerInventory.e_InventoryUpdated.RemoveAllListeners();
                playerInventory.e_ItemRemoved.RemoveAllListeners();
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
        if(!popupSuppressed) {
            popupWindow.SetActive(true);
        }
        popupHovered = true;
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
        if(itemFeatures.thirstRegen != 0) {
            thirstPanel.SetActive(true);
            thirstPanel.GetComponent<TMP_Text> ().text = "Thirst: " + itemFeatures.thirstRegen.ToString();
        } else {
            thirstPanel.SetActive(false);
        }
        weightPanel.GetComponent<TMP_Text>().text = "Weight: " + itemFeatures.weight.ToString();
        valuePanel.GetComponent<TMP_Text>().text = "Value: " + itemFeatures.basePrice.ToString();

        //Align Window
        if(rT != null) {
            RectTransform pT = popupWindow.GetComponent<RectTransform>();
            AlignWindow(pT, rT);
        }
    }

    public void DisplayPopup() {
        if(!popupSuppressed) {
            popupWindow.SetActive(true);
        }
    }

    public void StowPopup(ItemButton itemButton) {
        if(itemButton == popupHovering) {
            popupHovered = false;
            popupWindow.SetActive(false);
        }
    }

    public void DisplayActions(ItemButton itemButton, RectTransform rT) {
        popupSuppressed = true;
        StowPopup(popupHovering);

        actionsWindow.SetActive(true);
        actionsClicked = itemButton;
        GoodsItem itemFeatures = itemReader.itemPool.items[itemButton.item.id];

        foreach(Transform child in actionsWindow.transform) {
            child.gameObject.SetActive(false);
        }
        foreach(ItemAction IA in itemButton.availableActions) {
            switch(IA) {
                case ItemAction.BUY:
                    buyPanel.SetActive(true);
                    playerInventory.e_CurrencyUpdated.AddListener(RefreshBuy);
                    break;
                case ItemAction.SELL:
                    sellPanel.SetActive(true);
                    break;
                case ItemAction.WITHDRAW:
                    withdrawPanel.SetActive(true);
                    break;
                case ItemAction.DEPOSIT:
                    depositPanel.SetActive(true);
                    break;
                case ItemAction.CONSUME:
                    if(itemFeatures.thirstRegen != 0) {
                        consumePanel.SetActive(true);
                    }
                    break;
                case ItemAction.DISCARD:
                    discardPanel.SetActive(true);
                    break;
            }
        }

        RectTransform aT = actionsWindow.GetComponent<RectTransform>();
        AlignWindow(aT, rT);
    }

    public void StowActions(ItemButton itemButton) {
        if(itemButton == actionsClicked) {
            playerInventory.e_CurrencyUpdated.RemoveListener(RefreshBuy);
            if(popupSuppressed) {
            popupSuppressed = false;
            if(popupHovered) {
                DisplayPopup();
            }
            }
            actionsWindow.SetActive(false);
        }
    }

    public bool isOrIsChildOf(GameObject child, GameObject parent) {
        return child == parent || child.transform.IsChildOf(parent.transform);
    }

    public bool mouseOnRect(GameObject rectHaver) {
        RectTransform rectTransform = rectHaver.GetComponent<RectTransform>();
        return RectTransformUtility.RectangleContainsScreenPoint(rectTransform, Mouse.current.position.ReadValue());
    }

    public bool mouseOnButtons(List<GameObject> buttons) {
        foreach(GameObject button in buttons) {
            if(mouseOnRect(button)) {
                return true;
            }
        }
        return false;
    }

    public void OnMouseClick() {
        if(actionsWindow.activeSelf) {
            List<GameObject> buttonsToCheck;
            switch(openType) {
                case MenuType.PLAYER_INVENTORY:
                    buttonsToCheck = pInventoryPanel.GetComponent<ContentMediator>().buttons;
                    break;
                case MenuType.MERCHANT:
                    buttonsToCheck = mInventoryPanel.GetComponent<ContentMediator>().buttons;
                    buttonsToCheck.AddRange(mShopPanel.GetComponent<ContentMediator>().buttons);
                    break;
                case MenuType.LOOT_CRATE:
                    buttonsToCheck = lInventoryPanel.GetComponent<ContentMediator>().buttons;
                    buttonsToCheck.AddRange(lCachePanel.GetComponent<ContentMediator>().buttons);
                    break;
                default:
                    buttonsToCheck = new List<GameObject>();
                    break;
            }
            if(!mouseOnRect(actionsWindow) && !mouseOnButtons(buttonsToCheck)) {
                StowActions(actionsClicked);
            }
        }
    }

    void AlignWindow(RectTransform pT, RectTransform rT) {
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

    //Item Actions
    public void Buy() {
        Debug.Log("Buy");
        GoodsItem itemFeatures = itemReader.itemPool.items[actionsClicked.item.id];
        int buttIndex = actionsClicked.butIndex;
        playerInventory.itemInventory[buttIndex].amount -= 1;
        actionsClicked.DecrementAmount();
    }

    public void Sell() {
        Debug.Log("Sell");
        GoodsItem itemFeatures = itemReader.itemPool.items[actionsClicked.item.id];
        int buttIndex = actionsClicked.butIndex;
        playerInventory.itemInventory[buttIndex].amount -= 1;
        actionsClicked.DecrementAmount();
    }

    public void Withdraw() {
        Debug.Log("Withdraw");
        GoodsItem itemFeatures = itemReader.itemPool.items[actionsClicked.item.id];
        int buttIndex = actionsClicked.butIndex;
        inventoryInteracting.itemInventory[buttIndex].amount -= 1;
        actionsClicked.DecrementAmount(); 
    }

    public void Deposit() {
        Debug.Log("Deposit");
        GoodsItem itemFeatures = itemReader.itemPool.items[actionsClicked.item.id];
        int buttIndex = actionsClicked.butIndex;
        playerInventory.itemInventory[buttIndex].amount -= 1;
        actionsClicked.DecrementAmount(); 
    }
    public void Consume() {
        Debug.Log("Consume");
        GoodsItem itemFeatures = itemReader.itemPool.items[actionsClicked.item.id];
        //Get thirst and add thirst regen of item;
        playerInventory.RemoveItem(actionsClicked.item.id, 1);
        //actionsClicked.DecrementAmount();
    }
    public void Discard() {
        GoodsItem itemFeatures = itemReader.itemPool.items[actionsClicked.item.id];
        int buttIndex = actionsClicked.butIndex; 
        Debug.Log("Discard");
        playerInventory.RemoveItem(actionsClicked.item.id, 1);
        // playerInventory.itemInventory[buttIndex].amount -= 1;
        // actionsClicked.DecrementAmount();
    }
}
