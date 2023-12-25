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

    public float TotalGridWidth { get { return Columns * CellSize.x; } }
    public float TotalGridHeight { get { return Rows * CellSize.y; } }

    private Vector3 RotationPivot { get { return new Vector3(TotalGridWidth, 0, TotalGridHeight) / 2f; } }

    private IGridLayout layout = new SquareGridLayout();
    private string id;
    private Cell[,] cells;
    //private PathfindingAlgorithm pathfindingAlgorithm;

    private void Awake() {
        CreateGrid();
    }

    // A IGridObject has a place on grid method, which calls this one.
    // Shape Layout represents the cells in addition to the center or start cell in relative coordinates to the start cell.
    public void PlaceObject(IGridObject gridObject, Vector2Int startCell, List<Vector2Int> shapeLayout = null) {
        List<Cell> sharedCells = new List<Cell>();
        Cell firstCell = cells[startCell.y, startCell.x];
        sharedCells.Add(firstCell);
        
        if(shapeLayout != null) {
            foreach (Vector2Int deltaCoord in shapeLayout) {
                Vector2Int coord = startCell + deltaCoord;
                if (CellWithinBounds(coord)) {
                    Cell cell = cells[coord.y, coord.x];
                    sharedCells.Add(cell);
                }
            }
        }

        foreach (Cell c in sharedCells) {
            c.SetOccupyingObject(gridObject);
            c.SetSharedCells(sharedCells);
        }
    }

    public void RemoveObject(Vector2Int coord) {
        Cell firstCell = cells[coord.y, coord.x];
        List<Cell> sharedCells = firstCell.GetSharedCells();

        foreach (Cell c in sharedCells) {
            c.RemoveOccupyingObject();
        }
    }

    public IGridObject GetObjectAt(Vector2Int coord) {
        if (CellWithinBounds(coord)) {
            return cells[coord.y, coord.x].GetOccupyingObject();
        }
        else {
            return null;
        }   
    }

    public bool PositionIsOccupied(Vector2Int coord) {
        if (CellWithinBounds(coord)) {
            return cells[coord.y, coord.x].IsOccupied();
        }
        else {
            return false;
        }     
    }

    public bool PositionIsOccupied(Vector3 worldPos) {
        Vector2Int cellCoords = GetCellCoords(worldPos);
        return PositionIsOccupied(cellCoords);
    }

    // Cell via world position
    public Vector2Int GetCellCoords(Vector3 worldPosition) {
        Vector3 normalizedPosition = RemoveScaleRotationOffset(worldPosition);
        //The grid now has origin at 0,0 with a rotation of 0 and a cellsize of 1x1
        Vector2Int cellCoordinates = layout.GetCellCoordinate(normalizedPosition);
        return cellCoordinates;
    }

    public Vector3 GetCellCenter(Vector2Int cellCoord) {
        //Calculate the position in a grid with no rotation, the origin at (0,0), and a cellsize of 1x1
        Vector3 normalizedPosition = layout.GetCellCenter(cellCoord);
        //Apply scale, rotation and offset to the position
        Vector3 worldPosition = ApplyScaleRotationOffset(normalizedPosition);
        return worldPosition;
    }

    public Vector3 GetCellCenter(Vector3 worldPosition) {
        Vector2Int cellCoords = GetCellCoords(worldPosition);
        return GetCellCenter(cellCoords);
    }

    // World position via cell
    public Vector3 GetCellWorldPosition(Vector2Int cellCoord) {
        //Calculate the position in a grid with no rotation, the origin at (0,0), and a cellsize of 1x1
        Vector3 normalizedPosition = layout.CalculateCellPosition(cellCoord);
        //Apply scale, rotation and offset to the position
        Vector3 worldPosition = ApplyScaleRotationOffset(normalizedPosition);
        return worldPosition;
    }

    private Vector3 ApplyScaleRotationOffset(Vector3 normalizedPosition) {
        //Scale the grid to have the correct cellsize
        Vector3 scaledWorldPosition = new Vector3(normalizedPosition.x * CellSize.x, 0, normalizedPosition.z * CellSize.y);
        //Move the grid so that the rotation pivot is at 0,0
        Vector3 pivotRelative = scaledWorldPosition - RotationPivot;
        //Rotate the grid
        Vector3 rotatedGrid = Rotation * pivotRelative;
        //Move the grid to the correct offset
        Vector3 offsetGrid = rotatedGrid + Origin + RotationPivot;
        return offsetGrid;
    }

    private Vector3 RemoveScaleRotationOffset(Vector3 worldPosition) {
        //Move grid so rotation Pivot is at 0,0
        Vector3 pivotRelative = worldPosition - RotationPivot - Origin;
        //Rotate the grid around 0,0 with the inverse of the grids rotation, giving the resulting grid a rotation of 0
        Vector3 unrotatedGrid = Quaternion.Inverse(Rotation) * pivotRelative;
        //Move the grid to have the origin at 0,0
        Vector3 offsetGrid = unrotatedGrid + RotationPivot;
        //Scale the grid to have a cellsize of 1x1
        Vector3 normalizedPosition = Vector3.Scale(offsetGrid, new Vector3(1f / CellSize.x, 1, 1f / CellSize.y));
        return normalizedPosition;
    }

    //Get the origin position of the cell that a given position is in
    public Vector3 GetCellWorldPosition(Vector3 worldPosition) {
        Vector2Int cellCoord = GetCellCoords(worldPosition);
        return GetCellWorldPosition(cellCoord);
    }

    public Vector3 GetSubgridCenter(Vector2Int bottomLeft, Vector2Int dimensions) {
        Vector3 bottomLeftPos = layout.CalculateCellPosition(bottomLeft);
        Vector3 normalizedCenter = bottomLeftPos + new Vector3(dimensions.x, 0, dimensions.y) / 2f;
        Vector3 worldPos = ApplyScaleRotationOffset(normalizedCenter);
        return worldPos;
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

    [Button("Destroy Grid", ButtonSizes.Medium)]
    private void DestroyGrid() {
        for (int i = transform.childCount - 1; i >= 0; i--) {
            DestroyImmediate(transform.GetChild(i).gameObject);
        }
        cells = null;
    }


    [Button("Create Grid", ButtonSizes.Medium)]
    private void CreateGrid() {
        DestroyGrid();
        cells = new Cell[Rows, Columns];
        for (int y = 0; y < cells.GetLength(0); y++) {
            for (int x = 0; x < cells.GetLength(1); x++) {
                cells[y, x] = new Cell(y, x, this, cellPrefab);
            }
        }
    }

    #region Debugging
    [FoldoutGroup("Debug")]
    [SerializeField] private bool showGridLines = true;
    [FoldoutGroup("Debug")]
    [SerializeField] private bool showOccupiedCells = true;

    private void OnDrawGizmos() {
        if (showGridLines) {
            Gizmos.color = Color.green;
            for (int y = 0; y < Rows + 1; y++) {
                Vector3 start = GetCellWorldPosition(new Vector2Int(0, y));
                Vector3 end = GetCellWorldPosition(new Vector2Int(Columns, y));
                Gizmos.DrawLine(start, end);
            }

            for (int x = 0; x < Columns + 1; x++) {
                Vector3 start = GetCellWorldPosition(new Vector2Int(x, 0));
                Vector3 end = GetCellWorldPosition(new Vector2Int(x, Rows));
                Gizmos.DrawLine(start, end);
            }
        }

        if (showOccupiedCells && cells != null) {
            Gizmos.color = Color.red;
            for (int y = 0; y < cells.GetLength(0); y++) {
                for (int x = 0; x < cells.GetLength(1); x++) {
                    if (cells[y, x].IsOccupied()) {
                        Vector3 pos = GetCellCenter(new Vector2Int(x, y));
                        Gizmos.DrawWireSphere(pos, Mathf.Min(CellSize.x, CellSize.y) / 3f);
                    }
                }
            }
        } 
    }
    #endregion
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
