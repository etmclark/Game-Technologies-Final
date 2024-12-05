using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SleepScript : MonoBehaviour
{
    public List<GameObject> mediators;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitForSleep());
    }

    bool sleepCondition() {
        foreach (GameObject mediator in mediators) {
            if (!mediator.GetComponent<ContentMediator>().sleepy) {
                return false;
            }
        }
        return true;
    }

    // Update is called once per frame
    IEnumerator WaitForSleep() {
        for(;;) {
            if(sleepCondition()) {
                break;
            } yield return null;
        }
        gameObject.SetActive(false);
    }
}
