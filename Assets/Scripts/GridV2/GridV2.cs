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
        // Implementation to move the grid
    }

    public void RotateGrid(Quaternion newRotation) 
    {
        // Implementation to rotate the grid
    }

    // Cell via coordinates
    public CellV2 GetCell(int row, int column) 
    {
        throw new System.NotImplementedException();
    }
    // Cell via world position
    public CellV2 GetCell(Vector3 worldPosition) 
    {
        throw new System.NotImplementedException();
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
        throw new System.NotImplementedException();
    }
    public List<CellV2> FindPathAsCells(CellV2 startCell, CellV2 endCell) 
    {
        throw new System.NotImplementedException();
    }

    public List<Vector2Int> FindPathAsCoordinates(CellV2 startCell, CellV2 endCell) 
    {
        throw new System.NotImplementedException();
    }

    private bool CellWithinBounds(int row, int column) 
    {
        throw new System.NotImplementedException();
    }
    public void PlaceObject(IGridObject gridObject, CellV2 startCell, List<Vector2Int> shapeLayout) 
    {
        // A IGridObject has a place on gird method, which calls this one.
        // Shape Layout represents the cells in addition to the center or start cell in relative coordinates to the start cell.
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