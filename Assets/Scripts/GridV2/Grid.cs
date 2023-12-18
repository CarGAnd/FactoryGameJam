using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour {

    [field: SerializeField] public int Columns { get; private set; }
    [field: SerializeField] public int Rows { get; private set; }
    [field: SerializeField] public Vector3 Origin { get; private set; }
    [field: SerializeField] public Quaternion Rotation { get; private set; }
    [field: SerializeField] public Vector2 CellSize { get; private set; }

    [SerializeField] private GameObject cellPrefab;

    private IGridLayout layout;
    private string id;
    private Cell[,] cells;
    //private PathfindingAlgorithm pathfindingAlgorithm;

    private void Awake() {
        CreateGrid();
    }

    public string GetID() {
        return id;
    }

    // A IGridObject has a place on grid method, which calls this one.
    // Shape Layout represents the cells in addition to the center or start cell in relative coordinates to the start cell.
    public void PlaceObject(IGridObject gridObject, Cell startCell, List<Vector2Int> shapeLayout = null) {
        startCell.SetOccupyingObject(gridObject);

        if(shapeLayout== null) {
            return;
        }

        foreach (Vector2Int deltaCoord in shapeLayout) {
            Vector2Int coord = startCell.GetCellCoordinates() + deltaCoord;
            if (CellWithinBounds(coord.y, coord.x)) {
                Cell cell = cells[coord.y, coord.x];
                cell.SetOccupyingObject(gridObject);
            }
        }
    }

    public bool PositionIsOccupied(int row, int column) {
        return cells[row, column].GetOccupyingObject() != null;
    }

    public bool PositionIsOccupied(Vector3 worldPos) {
        Vector2Int cellCoords = GetCell(worldPos).GetCellCoordinates();
        return PositionIsOccupied(cellCoords.y, cellCoords.x);
    }

    public void RemoveObject(Cell cell) {
        PlaceObject(null, cell, null);
    }

    public void ResizeGrid(int newRows, int newColumns) {
        // Implementation to resize the grid.
        // Remember to preserve existing cells' data
    }

    // Cell via coordinates
    public Cell GetCell(int row, int column) {
        if (CellWithinBounds(row, column)) {
            return cells[row, column];
        }
        else {
            return null;
        }
    }

    // Cell via world position
    public Cell GetCell(Vector3 worldPosition) {
        Vector3 adjusted = Quaternion.Inverse(Rotation) * worldPosition - Origin;
        Vector2Int cellCoordinates = layout.GetCellCoordinate(Vector3.Scale(adjusted, new Vector3(1f /  CellSize.x, 1, 1f / CellSize.y)));
        return GetCell(cellCoordinates.y, cellCoordinates.x);
    }

    public Vector3 GetCellCenter(int row, int column) {
        Vector3 normalizedPosition = layout.GetCellCenter(row, column);
        return Rotation * new Vector3(normalizedPosition.x * CellSize.x, 0, normalizedPosition.z * CellSize.y) + Origin;
    }

    // World position via cell
    public Vector3 CalculateCellPosition(int row, int column) {
        Vector3 normalizedPosition = layout.CalculateCellPosition(row, column);
        Vector3 worldPosition = Rotation * new Vector3(normalizedPosition.x * CellSize.x, 0, normalizedPosition.z * CellSize.y) + Origin;
        return worldPosition;
    }

    public void MoveGrid(Vector3 newOrigin) {
        Origin = newOrigin;
    }

    public void RotateGrid(Quaternion newRotation) {
        Rotation = newRotation;
    }

    public void AdjustCellSize(float length, float width) {
        CellSize = new Vector2(width, length);
    }

    private bool CellWithinBounds(int row, int column) {
        return row >= 0 && row < Rows && column >= 0 && column < Columns;
    }

    public List<Cell> FindPathAsCells(Cell startCell, Cell endCell) {
        throw new System.NotImplementedException();
    }

    public List<Vector2Int> FindPathAsCoordinates(Cell startCell, Cell endCell) {
        throw new System.NotImplementedException();
    }

    public void SaveGrid() {
        // Implementation to save grid state
        // This is meant to save the grid data.
    }

    public void LoadGrid() {
        // Implementation to load grid state
        // This is meant to load the grid data.
    }

    public void VisualizeGrid() {
        // Implementation for grid visualization
        // might be a different class, might be gizmos, not sure.
    }


    [Button("Destroy Grid")]
    private void DestroyGrid() {
        for (int i = transform.childCount - 1; i >= 0; i--) {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        cells = null;
    }


    [Button("Create Grid")]
    private void CreateGrid() {
        DestroyGrid();
        layout = new SquareGridLayout();
        cells = new Cell[Rows, Columns];
        for (int y = 0; y < cells.GetLength(0); y++) {
            for (int x = 0; x < cells.GetLength(1); x++) {
                cells[y, x] = new Cell(y, x, this, cellPrefab);
            }
        }
    }
}
