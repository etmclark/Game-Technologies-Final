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
    void Start()
    {
        for (int i = 0; i < minRows; i++) {
            this.AddRow();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void AddRow() {
        if (contentPanel != null) {
            for(int i = 0; i < rowSize; i++) {
                GameObject newChild = Instantiate(contentContainerPrefab);
                newChild.transform.SetParent(contentPanel.transform, false);
            }
        }
    }
}
