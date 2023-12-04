using System.Collections.Generic;
using UnityEngine;

public class Cell {

    private NewGrid grid;
    private int row;
    private int column;
    private CellState state;
    private IGridObject occupant;
    private GameObject groundPrefab;

    public static Vector2Int[] fourWayNeighbors = new Vector2Int[]
    {
        new Vector2Int(1,0),
        new Vector2Int(0,1),
        new Vector2Int(-1,0),
        new Vector2Int(0,-1)
    };

    public static Vector2Int[] eightWayNeighbors = new Vector2Int[]
    {
        new Vector2Int(1,0),
        new Vector2Int(0,1),
        new Vector2Int(-1,0),
        new Vector2Int(0,-1),
        new Vector2Int(1,1),
        new Vector2Int(1,-1),
        new Vector2Int(-1,1),
        new Vector2Int(-1,-1)
    };

    public Cell(int row, int column, NewGrid grid) {
        this.row = row;
        this.column = column;
        this.grid = grid;
    }

    public CellState GetState() {
        return state;
    }

    public Vector2Int GetCoordinates() {
        return new Vector2Int(column, row);
    }

    public IGridObject GetOccupant() {
        return occupant;
    }

    public void SetOccupant(IGridObject newOccupant) {
        occupant = newOccupant;
        state = newOccupant == null ? CellState.EMPTY : CellState.OCCUPIED;
    }

    public bool IsOccupied() {
        return state == CellState.OCCUPIED;
    }

    public List<Cell> GetNeighbors(NeightborConfiguration neighborConfiguration) {
        List<Cell> neighbors = new List<Cell>();
        Vector2Int[] neighborDeltas = neighborConfiguration == NeightborConfiguration.FOUR_WAY ? fourWayNeighbors : eightWayNeighbors;
        Vector2Int coord = GetCoordinates();

        foreach(Vector2Int delta in neighborDeltas) {
            Vector2Int cellCoord = coord + delta;
            Cell gridCell = grid.GetCell(cellCoord.y, cellCoord.x);
            if (gridCell != null) {
                neighbors.Add(gridCell);
            }
        }

        return neighbors;
    }

    #region TODO

    public void SaveCell() {

    }

    public void LoadCell() {

    }

    #endregion
}

public enum NeightborConfiguration {
    FOUR_WAY,
    EIGHT_WAY
}

public enum CellState {
    EMPTY,
    OCCUPIED
}

