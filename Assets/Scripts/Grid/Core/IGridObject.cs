using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGridObject {
    // Returns a list of cells occupied by the object starting from a given cell.
    List<Vector2Int> GetOccupyingCells(Vector2Int startCell, Grid grid);

    // Places the object on the grid starting from a specified cell.
    void OnPlacedOnGrid(Vector2Int startCell, Grid grid);

    // Removes the object from the grid.
    void RemoveFromGrid(Grid grid);

    // Returns the shape layout of the object in terms of relative cell positions.
    List<Vector2Int> GetShapeLayout();
}

public interface IGridInteractable : IGridObject {
    // Method called when the object is selected (e.g., through user interaction).
    void OnSelected();

    // Returns whether the object is currently selected.
    bool IsSelected();

    // Returns whether the object is currently placed on the grid.
    bool IsPlaced();
}
