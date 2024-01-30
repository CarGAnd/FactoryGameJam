using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MouseInput : MonoBehaviour {
    
    public UnityEvent<Vector3> MouseOverGridSpace;
    
    [SerializeField] private Camera cam;
    [field: SerializeField] public FactoryGrid BuildGrid { get; private set; }

    public Vector3 LastGroundHitPoint { get; private set; }
    public Vector2Int LastMouseGridPos { get; private set; }

    void Awake() {
        cam = Camera.main;
    }

    void Update() {
        UpdateMousePosition();
    }

    private void UpdateMousePosition() {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        Vector3 worldPosition = BuildGrid.RaycastGridPlane(ray);
        Vector2Int mouseGridPosition = BuildGrid.GetCellCoords(worldPosition);

        LastGroundHitPoint = worldPosition;

        if (mouseGridPosition != LastMouseGridPos) {
            Vector3 gridCellCenter = BuildGrid.GetCellCenter(mouseGridPosition);
            LastMouseGridPos = mouseGridPosition;
            MouseOverGridSpace?.Invoke(gridCellCenter);
        }
    }

    public Vector3 GetMousePosOnGrid(FactoryGrid grid) {
        Ray ray = cam.ScreenPointToRay(Mouse.current.position.ReadValue());
        Vector3 worldPosition = BuildGrid.RaycastGridPlane(ray);
        return worldPosition;
    }

    public bool LeftMouseButtonPressed() {
        return Mouse.current.leftButton.wasPressedThisFrame;
    }

    public bool RightMouseButtonPressed() {
        return Mouse.current.rightButton.wasPressedThisFrame;
    }

    public bool MouseScrolledUp() {
        return Mouse.current.scroll.ReadValue().y > 0.1f;
    }

    public bool MouseScrolledDown() {
        return Mouse.current.scroll.ReadValue().y < -0.1f;
    }
}
