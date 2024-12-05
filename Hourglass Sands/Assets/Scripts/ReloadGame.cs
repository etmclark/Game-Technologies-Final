using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadSceneButton : MonoBehaviour
{
    public void ReloadScene()
    {
        SceneManager.LoadScene(1);
    }
}