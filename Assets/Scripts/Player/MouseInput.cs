using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MouseInput : MonoBehaviour {
    
    public UnityEvent<Vector3> MouseOverGridSpace;
    
    [SerializeField] private Camera cam;
    [SerializeField] private PlayerControls playerControls;
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
        Vector3 worldPosition = grid.RaycastGridPlane(ray);
        return worldPosition;
    }

    public bool LeftMouseButtonWasPressed() {
        return Mouse.current.leftButton.wasPressedThisFrame;
    }

    public bool RightMouseButtonWasPressed() {
        return Mouse.current.rightButton.wasPressedThisFrame;
    }

    public bool MouseScrolledUp() {
        return Mouse.current.scroll.ReadValue().y > 0.1f;
    }

    public bool MouseScrolledDown() {
        return Mouse.current.scroll.ReadValue().y < -0.1f;
    }

    public bool RightMouseButtonIsPressed() {
        return Mouse.current.rightButton.IsPressed();
    }

    public bool RotateModuleClockwise() {
        return playerControls.Modules.RotateModule.ReadValue<float>() > 0.01f;
    }

    public bool RotateModuleCounterClockwise() {
        return playerControls.Modules.RotateModule.ReadValue<float>() < -0.01f;
    }

    public bool PlaceModule() {
        return playerControls.Modules.PlaceModule.WasPressedThisFrame();
    }

    public bool PlaceModuleStarted() {
        return playerControls.Modules.PlaceModule.WasPressedThisFrame();
    }

    public bool PlaceModuleEnded() {
        return playerControls.Modules.PlaceModule.WasReleasedThisFrame();
    }

    public bool DeleteModule() {
        return playerControls.Modules.DeleteModule.WasPressedThisFrame();
    }

    public bool CancelModulePlacement() {
        return playerControls.Modules.CancelModulePlacement.WasPressedThisFrame();
    }


}
