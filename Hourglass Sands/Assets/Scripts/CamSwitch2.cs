using UnityEngine;
using Cinemachine;

public class CameraSwitcher : MonoBehaviour
{
    public Camera firstPersonCamera; // Assign your first-person camera here
    public CinemachineVirtualCamera virtualCamera; // Assign your Cinemachine virtual camera here

    private bool isUsingFirstPerson = true;

    void Update()
    {
        // Check for input to toggle between cameras (e.g., "C" key)
        if (Input.GetKeyDown(KeyCode.C))
        {
            ToggleCameras(!isUsingFirstPerson);
        }
    }

    void ToggleCameras(bool useFirstPerson)
    {
        isUsingFirstPerson = useFirstPerson;

        // Enable or disable cameras
        firstPersonCamera.enabled = useFirstPerson;
        virtualCamera.enabled = !useFirstPerson;
    }
}