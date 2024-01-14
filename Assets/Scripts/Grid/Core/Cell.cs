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
    private Vector2Int occupyingObjectOrigin;

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

    public void SetOccupyingObject(IGridObject occupyingObject, Vector2Int objectOriginCell, List<Cell> sharedCells = null) 
    {
        if(occupyingObject != null) 
        {
            this.occupyingObject = occupyingObject;
            this.occupyingObjectOrigin = objectOriginCell;
            this.sharedCells = sharedCells;
            state = CellState.OCCUPIED;
        } 
        else 
        {
            RemoveOccupyingObject();
        }
    }

    public void SetOccupyingObject(IGridObject occupyingObject) {
        SetOccupyingObject(occupyingObject, new Vector2Int(column, row), null);
    }

    public List<Cell> GetSharedCells() {
        return sharedCells;
    }

    public Vector2Int GetObjectOrigin() {
        return occupyingObjectOrigin;
    }

    public Vector2Int GetOccupyingObjectOrigin() {
        return occupyingObjectOrigin;
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
}

public enum CellState 
{
    EMPTY,
    OCCUPIED
}
