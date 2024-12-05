using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ScarcityGeneration : MonoBehaviour
{
    // Start is called before the first frame update
    Dictionary<string, Dictionary<int, float>> scarcityMap = new();
    ItemPoolReader itemPoolReader;
    public TextAsset POIFile;
    private SettlementList locationData;

    private float scarcityMaxDeviance = 0.3f;
    void Start()
    {
        itemPoolReader = FindObjectOfType<ItemPoolReader>();
        Assert.IsNotNull(POIFile);
        locationData = JsonUtility.FromJson<SettlementList>(POIFile.text);

        if(itemPoolReader.itemPool != null) {
            Debug.Log("not null");
            GenerateScarcity();
        } else {
            Debug.Log("null");
            StartCoroutine(WaitForItemPool());
        }
    }

    void GenerateScarcity() {
        foreach (Settlement settlement in locationData.settlements) {
            Dictionary<int, float> innerDict = new();
            foreach (GoodsItem item in itemPoolReader.itemPool.items) {
                float deviance = Random.Range(-1.0f, 1.0f) * scarcityMaxDeviance;
                innerDict.Add(item.id, 1f + deviance);
                Debug.Log(settlement.name + item.id + " " + (1f + deviance));
            }
            scarcityMap.Add(settlement.name, innerDict);
        }
        startGeneration();
    }

    void startGeneration() {
        StartCoroutine(RegenerationTimer());
        return;
    }

    void generateBoard() {
        Debug.Log("generating");
        GenerativeInventory[] generators = FindObjectsOfType<GenerativeInventory>(); 
        foreach (GenerativeInventory generative in generators) {
            Debug.Log(generative);
            generative.Regenerate(scarcityMap);
        }
    }

    IEnumerator WaitForItemPool() {
        for(;;) {
            if(itemPoolReader.itemPool != null) {
                break;
            }
            yield return new WaitForSeconds(.1f);
        }
        GenerateScarcity();
    }

    IEnumerator RegenerationTimer() {
        for(;;) {
            generateBoard();
            yield return new WaitForSeconds(120f);
        }
    }
}
