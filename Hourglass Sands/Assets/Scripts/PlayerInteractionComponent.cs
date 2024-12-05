using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class PlayerInteractionComponent : MonoBehaviour
{
    public Camera activeCamera;
    private IInteractable lookAtInteractable = null;
    private PanelAnim menuController;
    private PlayerInventory playerInventory;
    [SerializeField] private float lookRange = 18f;
    private float lookAtTimer = 0.1f;
    // Start is called before the first frame update
    void Start()
    {
        playerInventory = GetComponent<PlayerInventory>();
        menuController = FindObjectOfType<PanelAnim>();
        StartCoroutine(LookAt());
    }

    // Update is called once per frame
    IEnumerator LookAt() {
        for(;;) {
            IInteractable oldLookAt = lookAtInteractable;
            if (Physics.Raycast(activeCamera.transform.position, activeCamera.transform.forward, out RaycastHit hit, lookRange)) {
                lookAtInteractable = hit.transform.GetComponent<IInteractable>();
            } else {
                lookAtInteractable = null;
            }
            if(lookAtInteractable != null && oldLookAt == null) {
                menuController.ShowTooltip();
                menuController.SetTooltip(lookAtInteractable.ToolTip);
            } else if(lookAtInteractable == null && oldLookAt != null) {
                menuController.HideTooltip();
            }
            yield return new WaitForSeconds(lookAtTimer);
        }
    }

    public void OpenInventory(InputAction.CallbackContext ctx) {
        if(ctx.performed) {
            menuController.OpenInventory(playerInventory);
        }
    }

    public void StartMenu(InputAction.CallbackContext ctx) {
        if(ctx.performed) {
            menuController.StartMenu();
        }
    }

    public void Interact(InputAction.CallbackContext ctx) {
        if(ctx.performed) {
            lookAtInteractable?.OnInteract(this);
        }
    }

    public void Click(InputAction.CallbackContext ctx) {
        if(ctx.performed) {
            menuController.OnMouseClick();
        }
    }
}
