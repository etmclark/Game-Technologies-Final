using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitch : MonoBehaviour
{
    // Start is called before the first frame update

    public Camera mainCamera;
    public Camera playerCamera;

    private bool isCameraActive = true;
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            isCameraActive = !isCameraActive;

            mainCamera.gameObject.SetActive(isCameraActive);
            playerCamera.gameObject.SetActive(!isCameraActive);


        }
    }
}
