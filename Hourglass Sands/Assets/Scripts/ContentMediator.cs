using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentMediator : MonoBehaviour
{
    // Start is called before the first frame update
    public int minRows = 0;
    public int rowSize = 1;
    public GameObject contentPanel;
    public GameObject contentContainerPrefab;
    public List<GameObject> contentList = new();
    public GameObject buttonPrefab;
    public List<GameObject> buttons = new();
    private ItemPool itemPool;
    void Start()
    {
        itemPool = FindObjectOfType<ItemPoolReader>().itemPool;
        for (int i = 0; i < minRows; i++) {
            this.AddRow();
        }
    }

    

    void RefreshButtonParents() {
        for (int i = 0; i < buttons.Count; i++) {
            buttons[i].transform.parent = contentList[i].transform.parent;
        }
    }

    void AddButton(int itemID) {
        GameObject newButton = Instantiate(buttonPrefab);
        newButton.GetComponent<ItemButton>().LoadItem(itemPool.items[itemID]);
        int index = buttons.Count;
        buttons.Add(newButton);
        newButton.transform.parent = contentList[index].transform;
    }

    void InsertButton(int buttonIndex, int itemID) {
        GameObject newButton = Instantiate(buttonPrefab);
        newButton.GetComponent<ItemButton>().LoadItem(itemPool.items[itemID]);
        buttons.Insert(buttonIndex, newButton);
        RefreshButtonParents();
    }

    void RemoveButton(int buttonIndex) {
        buttons.RemoveAt(buttonIndex);
        RefreshButtonParents();
    }

    void ClearButtons() {
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
