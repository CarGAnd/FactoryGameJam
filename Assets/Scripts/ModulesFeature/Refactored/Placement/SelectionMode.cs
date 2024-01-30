using UnityEngine;
using UnityEngine.Events;

public class SelectionMode : MonoBehaviour, IMouseMode {

    [HideInInspector] public UnityEvent enterSelectionMode;
    [HideInInspector] public UnityEvent exitSelectionMode;

    public Vector3 LastMouseGridPosition { get; private set; }
    private FactoryGrid grid;

    public void Initialize(FactoryGrid grid) {
        this.grid = grid;
    }

    public void EnterMode() {
        enterSelectionMode.Invoke();
    }

    public void ExitMode() {
        exitSelectionMode.Invoke();
    }

    public void UpdateInput(MouseInput mouseInput) {
        LastMouseGridPosition = mouseInput.LastGroundHitPoint;
    }
}


