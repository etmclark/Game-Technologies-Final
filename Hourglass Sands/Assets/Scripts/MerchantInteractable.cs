using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantInteractable : MonoBehaviour, IInteractable
{
    public InventoryComponent mInventory;
    public string firstName = "Merchant";
    public Town location;
    private PanelAnim menuControls;
    readonly private string toolTip = "[E] Trade with ";
    public string ToolTip {
        get { return toolTip + firstName; }
    }

    public static string TownToName(Town townEnum) {
        switch(townEnum) {
            case Town.Oumnia:
                return "Oumnia";
            case Town.Lygash:
                return "Lygash";
            case Town.Kybeck:
                return "Kybeck Laesh";
            case Town.Vorbeck:
                return "Vorbek Laesh";
            case Town.Noor:
                return "Noor Vaesh";
        }
        return "";
    }

    public string returnName() {
        return TownToName(location);
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
