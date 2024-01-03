using System.Collections.Generic;
using UnityEngine;

public class Cell {
    private Grid grid;
    private int row;
    private int column;
    private CellState state;
    private IGridObject occupyingObject;
    //The list of cells that has the same occupying object
    private List<Cell> sharedCells;

    public Cell(int row, int column, Grid grid, CellState state = CellState.EMPTY)
    {
        this.row = row;
        this.column = column;
        this.state = state;
        this.grid = grid;
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
        if(occupyingObject != null) 
        {
            this.occupyingObject = occupyingObject;
            state = CellState.OCCUPIED;
        } 
        else 
        {
            RemoveOccupyingObject();
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
