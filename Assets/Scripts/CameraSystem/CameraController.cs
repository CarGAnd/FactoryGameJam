using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private InputManager inputManager; // Handles user input
    [SerializeField] private CameraSystem cameraSystem;

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraMoveDir = (inputManager.GetMouseMovement() + inputManager.GetMovementInput()).normalized;
        cameraSystem.UpdateCameraPosition(cameraMoveDir * Time.deltaTime);

        float zoomDelta = inputManager.GetZoomInput();
        //cameraSystem.UpdateCameraZoom(zoomDelta * Time.deltaTime);
    }
}
