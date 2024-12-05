using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantInteractable : MonoBehaviour, IInteractable
{
    public InventoryComponent mInventory;
    public string firstName = "Merchant";
    private PanelAnim menuControls;
    readonly private string toolTip = "[E] Trade with ";
    public string ToolTip {
        get { return toolTip + firstName; }
    }

    public void OnInteract(PlayerInteractionComponent interactionComponent)
    {
        menuControls.OpenMerchantMenu(interactionComponent.GetComponent<PlayerInventory>(), mInventory);
    }

    // Start is called before the first frame update
    void Start()
    {
        menuControls = FindObjectOfType<PanelAnim>();
    }
}
