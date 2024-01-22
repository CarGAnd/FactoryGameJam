using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class MouseInput : MonoBehaviour {
    
    public UnityEvent<Vector3> MouseOverGridSpace;
    
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask groundLayer;
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
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer)) {
            Vector3 position = hit.point;
            Vector3 gridCellCenter = BuildGrid.GetCellCenter(position);
            LastGroundHitPoint = position;

            if (BuildGrid.GetCellCoords(position) != LastMouseGridPos) {
                LastMouseGridPos = BuildGrid.GetCellCoords(position);
                MouseOverGridSpace?.Invoke(gridCellCenter);
            }
        }
    }

    public bool LeftMouseButtonPressed() {
        return Mouse.current.leftButton.wasPressedThisFrame;
    }

    public bool RightMouseButtonPressed() {
        return Mouse.current.rightButton.wasPressedThisFrame;
    }
}
