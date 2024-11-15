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

    void Update()
    {
         if (Physics.Raycast(playerCamera.transform.position, playerCamera.transform.forward, out RaycastHit hit, interactRange)) {
            if (hit.collider.CompareTag("Merchant"))  {
                lookingAtMerchant = hit.collider.gameObject; 
            } else {
                lookingAtMerchant = null;
            }
        } else {
            lookingAtMerchant = null;
        }
     
        if (lookingAtMerchant != null && Input.GetKeyDown(KeyCode.M) && !isPanelVisible) {
            StartCoroutine(ShowPanel(panel));
            Time.timeScale = 0; 
        } else if (Input.GetKeyDown(KeyCode.Escape) && isPanelVisible) {
            StartCoroutine(HidePanel(panel));
            Time.timeScale = 1; 
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
