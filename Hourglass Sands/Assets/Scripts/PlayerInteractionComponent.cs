using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class PlayerInteractionComponent : MonoBehaviour
{
    public Camera pCamera;
    private IInteractable lookAtInteractable = null;
    private PanelAnim menuController;
    private InventoryComponent playerInventory;
    [SerializeField] private float lookRange = 18f;
    private float lookAtTimer = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        playerInventory = GetComponent<InventoryComponent>();
        menuController = FindObjectOfType<PanelAnim>();
        StartCoroutine(LookAt());
    }

    // Update is called once per frame
    IEnumerator LookAt() {
        for(;;) {
            if (Physics.Raycast(pCamera.transform.position, pCamera.transform.forward, out RaycastHit hit, lookRange)) {
                lookAtInteractable = hit.transform.GetComponent<IInteractable>();
            } else {
                lookAtInteractable = null;
            }
            yield return new WaitForSeconds(lookAtTimer);
        }
    }

    public void OpenInventory(InputAction.CallbackContext ctx) {
        if(ctx.performed) {
            menuController.openInventory(playerInventory);
        }
    }

    public void StartMenu(InputAction.CallbackContext ctx) {
        menuController.startMenu();
    }

    public void Interact(InputAction.CallbackContext ctx) {
        lookAtInteractable?.OnInteract();
    }
}
