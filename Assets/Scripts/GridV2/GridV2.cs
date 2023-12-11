using System.Collections.Generic;
using UnityEngine;

public class GridV2 {
    private int columns;
    private int rows;
    private IGridLayout layout;
    private Vector3 origin;
    private Quaternion rotation;
    private Vector2 cellSize;
    private string id;
    private CellV2[,] cells;
    //private PathfindingAlgorithm pathfindingAlgorithm;

    private static Vector2Int[] fourWayNeighbors = new Vector2Int[]
    {
        new Vector2Int(1, 0),
        new Vector2Int(0, 1),
        new Vector2Int(-1, 0),
        new Vector2Int(0, -1)
    };

    private static Vector2Int[] eightWayNeighbors = new Vector2Int[]
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

    public GridV2(IGridLayout layout, int rows, int columns, Vector3 origin, Quaternion rotation) 
    {
        this.layout = layout;
        this.rows = rows;
        this.columns = columns;
        this.origin = origin;
        this.rotation = rotation;
        cells = new CellV2[rows, columns];
        // Construct the grid using the layout. The idea is each layout will construct a grid differently.
        layout.ConstructGrid(rows, columns, cellSize, origin, rotation, cells);
    }

    public string GetID() 
    {
        return id;
    }

    public void ResizeGrid(int newRows, int newColumns) 
    {
        // Implementation to resize the grid.
        // Remember to preserve existing cells' data
    }

    public void MoveGrid(Vector3 newOrigin) 
    {
        origin = newOrigin;
    }

    public void RotateGrid(Quaternion newRotation) 
    {
        rotation = newRotation;
    }

    // Cell via coordinates
    public CellV2 GetCell(int row, int column) 
    {
        if (CellWithinBounds(row, column)) {
            return cells[row, column];
        }
        else {
            return null;
        }
    }

    // Cell via world position
    public CellV2 GetCell(Vector3 worldPosition) 
    {
        Vector3 adjusted = Quaternion.Inverse(rotation) * worldPosition - origin;
        int gridX = (int)(adjusted.x / cellSize.x);
        int gridY = (int)(adjusted.z / cellSize.y);

        return GetCell(gridY, gridX);
    }

    // Cell via mouse position
    public CellV2 GetCell(Vector2 mousePosition) 
    {
        throw new System.NotImplementedException();
    }

    public void AdjustCellSize(float length, float width) 
    {
        // Again this will depend on the type of layout.
        layout.AdjustCellSize(length, width);
        // Additional implementation to adjust cell sizes in the grid
    }

    // Moved to grid because a Cell doesn't need to know its neighbors
    public List<CellV2> GetCellNeighbors(CellV2 cell, NeighborConfiguration configuration) 
    {
        List<CellV2> neighbors = new List<CellV2>();
        Vector2Int[] neighborDeltas = configuration == NeighborConfiguration.FOUR_WAY ? fourWayNeighbors : eightWayNeighbors;
        Vector2Int coord = cell.GetCellCoordinates();

        foreach (Vector2Int delta in neighborDeltas) {
            Vector2Int cellCoord = coord + delta;
            if(CellWithinBounds(cellCoord.y, cellCoord.x)) {
                CellV2 gridCell = cells[cellCoord.y, cellCoord.x];
                neighbors.Add(gridCell);
            }
        }
        return neighbors;
    }

    private bool CellWithinBounds(int row, int column) 
    {
        return row >= 0 && row < rows && column >= 0 && column < columns;
    }

    // A IGridObject has a place on grid method, which calls this one.
    // Shape Layout represents the cells in addition to the center or start cell in relative coordinates to the start cell.
    public void PlaceObject(IGridObject gridObject, CellV2 startCell, List<Vector2Int> shapeLayout) 
    {
        startCell.SetOccupyingObject(gridObject);
        foreach(Vector2Int deltaCoord in shapeLayout) {
            Vector2Int coord = startCell.GetCellCoordinates() + deltaCoord;
            if(CellWithinBounds(coord.y, coord.x)) {
                CellV2 cell = cells[coord.y, coord.x];
                cell.SetOccupyingObject(gridObject);
            }        
        }
    }

    public List<CellV2> FindPathAsCells(CellV2 startCell, CellV2 endCell) {
        throw new System.NotImplementedException();
    }

    public List<Vector2Int> FindPathAsCoordinates(CellV2 startCell, CellV2 endCell) {
        throw new System.NotImplementedException();
    }

    public void SaveGrid() 
    {
        // Implementation to save grid state
        // This is meant to save the grid data.
    }

    public void LoadGrid() 
    {
        // Implementation to load grid state
        // This is meant to load the grid data.
    }

    public void VisualizeGrid() 
    {
        // Implementation for grid visualization
        // might be a different class, might be gizmos, not sure.
    }
}
// Moved to Grid as cells didn't need to know their neighbors.
public enum NeighborConfiguration 
{
    FOUR_WAY,
    EIGHT_WAY
}