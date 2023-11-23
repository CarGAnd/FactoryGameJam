using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Grid<T> {

    [field: SerializeField] public int Width { get; private set; }
    [field: SerializeField] public int Height { get; private set; }
    [field: SerializeField] public float CellSize { get; private set; }
    [field: SerializeField] public Vector3 Origin { get; private set; }

    private T[,] gridCells;

    public Grid(int width, int height, Vector3 origin, float cellSize = 1) {
        this.Width = width;
        this.Height = height;
        this.CellSize = cellSize;
        this.Origin = origin;
        gridCells = new T[width, height];
    }

    public bool PositionIsOccupied(int x, int y) {
        return gridCells[x, y] != null;
    }

    public T GetObjectAtCoordinates(int x, int y) {
        return gridCells[x, y];
    }

    public void SetObjectAtCoordinates(int x, int y, T newObject) {
        gridCells[x, y] = newObject;
    }

    public Vector2Int WorldToGrid(Vector3 worldPos) {
        Vector3 adjusted = worldPos - Origin;
        int gridX = (int)(adjusted.x / CellSize);
        int gridY = (int)(adjusted.z / CellSize);

        return new Vector2Int(gridX, gridY);
    }

    public Vector3 GridToWorld(int x, int y) {
        return new Vector3(x, 0, y) * CellSize + Origin;
    }

    public Vector3 GridCellCenterWorldPos(int x, int y) {
        return GridToWorld(x, y) + new Vector3(CellSize, 0, CellSize) / 2;
    }

    public Vector3 GridCellCenterWorldPos(Vector3 position) {
        Vector2Int gridPos = WorldToGrid(position);
        return GridCellCenterWorldPos(gridPos.x, gridPos.y);
    }

    private bool CoordinatesAreValid(int x, int y) {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }
}
