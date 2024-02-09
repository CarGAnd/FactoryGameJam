using UnityEngine;
using Cinemachine;

public class CameraSystem : MonoBehaviour
{
    public CinemachineVirtualCamera virtualCamera; // Manages camera's position and lens
    public CinemachineConfiner confiner; // Restricts camera's movement within bounds
    [SerializeField] private ZoomSettings zoomSettings;
    [SerializeField] private MovementSettings movementSettings;

    public void UpdateCameraPosition(Vector3 delta)
    {
        // Use inputManager to get movement input and apply it to the camera.
        // Leverage Cinemachine's smooth movement capabilities for isometric movement (Design Doc: Camera Movement).
        virtualCamera.transform.position += (Quaternion.Euler(virtualCamera.transform.root.eulerAngles.y * Vector3.up) * delta) * movementSettings.moveSpeed;
        // Implementation to move the virtualCamera using Cinemachine
    }

    public void UpdateCameraZoom(float delta)
    {
        // Use inputManager to get zoom input and apply it to the camera's FOV or Orthographic Size.
        // Adjust zoom within limits defined in zoomSettings (Design Doc: Camera Zoom).
        if (virtualCamera.m_Lens.Orthographic) {
            virtualCamera.m_Lens.OrthographicSize -= delta * zoomSettings.zoomSpeed;
            if(virtualCamera.m_Lens.OrthographicSize > zoomSettings.maxZoom) {
                virtualCamera.m_Lens.OrthographicSize = zoomSettings.maxZoom;
            }
            if (virtualCamera.m_Lens.OrthographicSize < zoomSettings.minZoom) {
                virtualCamera.m_Lens.OrthographicSize = zoomSettings.minZoom;
            }
        }
        else {
            virtualCamera.transform.position += virtualCamera.transform.forward * delta * zoomSettings.zoomSpeed;
        }
    }

    public void ApplyCameraBounds()
    {
        // Use confiner to apply bounds to the camera (Design Doc: Camera Bounds).
        // The confiner's bounding volume can be set to a Collider representing the bounds.
        // Implementation to apply bounds using Cinemachine
    }

    
}

[System.Serializable]
public class ZoomSettings
{
    public float minZoom;
    public float maxZoom;
    public float zoomSpeed;
}

[System.Serializable]
public class MovementSettings
{
    public float moveSpeed;
    public AnimationCurve dampeningCurve;
    public bool isometricMovement; // If true, movement will be relative to the camera's orientation
}
