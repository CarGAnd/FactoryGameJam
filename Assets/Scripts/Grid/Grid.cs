using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Grid : MonoBehaviour {

    [HideInInspector] public UnityEvent gridUpdated;

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

    // A IGridObject has a place on grid method, which calls this one.
    // Shape Layout represents the cells in addition to the center or start cell in relative coordinates to the start cell.
    public void PlaceObject(IGridObject gridObject, Vector2Int startCell, List<Vector2Int> shapeLayout = null) {
        Cell firstCell = cells[startCell.y, startCell.x];
        firstCell.SetOccupyingObject(gridObject);

        if(shapeLayout == null) {
            return;
        }

        foreach (Vector2Int deltaCoord in shapeLayout) {
            Vector2Int coord = startCell + deltaCoord;
            if (CellWithinBounds(coord)) {
                Cell cell = cells[coord.y, coord.x];
                cell.SetOccupyingObject(gridObject);
            }
        }
    }

    public void RemoveObject(Vector2Int coord) {
        Cell firstCell = cells[coord.y, coord.x];
        firstCell.RemoveOccupyingObject();

        /*if (shapeLayout == null) {
            return;
        }

        foreach (Vector2Int deltaCoord in shapeLayout) {
            Vector2Int coord = startCell + deltaCoord;
            if (CellWithinBounds(coord.y, coord.x)) {
                Cell cell = cells[coord.y, coord.x];
                cell.SetOccupyingObject(gridObject);
            }
        }*/
    }

    public bool PositionIsOccupied(Vector2Int coord) {
        return cells[coord.y, coord.x].IsOccupied();
    }

    public bool PositionIsOccupied(Vector3 worldPos) {
        Vector2Int cellCoords = GetCellCoords(worldPos);
        return PositionIsOccupied(cellCoords);
    }

    // Cell via world position
    public Vector2Int GetCellCoords(Vector3 worldPosition) {
        Vector3 adjusted = Quaternion.Inverse(Rotation) * worldPosition - Origin;
        Vector2Int cellCoordinates = layout.GetCellCoordinate(Vector3.Scale(adjusted, new Vector3(1f /  CellSize.x, 1, 1f / CellSize.y)));
        return new Vector2Int(cellCoordinates.x, cellCoordinates.y);
    }

    public Vector3 GetCellCenter(Vector2Int cellCoord) {
        Vector3 normalizedPosition = layout.GetCellCenter(cellCoord);
        return Rotation * new Vector3(normalizedPosition.x * CellSize.x, 0, normalizedPosition.z * CellSize.y) + Origin;
    }

    public Vector3 GetCellCenter(Vector3 worldPosition) {
        Vector2Int cellCoords = GetCellCoords(worldPosition);
        Vector3 normalizedPosition = layout.GetCellCenter(cellCoords);
        return Rotation * new Vector3(normalizedPosition.x * CellSize.x, 0, normalizedPosition.z * CellSize.y) + Origin;
    }

    // World position via cell
    public Vector3 GetCellWorldPosition(Vector2Int cellCoord) {
        Vector3 normalizedPosition = layout.CalculateCellPosition(cellCoord);
        Vector3 worldPosition = Rotation * new Vector3(normalizedPosition.x * CellSize.x, 0, normalizedPosition.z * CellSize.y) + Origin;
        return worldPosition;
    }

    public void ResizeGrid(int newRows, int newColumns) {
        // Implementation to resize the grid.
        // Remember to preserve existing cells' data
        UpdateCellPositions();
    }

    public void MoveGrid(Vector3 newOrigin) {
        Origin = newOrigin;
        UpdateCellPositions();
    }

    public void RotateGrid(Quaternion newRotation) {
        Rotation = newRotation;
        UpdateCellPositions();
    }

    public void AdjustCellSize(float length, float width) {
        CellSize = new Vector2(width, length);
        UpdateCellPositions();
    }

    private void UpdateCellPositions() {
        for (int y = 0; y < cells.GetLength(0); y++) {
            for (int x = 0; x < cells.GetLength(1); x++) {
                cells[y, x].UpdatePosition();
            }
        }
        gridUpdated?.Invoke();
    }

    public bool CellWithinBounds(Vector2Int cellCoord) {
        return cellCoord.y >= 0 && cellCoord.y < Rows && cellCoord.x >= 0 && cellCoord.x < Columns;
    }

    public List<Vector2Int> GetCellNeighbors(Vector2Int cellCoord) {
        return layout.GetCellNeighbors(cellCoord);
    }

    public List<Vector2Int> FindPathAsCoordinates(Vector2Int startCell, Vector2Int endCell) {
        //Uses BFS for now
        //TODO: implement A*
        return new BFSPathFind().FindPathAsCoordinates(this, startCell, endCell);
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

    public string GetID() {
        return id;
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

public class BFSPathFind {
    public List<Vector2Int> FindPathAsCoordinates(Grid grid, Vector2Int startCell, Vector2Int endCell) {
        if (startCell == endCell) {
            //if start and end are the same the path is just an empty list
            return new List<Vector2Int>();
        }

        Queue<PathCell> frontier = new Queue<PathCell>();
        frontier.Enqueue(new PathCell(null, startCell));
        List<Vector2Int> visited = new List<Vector2Int>();

        while (frontier.Count > 0) {
            PathCell currentCell = frontier.Dequeue();
            Vector2Int currentCoord = currentCell.coord;
            List<Vector2Int> neighbors = grid.GetCellNeighbors(currentCoord);
            foreach (Vector2Int coord in neighbors) {
                if(!grid.CellWithinBounds(coord) || grid.PositionIsOccupied(coord)) {
                    continue;
                }
                PathCell cell = new PathCell(currentCell, coord);
                if (coord == endCell) {
                    return GetPath(cell);
                }
                else if (!visited.Contains(coord)) {
                    frontier.Enqueue(cell);
                    visited.Add(coord);
                }
            }
        }
        //no path possible
        return null;
    }

    private List<Vector2Int> GetPath(PathCell endCell) {
        List<Vector2Int> pathList = new List<Vector2Int>();
        while(endCell != null) {
            pathList.Add(endCell.coord);
            endCell = endCell.prevCell;
        }
        pathList.Reverse();
        return pathList;
    }

    private class PathCell {
        public PathCell prevCell;
        public Vector2Int coord;

        public PathCell(PathCell prevCell, Vector2Int coord) {
            this.prevCell = prevCell;
            this.coord = coord;
        }
    }
}
