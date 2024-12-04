///The code for using coroutine to manipulate the animation curve is a tutorial I found on the Internet: https://www.bilibili.com/video/BV11Y4y1a7hV/?spm_id_from=333.337.search-card.all.click&vd_source=5a7c3e147f0b6dc323e06605e69008fb

using System.Collections;
using UnityEngine;

public class PanelAnim : MonoBehaviour
{
    public AnimationCurve showCurve;
    public AnimationCurve hideCurve;
    public float animationSpeed;
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


    // void Update()
    // {
    //     Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * interactRange);
    //     if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, interactRange)) {
    //         if (hit.collider.CompareTag("Merchant"))  {
    //             lookingAtMerchant = hit.collider.gameObject; 
    //         } else {
    //             lookingAtMerchant = null;
    //         }
    //     } else {
    //         lookingAtMerchant = null;
    //     }
     
    //     if (Input.GetKeyDown(KeyCode.Escape) && isPanelVisible) {
    //         StartCoroutine(HidePanel(openPanel));
    //         Time.timeScale = 1; 
    //     } else if (lookingAtMerchant != null && Input.GetKeyDown(KeyCode.M) && !isPanelVisible) {
    //         StartCoroutine(ShowPanel(merchantPanel));
    //         Time.timeScale = 0; 
    //         openPanel = merchantPanel;
    //     } else if(Input.GetKeyDown(KeyCode.I) && !isPanelVisible) {
    //         StartCoroutine(ShowPanel(inventoryPanel));
    //         Time.timeScale = 0; 
    //         openPanel = inventoryPanel;
    //     }
    // }

    IEnumerator ShowPanel(GameObject gameObject)
    {
        float timer = 0;
        animating = true;
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
        finishClose();
        animating = false;
    }

    public void openInventory(InventoryComponent pInventory) {
        Debug.Log("animating: " + animating.ToString());
        Debug.Log("openPanel == null: " + (openPanel == null).ToString());
        if(!animating && openPanel == null) {
            pInventoryPanel.SetActive(true);
            pInventoryPanel.GetComponent<ContentMediator>().LoadFromInventory(pInventory);
            StartCoroutine(ShowPanel(pInventoryPanel));
            Time.timeScale = 0; 
            openPanel = pInventoryPanel;
            openType = MenuType.PLAYER_INVENTORY;
        }
    }

    public void openMerchantMenu(InventoryComponent pInventory, InventoryComponent mInventory) {
        if(!animating && openPanel == null) {
            StartCoroutine(ShowPanel(merchantPanel));
            Time.timeScale = 0; 
            openPanel = merchantPanel;
            openType = MenuType.MERCHANT;
        }
    }

    public void openSettings() {
        //Unimplemented
    }

    void closeMenu() {
        StartCoroutine(HidePanel(openPanel));
    }

    void finishClose() {
        switch (openType) {
            case MenuType.MERCHANT:
                break;
            case MenuType.PLAYER_INVENTORY:
                pInventoryPanel.GetComponent<ContentMediator>().ClearButtons();
                openPanel.SetActive(false);
                break;
            case MenuType.LOOT_CRATE:
                break;
            default:
                break;
        }
        openPanel = null;
        openType = MenuType.NONE;
    }

    public void startMenu() {
        if(!animating) {
            if(openPanel == null) {
                openSettings();
            } else {
                closeMenu();
            }
        }
    }
}
