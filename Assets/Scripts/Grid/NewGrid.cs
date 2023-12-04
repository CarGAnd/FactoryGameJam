using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class NewGrid {

    public UnityEvent GridSaved;
    public UnityEvent GridLoaded;

    private int columns;
    private int rows;
    private float cellWidth;
    private float cellHeight;
    private Vector3 origin;
    private Quaternion rotation;

    private Cell[,] cells;
    
    public void CreateGrid(int rows, int columns, float cellWidth, float cellHeight, Vector3 origin, Quaternion rotation) {
        this.columns = columns;
        this.rows = rows;
        this.cellWidth = cellWidth;
        this.cellHeight = cellHeight;
        this.origin = origin;
        this.rotation = rotation;

        cells = new Cell[rows, columns];
    }

    public void PlaceOnGrid(IGridObject gridObject, Vector2Int startingCell) {
        List<Cell> cells = gridObject.GetOccupiedCells(this, startingCell);
        foreach(Cell c in cells) {
            c.SetOccupant(gridObject);
        }
    }

    public void ResizeGrid(int newRows, int newColumns) {
        Cell[,] newGrid = new Cell[newRows, newColumns];

        int minRows = Mathf.Min(newRows, rows);
        int minColumns = Mathf.Min(newColumns, columns);

        for(int y = 0; y < minRows; y++) {
            for (int x = 0; x < minColumns; x++) {
                Cell newCell = new Cell(y, x, this);
                newCell.SetOccupant(cells[y, x].GetOccupant());
                newGrid[y, x] = newCell;
            }
        }

        cells = newGrid;
        rows = newRows;
        columns = newColumns;
    }

    public void AdjustCellSize(float newCellWidth, float newCellHeight) {
        cellWidth = newCellWidth;
        cellHeight = newCellHeight;
    }

    public void MoveGrid(Vector3 newOrigin) {
        origin = newOrigin;
    }

    public Cell GetCell(int row, int column) {
        if (!IsWithingBounds(row, column)) {
            return null;
        }
        return cells[row, column];
    }

    public Cell GetCell(Vector3 worldPos) {
        Vector3 adjusted = Quaternion.Inverse(rotation) * worldPos - origin;
        int gridX = (int)(adjusted.x / cellWidth);
        int gridY = (int)(adjusted.z / cellHeight);

        return cells[gridY, gridX];
    }

    public void SetCell(int row, int column, Cell newObject) {
        if (!IsWithingBounds(row, column)) {
            return;
        }
        cells[row, column] = newObject;
    }

    private bool IsWithingBounds(int row, int column) {
        return column >= 0 && column < columns && row >= 0 && row < rows;
    }

    #region TODO

    public void SaveToFile(string filePath) {
        GridSaved?.Invoke();
    }

    public void LoadFromFile(string filePath) {
        GridLoaded?.Invoke();
    }

    public List<Cell> FindPath(Vector2Int startPosition, Vector2Int endPosition) {
        return null;
    }

    public void CombineGrids(List<NewGrid> otherGrid) {

    }

    private void OnDrawGizmos() {

    }

    #endregion
}

public interface IGridObject {
    List<Cell> GetOccupiedCells(NewGrid grid, Vector2Int startingCell);
    void PlaceOnGrid(NewGrid grid, Vector2Int startingCell);
    void RemoveFromGrid();
    T GetOccupant<T>();
}

public interface IGridInteractable {
    void OnSelected();
    bool IsSelected();
    bool IsPlaced();
}