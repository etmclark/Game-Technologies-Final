using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MenuType {
    PLAYER_INVENTORY,
    MERCHANT,
    LOOT_CRATE,
    SETTINGS, 
    NONE
}

public enum ItemActions {
    DISCARD,
    CONSUME,
    SELL,
    BUY,
    DEPOSIT,
    WITHDRAW
}