using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentMediator : MonoBehaviour
{
    // Start is called before the first frame update
    public int minRows;
    public int rowSize;
    public GameObject contentPanel;
    public GameObject contentContainerPrefab;
    public List<GameObject> contentList = new();
    public GameObject buttonPrefab;
    public List<GameObject> buttons = new();
    public List<ItemAction> availableActions = new();
    private ItemPoolReader itemReader;
    [NonSerialized] public bool sleepy = false;
    void Start()
    {
        itemReader = FindObjectOfType<ItemPoolReader>();
        for (int i = 0; i < minRows; i++) {
            this.AddRow();
        }
        sleepy = true;
    }

    public List<GameObject> LoadFromInventory(InventoryComponent inventory) {
        foreach (InventoryItem item in inventory.itemInventory) {
            AddButton(item.id, item.amount, inventory);
        }
        return buttons;
    }

    void RefreshButtonParents() {
        for (int i = 0; i < buttons.Count; i++) {
            buttons[i].GetComponent<ItemButton>().butIndex = i;
            buttons[i].transform.SetParent(contentList[i].transform, false);
        }
    }

    void AddButton(int itemID, int itemCount, InventoryComponent usingInventory) {
        if (itemCount > 0) {
            if (itemID > contentList.Count) {
                this.AddRow();
            }
            GameObject newButton = Instantiate(buttonPrefab);
            //I get to name it buttscript, as a treat
            ItemButton buttScript = newButton.GetComponent<ItemButton>();
            int index = buttons.Count;
            buttScript.butIndex = index;
            buttScript.conMed = this;
            buttScript.availableActions = this.availableActions;
            buttScript.LoadItem(itemReader.itemPool.items[itemID], itemCount, usingInventory);
            buttons.Add(newButton);
            newButton.transform.SetParent(contentList[index].transform, false);
        }
    }

    public void RemoveButton(int buttonIndex) {
        Destroy(buttons[buttonIndex]);
        buttons.RemoveAt(buttonIndex);
        RefreshButtonParents();
    }

    public void ClearButtons() {
        for(int i = 0; i < buttons.Count; i++) {
            Destroy(buttons[i]);
        }
        buttons.Clear();
    }

    void AddRow() {
        if (contentPanel != null) {
            for(int i = 0; i < rowSize; i++) {
                GameObject newChild = Instantiate(contentContainerPrefab);
                newChild.transform.SetParent(contentPanel.transform, false);
                contentList.Add(newChild);
            }
        }
    }
}
