using UnityEngine;
using UnityEngine.Events;

public class MouseInput : MonoBehaviour {
    
    public UnityEvent<Vector3> MouseOverGridSpace;
    
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask groundLayer;
    [field: SerializeField] public Grid BuildGrid { get; private set; }

    public Vector3 LastHitPoint { get; private set; }
    public Vector2Int LastMouseGridPos { get; private set; }

    void Awake() {
        cam = Camera.main;
    }

    void Update() {
        UpdateMousePosition();
    }

    private void UpdateMousePosition() {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer)) {
            Vector3 position = hit.point;
            Vector3 gridCellCenter = BuildGrid.GetCellCenter(position);
            LastHitPoint = position;

            if (BuildGrid.GetCellCoords(position) != LastMouseGridPos) {
                LastMouseGridPos = BuildGrid.GetCellCoords(position);
                MouseOverGridSpace?.Invoke(gridCellCenter);
            }
        }
    }
}
