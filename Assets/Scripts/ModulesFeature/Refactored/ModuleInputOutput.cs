using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleInputOutput : MonoBehaviour, IGridObject
{
    private ModuleSO moduleSettings;
    private Vector2Int originCell;
    private Grid grid;
    private int numRotations;

    public void Initialize(ModuleSO moduleSettings) {
        this.moduleSettings = moduleSettings;
    }

    public void HasInputAtPosition(Vector2Int gridPosition) {

    }   
    
    public void HasOutputAtPosition(Vector2Int gridPosition) {

    }

    private void ConnectOnPlacement() {

    }

    public List<Vector2Int> GetOccupyingCells(Vector2Int startCell, Grid grid) {
        throw new System.NotImplementedException();
    }

    public void OnPlacedOnGrid(Vector2Int startCell, Grid grid) {
        this.originCell = startCell;
        this.grid = grid;
        ConnectOnPlacement();
    }

    public void Destroy() {
        RemoveFromGrid(grid);
    }

    public void RemoveFromGrid(Grid grid) {
        throw new System.NotImplementedException();
    }

    public List<Vector2Int> GetShapeLayout() {
        throw new System.NotImplementedException();
    }
}
