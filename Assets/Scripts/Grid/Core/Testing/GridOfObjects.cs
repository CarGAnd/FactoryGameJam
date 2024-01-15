using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridOfObjects<T> : MonoBehaviour, ISearchable
{
    public int Columns { get { return gridSize.Columns; } }
    public int Rows { get { return gridSize.Rows; } }
    [SerializeField] private GridLayout gridLayout;

    private Cell[,] cells;
    private GridSize gridSize;

    private void Awake() {
        CreateGrid();
    }

     // Shape Layout represents the cells in addition to the center or start cell in relative coordinates to the start cell.
    public void PlaceObject(IGridObject gridObject, Vector2Int startCell, List<Vector2Int> shapeLayout) {
        List<Cell> sharedCells = new List<Cell>();
        
        if(shapeLayout != null) {
            foreach (Vector2Int deltaCoord in shapeLayout) {
                Vector2Int coord = startCell + deltaCoord;
                if (CellWithinBounds(coord)) {
                    Cell cell = GetCellAt(coord);
                    sharedCells.Add(cell);
                }
            }
        }

        foreach (Cell c in sharedCells) {
            c.SetOccupyingObject(gridObject, startCell, sharedCells);
        }
    }

    public void PlaceObject(IGridObject gridObject, Vector2Int coordinate) {
        Cell cell = GetCellAt(coordinate);
        cell.SetOccupyingObject(gridObject);
    }

    public void RemoveObject(Vector2Int coord) {
        Cell firstCell = GetCellAt(coord);
        List<Cell> sharedCells = firstCell.GetSharedCells();

        if(sharedCells == null) {
            firstCell.RemoveOccupyingObject();
        }
        else {
            foreach (Cell c in sharedCells) {
                c.RemoveOccupyingObject();
            }
        }
    }
    
    public void MoveObject(Vector2Int from, Vector2Int to) {
        IGridObject gridObject = GetObjectAt(from);
        Cell fromCell = GetCellAt(from);
        List<Cell> sharedCells = fromCell.GetSharedCells();
        if(sharedCells == null) {
            sharedCells = new List<Cell>() { fromCell };
        }
        Vector2Int objectOrigin = fromCell.GetOccupyingObjectOrigin();
        List<Vector2Int> objectLayout = new List<Vector2Int>();
        foreach(Cell c in sharedCells) {
            Vector2Int originDelta = c.GetCellCoordinates() - objectOrigin;
            objectLayout.Add(originDelta);
        }
        RemoveObject(from);
        PlaceObject(gridObject, to, objectLayout);
    }

    public IGridObject GetObjectAt(Vector2Int coord) {
        if (CellWithinBounds(coord)) {
            return GetCellAt(coord).GetOccupyingObject();
        }
        else {
            return default(IGridObject);
        }   
    }

    public V GetObjectAsType<V>(Vector2Int coord) {
        IGridObject obj = GetObjectAt(coord);
        if(obj is V v) {
            return v;
        }
        else {
            return default(V);
        }
    }

    public List<Vector2Int> GetSharedPositions(Vector2Int coord) {
        List<Vector2Int> sharedPositions = new List<Vector2Int>();
        List<Cell> sharedCells = GetCellAt(coord).GetSharedCells();
        foreach(Cell c in sharedCells) {
            sharedPositions.Add(c.GetCellCoordinates());
        }
        return sharedPositions;
    }

    public Vector2Int GetObjectOriginCoord(Vector2Int coord) {
        return GetCellAt(coord).GetOccupyingObjectOrigin();
    }

    public bool PositionIsOccupied(Vector2Int coord) {
        if (CellWithinBounds(coord)) {
            return GetCellAt(coord).IsOccupied();
        }
        else {
            return false;
        }     
    }

    public bool PositionIsOccupied(Vector3 worldPos) {
        Vector2Int cellCoords = gridLayout.GetCellCoords(worldPos);
        return PositionIsOccupied(cellCoords);
    }

    public bool AllPositionsAreFree(List<Vector2Int> positions) {
        foreach(Vector2Int position in positions) {
            if (PositionIsOccupied(position)) {
                return false;
            }
        }
        return true;
    }
    
    public bool CellWithinBounds(Vector2Int cellCoord) {
        return cellCoord.y >= 0 && cellCoord.y < Rows && cellCoord.x >= 0 && cellCoord.x < Columns;
    }

    public List<Vector2Int> GetNeighbors(Vector2Int coord) {
        return gridLayout.GetCellNeighbors(coord);
    }

    private Cell GetCellAt(Vector2Int coord) {
        return cells[coord.y, coord.x];
    }

    public Path FindPath(Vector2Int startCoord, Vector2Int endCoord) {
        List<Vector2Int> positions = null;
        
        if(PositionIsOccupied(endCoord)) {
            return new Path(positions);
        }
        
        positions = GridBFS.FindPath(this, startCoord, (Vector2Int coord) => coord == endCoord, (Vector2Int coord) => CellWithinBounds(coord) && !PositionIsOccupied(coord));
        return new Path(positions);
    }

    public Vector2Int FindClosestUnoccupiedCell(Vector2Int startCoord) {
        List<Vector2Int> path = GridBFS.FindPath(this, startCoord, (Vector2Int coord) => !PositionIsOccupied(coord), (Vector2Int coord) => CellWithinBounds(coord));
        return path[path.Count - 1];
    }

    private void CreateGrid() {
        gridSize = GetComponent<GridSize>();
        cells = new Cell[Rows, Columns];
        for (int y = 0; y < cells.GetLength(0); y++) {
            for (int x = 0; x < cells.GetLength(1); x++) {
                //cells[y, x] = new Cell(y, x, this);
            }
        }
    }

    #region Debugging
    [SerializeField] private bool showOccupiedCells = true;

    private void OnDrawGizmos() {
        gridLayout = GetComponent<GridLayout>();
        gridSize = GetComponent<GridSize>();

        if (showOccupiedCells && cells != null) {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            for (int y = 0; y < cells.GetLength(0); y++) {
                for (int x = 0; x < cells.GetLength(1); x++) {
                    if (cells[y, x].IsOccupied()) {
                        Vector3 pos = gridLayout.GetCellCenter(new Vector2Int(x, y));
                        Vector3 cubeSize = new Vector3(1, 0.01f, 1) * Mathf.Min(gridLayout.CellSize.x, gridLayout.CellSize.y);
                        Gizmos.DrawCube(pos, cubeSize);
                    }
                }
            }
        } 
    }
    #endregion
}
