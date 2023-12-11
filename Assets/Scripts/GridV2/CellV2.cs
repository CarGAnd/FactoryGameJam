using System.Collections.Generic;
using UnityEngine;

public class CellV2 {
    private GridV2 grid;
    private int row;
    private int column;
    private CellState state;
    private IGridObject occupyingObject;
    [SerializeField] private GameObject groundPrefab;

    // These Cells will be created in GridLayout I think. in ConstructGrid()
    public CellV2(int row, int column, CellState state, GridV2 grid)
    {
        this.row = row;
        this.column = column;
        this.state = state;
        this.grid = grid;

        // Ensure CellInteractor is attached to groundPrefab.
        CellInteractor cellInteractor = groundPrefab.GetComponent<CellInteractor>();
        if(cellInteractor != null) {
            cellInteractor.SetCellReference(this);
        }
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

    public void RemoveOccupyingObject() 
    {
        occupyingObject = null;
        state = CellState.EMPTY;
    }

    //This refers to the coordinates within the grid, More or less just the row and column.
    public Vector2Int GetCellCoordinates()
    {
        return new Vector2Int(column, row);
    }

    public bool IsOccupied() 
    {
        return occupyingObject != null;
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


