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

public enum ItemAction {
    BUY,
    SELL,
    WITHDRAW,
    DEPOSIT,
    CONSUME,
    DISCARD,
    
}

public enum Town {
    Vorbeck,
    Kybeck,
    Noor,
    Oumnia,
    Lygash
}