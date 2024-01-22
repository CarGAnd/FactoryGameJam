using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private InputManager inputManager; // Handles user input
    [SerializeField] private CameraSystem cameraSystem;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraMoveDir = inputManager.GetMouseMovement();
        cameraSystem.UpdateCameraPosition(cameraMoveDir * Time.deltaTime);

        float zoomDelta = inputManager.GetZoomInput();
        cameraSystem.UpdateCameraZoom(zoomDelta * Time.deltaTime);
    }
}
