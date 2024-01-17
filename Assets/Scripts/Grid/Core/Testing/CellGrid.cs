using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellGrid<T> : ISearchable
{
    public int Columns { get; private set; }
    public int Rows { get; private set; }

    private GenericCell<T>[,] cells;

    public CellGrid(int numColumns, int numRows) {
        this.Columns = numColumns;
        this.Rows = numRows;
        CreateGrid(numColumns, numRows);
    }

    // Shape Layout represents the cells in addition to the center or start cell in relative coordinates to the start cell.
    public void PlaceObject(T gridObject, Vector2Int startCell, List<Vector2Int> shapeLayout) {
        List<GenericCell<T>> sharedCells = new List<GenericCell<T>>();
        
        if(shapeLayout != null) {
            foreach (Vector2Int deltaCoord in shapeLayout) {
                Vector2Int coord = startCell + deltaCoord;
                if (CellWithinBounds(coord)) {
                    GenericCell<T> cell = GetCellAt(coord);
                    sharedCells.Add(cell);
                }
            }
        }

        foreach (GenericCell<T> c in sharedCells) {
            c.SetOccupyingObject(gridObject, startCell, sharedCells);
        }
    }

    public void PlaceObject(T gridObject, Vector2Int coordinate) {
        GenericCell<T> cell = GetCellAt(coordinate);
        cell.SetOccupyingObject(gridObject);
    }

    public void RemoveObject(Vector2Int coord) {
        GenericCell<T> firstCell = GetCellAt(coord);
        List<GenericCell<T>> sharedCells = firstCell.GetSharedCells();

        if(sharedCells == null) {
            firstCell.RemoveOccupyingObject();
        }
        else {
            foreach (GenericCell<T> c in sharedCells) {
                c.RemoveOccupyingObject();
            }
        }
    }
    
    public void MoveObject(Vector2Int from, Vector2Int to) {
        T gridObject = GetObjectAt(from);
        GenericCell<T> fromCell = GetCellAt(from);
        List<GenericCell<T>> sharedCells = fromCell.GetSharedCells();
        if(sharedCells == null) {
            sharedCells = new List<GenericCell<T>>() { fromCell };
        }
        Vector2Int objectOrigin = fromCell.GetOccupyingObjectOrigin();
        List<Vector2Int> objectLayout = new List<Vector2Int>();
        foreach(GenericCell<T> c in sharedCells) {
            Vector2Int originDelta = c.GetCellCoordinates() - objectOrigin;
            objectLayout.Add(originDelta);
        }
        RemoveObject(from);
        PlaceObject(gridObject, to, objectLayout);
    }

    public T GetObjectAt(Vector2Int coord) {
        if (CellWithinBounds(coord)) {
            return GetCellAt(coord).GetOccupyingObject();
        }
        else {
            return default(T);
        }   
    }

    public V GetObjectAsType<V>(Vector2Int coord) {
        T obj = GetObjectAt(coord);
        if(obj is V v) {
            return v;
        }
        else {
            return default(V);
        }
    }

    public List<Vector2Int> GetSharedPositions(Vector2Int coord) {
        List<Vector2Int> sharedPositions = new List<Vector2Int>();
        List<GenericCell<T>> sharedCells = GetCellAt(coord).GetSharedCells();
        foreach(GenericCell<T> c in sharedCells) {
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
        Vector2Int[] directions = new Vector2Int[] { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };
        List<Vector2Int> neighbors = new List<Vector2Int>();
        foreach(Vector2Int direction in directions) {
            Vector2Int neighborValue = coord + direction;
            if (CellWithinBounds(neighborValue)) {
                neighbors.Add(neighborValue);
            }
        }
        return neighbors;
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

    private GenericCell<T> GetCellAt(Vector2Int coord) {
        return cells[coord.y, coord.x];
    }

    private void CreateGrid(int columns, int rows) {
        cells = new GenericCell<T>[rows, columns];
        for (int y = 0; y < rows; y++) {
            for (int x = 0; x < columns; x++) {
                cells[y, x] = new GenericCell<T>(y, x, this);
            }
        }
    }
}
