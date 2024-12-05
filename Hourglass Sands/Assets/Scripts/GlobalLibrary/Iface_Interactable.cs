using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable {
    string ToolTip { get; }
    void OnInteract(PlayerInteractionComponent interactionComponent);
}
