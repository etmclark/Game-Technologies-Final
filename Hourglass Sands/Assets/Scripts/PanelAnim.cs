///The code for using coroutine to manipulate the animation curve is a tutorial I found on the Internet: https://www.bilibili.com/video/BV11Y4y1a7hV/?spm_id_from=333.337.search-card.all.click&vd_source=5a7c3e147f0b6dc323e06605e69008fb

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    private GameObject weightPanel = null;
    private GameObject moneyPanel = null;
    private ScarcityGeneration scarcityGen = null;
    private MerchantInteractable mI;
    private BarGradient thirstBar;
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
        scarcityGen = FindObjectOfType<ScarcityGeneration>();
        thirstBar = FindObjectOfType<BarGradient>();
    }

    IEnumerator ShowPanel(GameObject gameObject)
    {
        float timer = 0;
        animating = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
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
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
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
            ib.CallActions = DisplayActions;
            ib.DismissActions = StowActions;
        }
    }

    public void setWeightAndMoney(GameObject pInvPanel) {
        Transform dataPanel = pInvPanel.transform.Find("PlayerData");
        weightPanel = dataPanel.Find("Weight").gameObject;
        moneyPanel = dataPanel.Find("Money").gameObject;
    }

    public void updateWeight() {
        if(weightPanel != null) {
            float weight = itemReader.itemPool.ComputeWeight(playerInventory.itemInventory.ToArray());
            weightPanel.GetComponent<TMP_Text>().text = "Weight: " + weight.ToString("0.00") + "/" + "50";
        }
    }

    public void updateMoney() {
        if(moneyPanel != null) {
            moneyPanel.GetComponent<TMP_Text>().text = "Gold: " + playerInventory.currency.ToString("0.00");
        }
    }

    public void RefreshInventory(InventoryComponent inventoryComp, GameObject inventoryPanel) {
        ContentMediator cm = inventoryPanel.GetComponent<ContentMediator>();
        int buttIndex = actionsClicked.butIndex;
        cm.ClearButtons();
        List<GameObject> buttons = cm.LoadFromInventory(inventoryComp);
        AssignPopupDelegates(buttons);
        if(actionsWindow.activeSelf && actionsClicked.transform.IsChildOf(inventoryPanel.transform)) {
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
            if(modifyPrice(itemFeatures.basePrice) > playerInventory.currency) {
                buyPanel.GetComponent<Button>().enabled = false;
            }
        }
    }

    public void OpenInventory(PlayerInventory pInventory) {
        if(!animating && openPanel == null) {
            playerInventory = pInventory;
            setWeightAndMoney(pInventoryPanel);
            updateWeight();
            updateMoney();
            pInventoryPanel.SetActive(true);
            pInventory.e_InventoryUpdated.AddListener(() => {RefreshInventory(pInventory, pInventoryPanel);});
            pInventory.e_InventoryUpdated.AddListener(updateWeight);
            pInventory.e_CurrencyUpdated.AddListener(updateMoney);
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
            setWeightAndMoney(mInventoryPanel);
            updateWeight();
            updateMoney();
            merchantPanel.SetActive(true);
            mInventoryPanel.SetActive(true);
            //player side
            pInventory.e_InventoryUpdated.AddListener(() => {RefreshInventory(pInventory, mInventoryPanel);});
            pInventory.e_InventoryUpdated.AddListener(updateWeight);
            pInventory.e_CurrencyUpdated.AddListener(updateMoney);
            pInventory.e_ItemRemoved.AddListener((int i) => CloseMatchingActions(mInventoryPanel, i));
            //merchant side
            mInventory.e_InventoryUpdated.AddListener(() => {RefreshInventory(mInventory, mShopPanel);});
            mInventory.e_ItemRemoved.AddListener((int i) => CloseMatchingActions(mShopPanel, i));
            List<GameObject> buttons = mInventoryPanel.GetComponent<ContentMediator>().LoadFromInventory(pInventory);
            buttons.AddRange(mShopPanel.GetComponent<ContentMediator>().LoadFromInventory(mInventory));
            AssignPopupDelegates(buttons);
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
            setWeightAndMoney(lInventoryPanel);
            updateWeight();
            updateMoney();
            merchantPanel.SetActive(true);
            mInventoryPanel.SetActive(true);
            //player side
            pInventory.e_InventoryUpdated.AddListener(() => {RefreshInventory(pInventory, mInventoryPanel);});
            pInventory.e_InventoryUpdated.AddListener(updateWeight);
            pInventory.e_CurrencyUpdated.AddListener(updateMoney);
            pInventory.e_ItemRemoved.AddListener((int i) => CloseMatchingActions(lInventoryPanel, i));
            //crate side
            cInventory.e_InventoryUpdated.AddListener(() => {RefreshInventory(cInventory, lCachePanel);});
            cInventory.e_ItemRemoved.AddListener((int i) => CloseMatchingActions(lCachePanel, i));
            List<GameObject> buttons = lInventoryPanel.GetComponent<ContentMediator>().LoadFromInventory(pInventory);
            buttons.AddRange(lCachePanel.GetComponent<ContentMediator>().LoadFromInventory(cInventory));
            AssignPopupDelegates(buttons);
            StartCoroutine(ShowPanel(lootPanel));
            Time.timeScale = 0; 
            openPanel = lootPanel;
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
                mInventoryPanel.GetComponent<ContentMediator>().ClearButtons();
                mShopPanel.GetComponent<ContentMediator>().ClearButtons();
                openPanel.SetActive(false);
                playerInventory.e_InventoryUpdated.RemoveAllListeners();
                playerInventory.e_ItemRemoved.RemoveAllListeners();
                playerInventory.e_CurrencyUpdated.RemoveAllListeners();
                inventoryInteracting.e_InventoryUpdated.RemoveAllListeners();
                inventoryInteracting.e_ItemRemoved.RemoveAllListeners();
                break;
            case MenuType.PLAYER_INVENTORY:
                pInventoryPanel.GetComponent<ContentMediator>().ClearButtons();
                openPanel.SetActive(false);
                playerInventory.e_InventoryUpdated.RemoveAllListeners();
                playerInventory.e_ItemRemoved.RemoveAllListeners();
                playerInventory.e_CurrencyUpdated.RemoveAllListeners();
                break;
            case MenuType.LOOT_CRATE:
                lInventoryPanel.GetComponent<ContentMediator>().ClearButtons();
                lCachePanel.GetComponent<ContentMediator>().ClearButtons();
                openPanel.SetActive(false);
                playerInventory.e_InventoryUpdated.RemoveAllListeners();
                playerInventory.e_ItemRemoved.RemoveAllListeners();
                playerInventory.e_CurrencyUpdated.RemoveAllListeners();
                inventoryInteracting.e_InventoryUpdated.RemoveAllListeners();
                inventoryInteracting.e_ItemRemoved.RemoveAllListeners();
                break;
            case MenuType.SETTINGS:
                break;
            default:
                break;
        }
        weightPanel = null;
        moneyPanel = null;
        Time.timeScale = 1;
        openPanel = null;
        openType = MenuType.NONE;
    }

    public void StartMenu() {
        if(!animating) {
            if(openPanel == null) {
                SceneManager.LoadScene(0);
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
        weightPanel.GetComponent<TMP_Text>().text = "Weight: " + itemFeatures.weight.ToString("0.0");
        valuePanel.GetComponent<TMP_Text>().text = "Value: " + modifyPrice(itemFeatures.basePrice).ToString("0.0");

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
        // int buttIndex = actionsClicked.butIndex;
        // playerInventory.itemInventory[buttIndex].amount -= 1;
        // actionsClicked.DecrementAmount();
        inventoryInteracting.RemoveItem(actionsClicked.item.id, 1);
        playerInventory.PurchaseItem(actionsClicked.item.id, 1, modifyPrice(itemFeatures.basePrice));

    }

    public void Sell() {
        Debug.Log("Sell");
        GoodsItem itemFeatures = itemReader.itemPool.items[actionsClicked.item.id];
        // int buttIndex = actionsClicked.butIndex;
        // playerInventory.itemInventory[buttIndex].amount -= 1;
        // actionsClicked.DecrementAmount();
        inventoryInteracting.AddItem(actionsClicked.item.id, 1);
        playerInventory.SellItem(actionsClicked.item.id, 1, modifyPrice(itemFeatures.basePrice));
    }

    public void Withdraw() {
        Debug.Log("Withdraw");
        playerInventory.AddItem(actionsClicked.item.id, 1);
        inventoryInteracting.RemoveItem(actionsClicked.item.id, 1);
    }

    public void Deposit() {
        Debug.Log("Deposit");
        // GoodsItem itemFeatures = itemReader.itemPool.items[actionsClicked.item.id];
        // int buttIndex = actionsClicked.butIndex;
        inventoryInteracting.AddItem(actionsClicked.item.id, 1);
        playerInventory.RemoveItem(actionsClicked.item.id, 1);
        // playerInventory.itemInventory[buttIndex].amount -= 1;
        // actionsClicked.DecrementAmount(); 
    }
    public void Consume() {
        Debug.Log("Consume");
        GoodsItem itemFeatures = itemReader.itemPool.items[actionsClicked.item.id];
        //Get thirst and add thirst regen of item;
        thirstBar.refill(itemFeatures.thirstRegen);
        playerInventory.RemoveItem(actionsClicked.item.id, 1);
        //actionsClicked.DecrementAmount();
    }
    public void Discard() {
        // GoodsItem itemFeatures = itemReader.itemPool.items[actionsClicked.item.id];
        // int buttIndex = actionsClicked.butIndex; 
        Debug.Log("Discard");
        playerInventory.RemoveItem(actionsClicked.item.id, 1);
        // playerInventory.itemInventory[buttIndex].amount -= 1;
        // actionsClicked.DecrementAmount();
    }

    public float modifyPrice(float basePrice) {
        Debug.Log("modified");
        Debug.Log(basePrice + "" + inventoryInteracting + "" + mI);
        if(inventoryInteracting != null) {
            if(mI == null) {
                mI = inventoryInteracting.GetComponent<MerchantInteractable>();
            }
            Debug.Log(basePrice + "" + mI);
            if(mI != null) {
                Debug.Log(GenerativeInventory.TownToName(mI.location) + "" + actionsClicked.item.id);
                float scarcity = scarcityGen.scarcityMap[GenerativeInventory.TownToName(mI.location)][actionsClicked.item.id];
                Debug.Log("scarcity: " + scarcity);
                return basePrice * scarcity;
            }
        }
        return basePrice;
    }
}
