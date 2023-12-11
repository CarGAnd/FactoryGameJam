using System.Collections.Generic;
using UnityEngine;

public class GridV2 : MonoBehaviour {
    private int columns;
    private int rows;
    private IGridLayout layout;
    private Vector3 origin;
    private Quaternion rotation;
    private Vector2 cellSize;
    private string id;
    private CellV2[,] cells;
    //private PathfindingAlgorithm pathfindingAlgorithm;
    
    public GridV2(IGridLayout layout, int rows, int columns, Vector3 origin, Quaternion rotation) 
    {
        this.layout = layout;
        this.rows = rows;
        this.columns = columns;
        this.origin = origin;
        this.rotation = rotation;
        cells = new CellV2[rows, columns];
    }

    public string GetID() 
    {
        return id;
    }

    // A IGridObject has a place on grid method, which calls this one.
    // Shape Layout represents the cells in addition to the center or start cell in relative coordinates to the start cell.
    public void PlaceObject(IGridObject gridObject, CellV2 startCell, List<Vector2Int> shapeLayout) {
        startCell.SetOccupyingObject(gridObject);
        foreach (Vector2Int deltaCoord in shapeLayout) {
            Vector2Int coord = startCell.GetCellCoordinates() + deltaCoord;
            if (CellWithinBounds(coord.y, coord.x)) {
                CellV2 cell = cells[coord.y, coord.x];
                cell.SetOccupyingObject(gridObject);
            }
        }
    }

    public void ResizeGrid(int newRows, int newColumns) 
    {
        // Implementation to resize the grid.
        // Remember to preserve existing cells' data
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
        Vector2Int cellCoordinates = layout.GetCellCoordinate(adjusted);

        return GetCell(cellCoordinates.y, cellCoordinates.x);
    }

    // World position via cell
    public Vector3 CalculateCellPosition(int row, int column) {
        Vector3 normalizedPosition = layout.CalculateCellPosition(row, column);
        Vector3 worldPosition = rotation * new Vector3(normalizedPosition.x * cellSize.x, 0, normalizedPosition.z * cellSize.y) + origin;
        return worldPosition;
    }

    public void MoveGrid(Vector3 newOrigin) {
        origin = newOrigin;
    }

    public void RotateGrid(Quaternion newRotation) {
        rotation = newRotation;
    }

    public void AdjustCellSize(float length, float width) {
        cellSize = new Vector2(width, length);
    }

    private bool CellWithinBounds(int row, int column) 
    {
        return row >= 0 && row < rows && column >= 0 && column < columns;
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
