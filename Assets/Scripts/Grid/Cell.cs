using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Cell
{
    private NewGrid grid;
    private int row;
    private int column;
    private CellState state;
    private IGridObject occupant;
    private GameObject groundPrefab;

    public static Vector2Int[] fourWayNeighbors = new Vector2Int[]
    {
        new Vector2Int(1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(-1, 0),
        new Vector2Int(0, -1)
    };

    public static Vector2Int[] eightWayNeighbors = new Vector2Int[]
    {
        new Vector2Int(1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(-1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(1, 1),
        new Vector2Int(1, -1),
        new Vector2Int(-1, 1),
        new Vector2Int(-1, -1)
    };

    // Constructor to initialize the Cell
    public Cell(int row, int column, NewGrid grid, GameObject groundPrefab = null)
    {
        this.row = row;
        this.column = column;
        this.grid = grid;
        this.groundPrefab = groundPrefab;
        state = CellState.EMPTY;

        GameObject groundObject = GameObject.Instantiate(groundPrefab, grid.GetCellWorldPosition(row,column), UnityEngine.Quaternion.identity);
        AttachCellGround(groundObject);
    }

    // Returns the state of the cell (EMPTY or OCCUPIED)
    public CellState GetState()
    {
        return state;
    }

    // Returns the current occupant of the cell
    public IGridObject GetOccupant()
    {
        return occupant;
    }

    // Sets a new occupant for the cell and updates its state
    public void SetOccupant(IGridObject newOccupant)
    {
        occupant = newOccupant;
        state = newOccupant == null ? CellState.EMPTY : CellState.OCCUPIED;
    }

    // Returns the prefab for the ground of the cell
    public GameObject GetGroundPrefab()
    {
        return groundPrefab;
    }

    // Returns the coordinates of the cell as Vector2Int
    public Vector2Int GetCoordinates()
    {
        return new Vector2Int(column, row);
    }

    // Checks if the cell is occupied
    public bool IsOccupied()
    {
        return state == CellState.OCCUPIED;
    }

    // Returns a list of neighboring cells based on the specified configuration
    public List<Cell> GetNeighbors(NeighborConfiguration neighborConfiguration)
    {
        List<Cell> neighbors = new List<Cell>();
        Vector2Int[] neighborDeltas = neighborConfiguration == NeighborConfiguration.FOUR_WAY ? fourWayNeighbors : eightWayNeighbors;
        Vector2Int coord = GetCoordinates();

        foreach (Vector2Int delta in neighborDeltas)
        {
            Vector2Int cellCoord = coord + delta;
            Cell gridCell = grid.GetCell(cellCoord.y, cellCoord.x);
            if (gridCell != null)
            {
                neighbors.Add(gridCell);
            }
        }

        return neighbors;
    }

    public void AttachCellGround(GameObject groundObject)
    {
        CellGround cellGround = groundObject.GetComponent<CellGround>();
        cellGround.SetCellReference(this);
    }

    // TODO: Placeholder for the logic to save cell state
    public void SaveCell()
    {
        // Implement save logic
    }

    // TODO: Placeholder for the logic to load cell state
    public void LoadCell()
    {
        // Implement load logic
    }

    // Returns the center position of the cell in world space
    public UnityEngine.Vector3 GetCenter()
    {
        // Implement logic to calculate the center based on cell size and position
        UnityEngine.Vector2 cellSize = grid.GetCellSize;

        UnityEngine.Vector3 cellWorldPosition = grid.GetCellWorldPosition(row, column);

        return new UnityEngine.Vector3(cellWorldPosition.x + cellSize.x / 2, cellWorldPosition.y, cellWorldPosition.z + cellSize.y / 2);
    }

    // Returns the grid ID this cell belongs to
    public string GetGridId()
    {
        return grid.GetGridID();
    }
}

public enum NeighborConfiguration
{
    FOUR_WAY,
    EIGHT_WAY
}

public enum CellState
{
    EMPTY,
    OCCUPIED
}


