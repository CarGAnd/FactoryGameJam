using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class NewGrid
{
    public UnityEvent GridSaved;
    public UnityEvent GridLoaded;

    private int rows;
    private int columns;
    private float cellWidth;
    private float cellHeight;
    private Vector3 origin;
    private Quaternion rotation;
    private Cell[,] cells;

    private string gridId;

    public Vector2 GetCellSize => new Vector2(cellWidth, cellHeight);

    // Creates the grid with specified dimensions and properties
    public void CreateGrid(int rows, int columns, float cellWidth, float cellHeight, Vector3 origin, Quaternion rotation)
    {
        // Initialization logic...
        // Should Register with GridInteraction

        this.rows = rows;
        this.columns = columns;
        this.cellWidth = cellWidth;
        this.cellHeight = cellHeight;
        this.origin = origin;
        this.rotation = rotation;
        this.cells = new Cell[rows, columns];

        //populate cells array
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; i++)
            {
                cells[i, j] = new Cell(i, j, this);
            }
        }

        gridId = System.Guid.NewGuid().ToString();

        GridInteraction.RegisterGrid(this);
    }

    // Places a grid object on the grid
    public void PlaceOnGrid(GameObject placedObject, Cell startingCell)
    {
        IGridObject gridObject = placedObject.GetComponent<IGridObject>();
        List<Cell> occupiedCells = gridObject.GetOccupiedCells(this, startingCell);

        foreach (Cell cell in occupiedCells)
        {
            if (cell == null || cell.IsOccupied())
            {
                return;
            }
        }

        GameObject newObject = MonoBehaviour.Instantiate(gridObject.GetOccupant<GameObject>(), startingCell.GetCenter(), Quaternion.identity);
        foreach (Cell cell in occupiedCells)
        {
            cell.SetOccupant(newObject.GetComponent<IGridObject>());
        }
    }

    // Resizes the grid to new dimensions
    public void ResizeGrid(int newRows, int newColumns)
    {
        // Resizing logic, ensure to preserve existing cells' data
        //Update cells array, but preserve already created cells. create new cells if needed
        Cell[,] newCells = new Cell[newRows, newColumns];

        for (int i = 0; i < newRows; i++)
        {
            for (int j = 0; j < newColumns; j++)
            {
                if (i < rows && j < columns)
                {
                    newCells[i, j] = cells[i, j];
                }
                else
                {
                    newCells[i, j] = new Cell(i, j, this);
                }
            }
        }

        cells = newCells;
        rows = newRows;
        columns = newColumns;
    }

    // Adjusts the size of each cell in the grid
    public void AdjustCellSize(float newCellWidth, float newCellHeight)
    {
        // Adjust cell dimensions and possibly reposition objects within the grid
        cellWidth = newCellWidth;
        cellHeight = newCellHeight;

        foreach (Cell cell in cells)
        {
            cell.GetGroundPrefab().transform.localScale = new Vector3(cellWidth, 1, cellHeight);
        }
    }

    // Moves the grid to a new origin point
    public void MoveGrid(Vector3 newOrigin)
    {
        origin = newOrigin;
    }

    // Gets a specific cell based on row and column index
    public Cell GetCell(int row, int column)
    {
        if (IsWithingBounds(row, column))
        {
            return cells[row, column];
        }
        else
        {
            return null;
        }
    }

    // Gets a cell based on mousePosition position
    public Cell GetCell(Vector2 mousePos)
    {
        // Convert mouse position to world position, then return the cell

        return null;
    }
    
    public Vector3 GetCellWorldPosition(int row, int column)
    {
        return new Vector3(origin.x + column * cellWidth, origin.y, origin.z + row * cellHeight);
    }

    // Sets a new cell object at a specific grid location
    public void SetCell(int row, int column, Cell newObject)
    {        
        if(IsWithingBounds(row, column))
        {
            cells[row, column] = newObject;
        }
    }
    private bool IsWithingBounds(int row, int column)
    {
        return row >= 0 && row < rows && column >= 0 && column < columns;
    }

    public string GetGridID()
    {
        return gridId;
    }
    // TODO: Methods to be implemented or further defined
    public void SaveToFile(string filePath)
    {
        // Logic to save grid state to a file
        GridSaved?.Invoke();
    }

    public void LoadFromFile(string filePath)
    {
        // Logic to load grid state from a file
        GridLoaded?.Invoke();
    }

    public List<Cell> FindPath(Vector2Int startPosition, Vector2Int endPosition)
    {
        // Implement pathfinding logic, possibly using the pathfindingAlgorithm
        return null;
    }

    public void CombineGrids(List<NewGrid> otherGrids)
    {
        // Logic to combine multiple grids into one, adjusting cells and dimensions as needed
    }

    private void OnDrawGizmos()
    {
    }
}


public interface IGridObject {
    List<Cell> GetOccupiedCells(NewGrid grid, Cell startingCell);
    void PlaceOnGrid(NewGrid grid, Vector2Int startingCell);
    void RemoveFromGrid();
    T GetOccupant<T>();
}

public interface IGridInteractable {
    void OnSelected();
    bool IsSelected();
    bool IsPlaced();
}