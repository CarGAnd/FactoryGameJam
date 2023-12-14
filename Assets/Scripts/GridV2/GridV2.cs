using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

public class GridV2 : MonoBehaviour {

    [field: SerializeField] public int Columns { get; private set; }
    [field: SerializeField] public int Rows { get; private set; }
    [field: SerializeField] public Vector3 Origin { get; private set; }
    [field: SerializeField] public Quaternion Rotation { get; private set; }
    [field: SerializeField] public Vector2 CellSize { get; private set; }

    [SerializeField] private GameObject cellPrefab;

    private IGridLayout layout;
    private string id;
    private CellV2[,] cells;
    //private PathfindingAlgorithm pathfindingAlgorithm;

    private Transform gridParent;

    private void Awake() {
        CreateGrid();
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
        Vector3 adjusted = Quaternion.Inverse(Rotation) * worldPosition - Origin;
        Vector2Int cellCoordinates = layout.GetCellCoordinate(adjusted);

        return GetCell(cellCoordinates.y, cellCoordinates.x);
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

    private bool CellWithinBounds(int row, int column) 
    {
        return row >= 0 && row < Rows && column >= 0 && column < Columns;
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


    [Button("Destroy Grid")]
    private void DestroyGrid() {
        gridParent = transform;
        for (int i = gridParent.childCount - 1; i >= 0; i--) {
            DestroyImmediate(gridParent.GetChild(i).gameObject);
        }
        cells = null;
    }


    [Button("Create Grid")]
    private void CreateGrid() {
        DestroyGrid();
        layout = new SquareGridLayout();
        gridParent = transform;
        cells = new CellV2[Rows, Columns];
        for(int y = 0; y < cells.GetLength(0); y++) {
            for(int x = 0; x < cells.GetLength(1); x++) {
                cells[y, x] = new CellV2(y, x, this, cellPrefab, gridParent);
            }
        }
    }
}
