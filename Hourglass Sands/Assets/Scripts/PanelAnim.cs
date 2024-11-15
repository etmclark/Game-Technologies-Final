using System.Collections;
using UnityEngine;

public class PanelAnim : MonoBehaviour
{
    public AnimationCurve showCurve;
    public AnimationCurve hideCurve;
    public float animationSpeed;
    public GameObject merchantPanel;
    public GameObject inventoryPanel;
    public Camera playerCamera; 
    public float interactRange = 15f; 
    private GameObject lookingAtMerchant = null; 
    private bool isPanelVisible = false;

    private GameObject openPanel;

    void Update()
    {
        Debug.DrawRay(playerCamera.transform.position, playerCamera.transform.forward * interactRange);
        if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, interactRange)) {
            if (hit.collider.CompareTag("Merchant"))  {
                lookingAtMerchant = hit.collider.gameObject; 
            } else {
                lookingAtMerchant = null;
            }
        } else {
            lookingAtMerchant = null;
        }
     
        if (Input.GetKeyDown(KeyCode.Escape) && isPanelVisible) {
            StartCoroutine(HidePanel(openPanel));
            Time.timeScale = 1; 
        } else if (lookingAtMerchant != null && Input.GetKeyDown(KeyCode.M) && !isPanelVisible) {
            StartCoroutine(ShowPanel(merchantPanel));
            Time.timeScale = 0; 
            openPanel = merchantPanel;
        } else if(Input.GetKeyDown(KeyCode.I) && !isPanelVisible) {
            StartCoroutine(ShowPanel(inventoryPanel));
            Time.timeScale = 0; 
            openPanel = inventoryPanel;
        }
    }

    IEnumerator ShowPanel(GameObject gameObject)
    {
        float timer = 0;
        while (timer <= 1) {
            gameObject.transform.localScale = Vector3.one * showCurve.Evaluate(timer);
            timer += Time.unscaledDeltaTime * animationSpeed;
            yield return null;
        }
        isPanelVisible = true;
    }

    IEnumerator HidePanel(GameObject gameObject)
    {
        float timer = 0;
        while (timer <= 1) {
            gameObject.transform.localScale = Vector3.one * hideCurve.Evaluate(timer);
            timer += Time.unscaledDeltaTime * animationSpeed;
            yield return null;
        }
        isPanelVisible = false;
    }
}
