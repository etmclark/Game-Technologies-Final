using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DebtInteractable : MonoBehaviour, IInteractable {
    readonly private string toolTip = "[E] Pay off debt: ";
    private float cost = 200;
    public string ToolTip {
        get { return toolTip + cost; }
    }

    public void OnInteract(PlayerInteractionComponent interactionComponent)
    {
        if(interactionComponent.GetComponent<PlayerInventory>().currency > 200) {
            SceneManager.LoadScene("Credits");
        }
    }
}
