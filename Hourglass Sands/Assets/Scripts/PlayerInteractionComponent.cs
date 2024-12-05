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

    void Update() {
        if(Input.GetKeyDown(KeyCode.I)) {
            Debug.Log("pressed");
            OI();
        }
        if(Input.GetKeyDown(KeyCode.Escape)) {
            SM();
        }
        if(Input.GetKeyDown(KeyCode.E)) {
            IN();
        }
        if(Input.GetMouseButtonDown(0)) {
            CL();
        }
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
            OI();
        }
    }

    public void StartMenu(InputAction.CallbackContext ctx) {
        if(ctx.performed) {
            SM();
        }
    }

    public void Interact(InputAction.CallbackContext ctx) {
        if(ctx.performed) {
            IN();
        }
    }

    public void Click(InputAction.CallbackContext ctx) {
        if(ctx.performed) {
            CL();
        }
    }

    public void OI() {
        menuController.OpenInventory(playerInventory);
    }

    public void SM() {
        menuController.StartMenu();
    }

    public void IN() {
        lookAtInteractable?.OnInteract(this);
    }

    public void CL() {
        menuController.OnMouseClick();
    }
}
