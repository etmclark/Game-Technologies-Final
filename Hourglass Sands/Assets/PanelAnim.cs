using System.Collections;
using UnityEngine;
public class PanelAnim : MonoBehaviour
{
    public AnimationCurve showCurve;
    public AnimationCurve hideCurve;
    public float animationSpeed;
    public GameObject panel;
    public Camera playerCamera; 
    public float interactRange = 5f; 
    private GameObject lookingAtMerchant = null; 

    private bool isPanelVisible = false;

    public AnimationCurve Show;
    public AnimationCurve Hide;
    public float AnimationSpeed;
    public GameObject Panel_Trading;
    public GameObject Panel_Inventory;

    private bool isAnimatingTrading = false;
    private bool isShowingTrading = false;
    private bool isAnimatingInventory = false;
    private bool isShowingInventory = false;

    public void Update()
    {
        if (Input.GetKeyUp(KeyCode.M) && !isAnimatingTrading && !isShowingTrading)
        {
            StartCoroutine(ShowPanel_Trading());
        }

        if (Input.GetKeyUp(KeyCode.I) && !isAnimatingInventory && !isShowingInventory)
        {
            StartCoroutine(ShowPanel_Inventory());
        }

        if (Input.GetKeyUp(KeyCode.Escape))
        {
            if (isShowingTrading && !isAnimatingTrading)
            {
                StartCoroutine(HidePanel_Trading());
            }
            else if (isShowingInventory && !isAnimatingInventory)
            {
                StartCoroutine(HidePanel_Inventory());
            }
        }
    }

    IEnumerator ShowPanel_Trading()
    {
        if (isShowingInventory && !isAnimatingInventory)
        {
            yield return StartCoroutine(HidePanel_Inventory());
        }

        FreezeGame();

        isAnimatingTrading = true;
        float timer = 0;
        Panel_Trading.SetActive(true);
        while (timer <= 1)
        {
            Panel_Trading.transform.localScale = Vector3.one * Show.Evaluate(timer);
            timer += Time.unscaledDeltaTime * AnimationSpeed;
            yield return null;
        }
        isAnimatingTrading = false;
        isShowingTrading = true;
    }

    IEnumerator HidePanel_Trading()
    {
        isAnimatingTrading = true;
        float timer = 0;
        while (timer <= 1)
        {
            Panel_Trading.transform.localScale = Vector3.one * Hide.Evaluate(timer);
            timer += Time.unscaledDeltaTime * AnimationSpeed;
            yield return null;
        }
        Panel_Trading.SetActive(false);
        isAnimatingTrading = false;
        isShowingTrading = false;

        UnfreezeGame();
    }

    IEnumerator ShowPanel_Inventory()
    {
        if (isShowingTrading && !isAnimatingTrading)
        {
            yield return StartCoroutine(HidePanel_Trading());
        }

        FreezeGame();

        isAnimatingInventory = true;
        float timer = 0;
        Panel_Inventory.SetActive(true);
        while (timer <= 1)
        {
            Panel_Inventory.transform.localScale = Vector3.one * Show.Evaluate(timer);
            timer += Time.unscaledDeltaTime * AnimationSpeed;
            yield return null;
        }
        isAnimatingInventory = false;
        isShowingInventory = true;
    }

    IEnumerator HidePanel_Inventory()
    {
        isAnimatingInventory = true;
        float timer = 0;
        while (timer <= 1)
        {
            Panel_Inventory.transform.localScale = Vector3.one * Hide.Evaluate(timer);
            timer += Time.unscaledDeltaTime * AnimationSpeed;
            yield return null;
        }
        Panel_Inventory.SetActive(false);
        isAnimatingInventory = false;
        isShowingInventory = false;

        UnfreezeGame();
    }

    IEnumerator ShowPanel(GameObject gameObject)
    {
        float timer = 0;
        while (timer <= 1)
        {
            gameObject.transform.localScale = Vector3.one * showCurve.Evaluate(timer);
            timer += Time.unscaledDeltaTime * animationSpeed;
            yield return null;
        }
        isPanelVisible = true;
    }

    IEnumerator HidePanel(GameObject gameObject)
    {
        float timer = 0;
        while (timer <= 1)
        {
            gameObject.transform.localScale = Vector3.one * hideCurve.Evaluate(timer);
            timer += Time.unscaledDeltaTime * animationSpeed;
            yield return null;
        }
        isPanelVisible = false;
    }

    void FreezeGame()
    {
        Time.timeScale = 0f;
    }

    void UnfreezeGame()
    {
        Time.timeScale = 1f;
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, interactRange))
        {
            if (hit.collider.CompareTag("Merchant")) 
            {
                lookingAtMerchant = hit.collider.gameObject; 
            }
            else
            {
                lookingAtMerchant = null;
            }
        }
        else
        {
            lookingAtMerchant = null;
        }

     
        if (lookingAtMerchant != null && Input.GetKeyDown(KeyCode.M) && !isPanelVisible)
        {
            StartCoroutine(ShowPanel(panel));
            Time.timeScale = 0; 
        }
        else if (Input.GetKeyDown(KeyCode.Escape) && isPanelVisible)
        {
            StartCoroutine(HidePanel(panel));
            Time.timeScale = 1; 
        }
    }
}
