using UnityEngine;

public class DeleteMode : MonoBehaviour, IMouseMode {

    public Vector3 LastMouseGridPosition { get; private set; }

    private FactoryGrid grid;
    private PlayerModeManager playerModeManager;

    public void Initialize(FactoryGrid grid, PlayerModeManager playerModeManager) {
        this.grid = grid;
        this.playerModeManager = playerModeManager;
    }

    public void EnterMode() {
    
    }

    public void ExitMode() {
    
    }

    public void UpdateInput(MouseInput mouseInput) {
        if (mouseInput.RightMouseButtonPressed()) {
            playerModeManager.GoToSelectionMode();
        }
        LastMouseGridPosition = mouseInput.LastGroundHitPoint;
    }
}


