using UnityEngine;
using UnityEngine.SceneManagement;

public class EscapeKey : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
    }
}
