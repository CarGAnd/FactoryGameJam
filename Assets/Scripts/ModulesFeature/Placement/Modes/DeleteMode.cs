using UnityEngine;
using UnityEngine.Events;

public class DeleteMode : MonoBehaviour, IMouseMode {

    public UnityEvent enterDeleteMode;
    public UnityEvent exitDeleteMode;

    public Vector3 LastMouseGridPosition { get; private set; }

    private FactoryGrid grid;
    private PlayerModeManager playerModeManager;

    public void Initialize(FactoryGrid grid, PlayerModeManager playerModeManager) {
        this.grid = grid;
        this.playerModeManager = playerModeManager;
    }

    public void EnterMode() {
        enterDeleteMode.Invoke();
    }

    public void ExitMode() {
        exitDeleteMode.Invoke();
    }

    public void UpdateInput(MouseInput mouseInput) {
        LastMouseGridPosition = mouseInput.LastGroundHitPoint;
        if (mouseInput.RightMouseButtonWasPressed()) {
            playerModeManager.GoToSelectionMode();
        }
        if (mouseInput.LeftMouseButtonWasPressed()) {
            RemoveModule(grid.GetCellCoords(LastMouseGridPosition));
        }
    }

    public void RemoveModule(Vector2Int gridPosition) {
        IGridObject gridObject = grid.GetObjectAt(gridPosition);
        if(gridObject != null) {
            gridObject.DestroyObject();
        }
    }   
}


