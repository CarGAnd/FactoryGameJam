using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGridLayout {
    // Calculates the world position of a cell based on its row and column.
    Vector3 CalculateCellPosition(int row, int column);

    // Constructs the grid with the specified dimensions.
    void ConstructGrid(int rows, int columns, Vector2 cellSize, Vector3 origin, Quaternion rotation, CellV2[,] cells);

    // Adjusts the size of cells in the grid.
    void AdjustCellSize(float length, float width);
}

public class SquareGridLayout : IGridLayout {

    private Quaternion rotation;
    private Vector2 cellSize;
    private Vector3 origin;

    public Vector3 CalculateCellPosition(int row, int column) 
    {
        return rotation * (new Vector3(column * cellSize.x, 0, row * cellSize.y)) + origin;
    }

    // Implementation for constructing a square grid
    public void ConstructGrid(int rows, int columns, Vector2 cellSize, Vector3 origin, Quaternion rotation, CellV2[,] cells) 
    {
        this.rotation = rotation;
        this.cellSize = cellSize;
        this.origin = origin;
    }

    // Implementation for adjusting cell size in a square grid
    // Length and Width are the two side lengths in a square cell.
    public void AdjustCellSize(float length, float width) 
    {

    }
}

public class HexGridLayout : IGridLayout {
    // Implementation for calculating cell position in a hexagonal grid
    public Vector3 CalculateCellPosition(int row, int column) 
    {
        throw new System.NotImplementedException();
    }

    // Implementation for constructing a hexagonal grid
    public void ConstructGrid(int rows, int columns, Vector2 cellSize, Vector3 origin, Quaternion rotation, CellV2[,] cells) 
    {

    }

    // Implementation for adjusting cell size in a Hex grid
    // Length represents distance between two parallel sides 
    // width represents distance between two vertices on the opposite sides of the parallel sides.
    public void AdjustCellSize(float length, float width) 
    {

    }
}
