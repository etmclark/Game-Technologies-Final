using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CacheInteractable : MonoBehaviour, IInteractable
{
    public InventoryComponent cInventory;
    private PanelAnim menuControls;
    readonly private string toolTip = "[E] Open";
    public string ToolTip {
        get { return toolTip;}
    }

    public void OnInteract(PlayerInteractionComponent interactionComponent)
    {
        menuControls.OpenCacheMenu(interactionComponent.GetComponent<PlayerInventory>(), cInventory);
    }

    // Start is called before the first frame update
    void Start()
    {
        menuControls = FindObjectOfType<PanelAnim>();
    }
}
