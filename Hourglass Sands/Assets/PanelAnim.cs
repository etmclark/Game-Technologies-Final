using System.Collections;
using UnityEngine;

public class Anime : MonoBehaviour
{
    public AnimationCurve Show;
    public AnimationCurve Hide;
    public float AnimationSpeed;
    public GameObject Panel_Trading;
    public GameObject Panel_Inventory;

    private bool isAnimatingTrading = false;
    private bool isShowingTrading = false;
    private bool isAnimatingInventory = false;
    private bool isShowingInventory = false;

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

    private void Update()
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

    void FreezeGame()
    {
        Time.timeScale = 0f;
    }

    void UnfreezeGame()
    {
        Time.timeScale = 1f;
    }
}
