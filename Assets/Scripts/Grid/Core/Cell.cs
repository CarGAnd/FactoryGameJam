using System.Collections.Generic;
using UnityEngine;

public class Cell {
    private Grid grid;
    private int row;
    private int column;
    private CellState state;
    private IGridObject occupyingObject;
    private GameObject groundObject;
    //The list of cells that has the same occupying object
    private List<Cell> sharedCells;

    public Cell(int row, int column, Grid grid, GameObject prefab, CellState state = CellState.EMPTY)
    {
        this.row = row;
        this.column = column;
        this.state = state;
        this.grid = grid;

        groundObject = MonoBehaviour.Instantiate(prefab, grid.transform);
        UpdatePosition();

        // Ensure CellInteractor is attached to groundPrefab.
        CellInteractor cellInteractor = groundObject.GetComponent<CellInteractor>();
        if(cellInteractor != null) {
            cellInteractor.SetCellReference(this);
        }
    }

    public void UpdatePosition() {
        Vector3 position = grid.GetCellCenter(new Vector2Int(row, column)) + Vector3.down * 0.5f;
        Quaternion rotation = grid.Rotation;
        Vector3 scale = new Vector3(grid.CellSize.x, 1, grid.CellSize.y);

        groundObject.transform.SetPositionAndRotation(position, rotation);
        groundObject.transform.localScale = scale;
    }

    public CellState GetState() 
    {
        return state;
    }

    public IGridObject GetOccupyingObject() 
    {
        return occupyingObject;
    }

    public void SetOccupyingObject(IGridObject occupyingObject) 
    {
        this.occupyingObject = occupyingObject;
        if(occupyingObject != null) 
        {
            state = CellState.OCCUPIED;
        } 
        else 
        {
            state = CellState.EMPTY;
        }
    }

    public void SetSharedCells(List<Cell> sharedCells) {
        this.sharedCells = sharedCells;
    }

    public List<Cell> GetSharedCells() {
        return sharedCells;
    }

    public void RemoveOccupyingObject() 
    {
        occupyingObject = null;
        sharedCells = null;
        state = CellState.EMPTY;
    }

    //This refers to the coordinates within the grid, More or less just the row and column.
    public Vector2Int GetCellCoordinates()
    {
        return new Vector2Int(column, row);
    }

    public bool IsOccupied() 
    {
        return state == CellState.OCCUPIED;
    }

    //This method will be called from CellInteractor, which should be placed on the groundPrefab.
    public void SelectCell() 
    {
        // Implementation for selecting the cell
    }

    public void SaveCell() 
    {
        // Implementation to save cell state
    }

    public void LoadCell() 
    {
        // Implementation to load cell state
    }
}

public enum CellState 
{
    EMPTY,
    OCCUPIED
}
