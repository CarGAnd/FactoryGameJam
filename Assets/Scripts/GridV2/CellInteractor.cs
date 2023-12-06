using UnityEngine;

public class CellInteractor : MonoBehaviour {
    private CellV2 cell;

    // Calls SelectCell() in cell.
    void OnMouseDown() {
        // Implementation for mouse down interaction
    }

    // Maybe for hovering logic, highlighting a cell if we're in build mode or whatever.
    void OnMouseEnter() {
        // Implementation for mouse enter interaction
    }

    public void SetCellReference(CellV2 cell) {
        this.cell = cell;
    }
}

