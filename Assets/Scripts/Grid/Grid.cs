using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Grid<T> {

    [field: SerializeField] public int Width { get; private set; }
    [field: SerializeField] public int Height { get; private set; }
    [field: SerializeField] public float CellSize { get; private set; }
    [field: SerializeField] public Vector3 Origin { get; private set; }
    [field: SerializeField] public float Rotation { get; private set; }

    private T[,] gridCells;
    private Quaternion gridRotation;

    public Grid(int width, int height, Vector3 origin, float cellSize = 1, float rotation = 0) {
        this.Width = width;
        this.Height = height;
        this.CellSize = cellSize;
        this.Origin = origin;
        this.Rotation = rotation;
        
        gridRotation = Quaternion.Euler(0, rotation, 0);
        gridCells = new T[width, height];
    }

    public bool PositionIsOccupied(int x, int y) {
        if (!CoordinatesAreValid(x, y)) {
            return true;
        }
        //Since T can be a primitive, we cant test for null
        //It is apparently not possible to do gridCells[x, y] != default(T)
        //This seems to be the "elegant" solution that catches both primitives and objects
        return !EqualityComparer<T>.Default.Equals(gridCells[x, y], default(T));
    }

    public bool PositionIsOccupied(Vector3 positon) {
        Vector2Int gridCoords = WorldToGrid(positon);
        return PositionIsOccupied(gridCoords.x, gridCoords.y);
    }

    public T GetObjectAt(int x, int y) {
        if (!CoordinatesAreValid(x, y)) {
            Debug.Log(string.Format("[{0},{1}] is outside the bounds of the grid", x, y));
            return default(T);
        }
        return gridCells[x, y];
    }

    public void SetObjectAt(int x, int y, T newObject) {
        if (!CoordinatesAreValid(x,y)) {
            Debug.Log(string.Format("[{0},{1}] is outside the bounds of the grid", x, y));
            return;
        }
        gridCells[x, y] = newObject;
    }

    public T RemoveObjectAt(int x, int y) {
        T obj = gridCells[x, y];
        gridCells[x, y] = default(T);
        return obj;
    }

    public Vector2Int WorldToGrid(Vector3 worldPos) {
        Vector3 adjusted = Quaternion.Inverse(gridRotation) * worldPos - Origin;
        int gridX = (int)(adjusted.x / CellSize);
        int gridY = (int)(adjusted.z / CellSize);

        return new Vector2Int(gridX, gridY);
    }

    public Vector3 GridToWorld(int x, int y) {
        return gridRotation * (new Vector3(x, 0, y) * CellSize) + Origin;
    }

    public Vector3 GridCellCenterWorldPos(int x, int y) {
        return GridToWorld(x, y) + gridRotation * (new Vector3(CellSize, 0, CellSize) / 2);
    }

    public Vector3 CellCenterFromWorldPos(Vector3 position) {
        Vector2Int gridPos = WorldToGrid(position);
        return GridCellCenterWorldPos(gridPos.x, gridPos.y);
    }

    private bool CoordinatesAreValid(int x, int y) {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }
}
