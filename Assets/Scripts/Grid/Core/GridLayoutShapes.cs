using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGridLayout {

    Vector2Int GetCellCoordinate(Vector3 worldPosition);

    // Calculates the world position of a cell based on its row and column.
    Vector3 CalculateCellPosition(Vector2Int cellCoord);

    Vector3 GetCellCenter(Vector2Int cellCoord);

    //Get all neighbors of a cell
    //The specific types in this function will likely be changed
    List<Vector2Int> GetCellNeighbors(Vector2Int cellCoord);
}

public class SquareGridLayout : IGridLayout {

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

    //Return the worldPosition of the given row and column in a grid with 1x1 cells, no offset, and no rotation
    public Vector3 CalculateCellPosition(Vector2Int cellCoord) 
    {
        return new Vector3(cellCoord.x, 0, cellCoord.y);
    }

    public Vector3 GetCellCenter(Vector2Int cellCoord) {
        return CalculateCellPosition(cellCoord) + new Vector3(0.5f, 0, 0.5f);
    }

    //Return the cell coordinates of the given position in a grid with 1x1 cells, no offset, and no rotation
    public Vector2Int GetCellCoordinate(Vector3 normalizedPosition) {
        int gridX = (int) normalizedPosition.x;
        int gridY = (int) normalizedPosition.z;

        return new Vector2Int(gridX, gridY);
    }

    // Moved to grid because a Cell doesn't need to know its neighbors
    public List<Vector2Int> GetCellNeighbors(Vector2Int cellCoord) {
        List<Vector2Int> neighbors = new List<Vector2Int>();
        
        foreach(Vector2Int offset in fourWayNeighbors) {
            neighbors.Add(cellCoord + offset);
        }

        return neighbors;
    }

    // Moved to Grid as cells didn't need to know their neighbors.
    public enum NeighborConfiguration {
        FOUR_WAY,
        EIGHT_WAY
    }
}

public class HexGridLayout : IGridLayout {

    public Vector3 CalculateCellPosition(Vector2Int cellCoord) {
        throw new System.NotImplementedException();
    }

    public Vector3 GetCellCenter(Vector2Int cellCoord) {
        throw new System.NotImplementedException();
    }

    public Vector2Int GetCellCoordinate(Vector3 worldPosition) {
        throw new System.NotImplementedException();
    }

    public List<Vector2Int> GetCellNeighbors(Vector2Int cellCoord) {
        throw new System.NotImplementedException();
    }
}
