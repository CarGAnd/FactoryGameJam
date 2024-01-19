using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This is mainly just a wrapper class for GridOfObjects + GridLayout, as they are usually used together
public class FactoryGrid : MonoBehaviour, ISearchable
{
    [field: SerializeField] public int Rows { get; private set; }
    [field: SerializeField] public int Columns { get; private set; }

    public Quaternion Rotation { get { return gridLayout.Rotation; } }
    public Vector2 CellSize { get { return gridLayout.CellSize; } }
    public Vector3 Origin { get { return gridLayout.Origin; } }

    [SerializeField] private GridLayout gridLayout;

    private CellGrid<IGridObject> placementGrid;

    private void Awake() {
        placementGrid = new CellGrid<IGridObject>(Columns, Rows);
    }
 
    public void PlaceObject(IGridObject gridObject, Vector2Int startCell, List<Vector2Int> shapeLayout) {
        placementGrid.PlaceObject(gridObject, startCell, shapeLayout);    
    }

    public void PlaceObject(IGridObject gridObject, Vector2Int coordinate) {
        placementGrid.PlaceObject(gridObject, coordinate);    
    }

    public void RemoveObject(Vector2Int coord) {
        placementGrid.RemoveObject(coord);
    }
    
    public void MoveObject(Vector2Int from, Vector2Int to) {
        placementGrid.MoveObject(from, to);
    }

    public IGridObject GetObjectAt(Vector2Int coord) {
        return placementGrid.GetObjectAt(coord);
    }

    public V GetObjectAsType<V>(Vector2Int coord) {
        return placementGrid.GetObjectAsType<V>(coord);    
    }

    public List<Vector2Int> GetSharedPositions(Vector2Int coord) {
        return placementGrid.GetSharedPositions(coord);    
    }

    public Vector2Int GetObjectOriginCoord(Vector2Int coord) {
        return placementGrid.GetObjectOriginCoord(coord);
    }

    public bool PositionIsOccupied(Vector2Int coord) {
        return placementGrid.PositionIsOccupied(coord);
    }

    public bool PositionIsOccupied(Vector3 worldPos) {
        Vector2Int cellCoords = gridLayout.GetCellCoords(worldPos);
        return PositionIsOccupied(cellCoords);
    }

    public bool AllPositionsAreFree(List<Vector2Int> positions) {
        return placementGrid.AllPositionsAreFree(positions);    
    }
    
    public bool CellWithinBounds(Vector2Int cellCoord) {
        return placementGrid.CellWithinBounds(cellCoord);
    }

    public List<Vector2Int> GetNeighbors(Vector2Int coord) {
        return placementGrid.GetNeighbors(coord);
    }

    public Path FindPath(Vector2Int startCoord, Vector2Int endCoord) {
        return placementGrid.FindPath(startCoord, endCoord);
    }

    public Vector2Int FindClosestUnoccupiedCell(Vector2Int startCoord) {
        return placementGrid.FindClosestUnoccupiedCell(startCoord);    
    }

    public Vector2Int GetCellCoords(Vector3 worldPosition) {
        return gridLayout.GetCellCoords(worldPosition);
    }

    public Vector3 GetCellCenter(Vector2Int cellCoord) {
        return gridLayout.GetCellCenter(cellCoord);    
    }

    public Vector3 GetCellCenter(Vector3 worldPosition) {
        Vector2Int cellCoords = GetCellCoords(worldPosition);
        return GetCellCenter(cellCoords);
    }

    public Vector3 GetCellWorldPosition(Vector2Int cellCoord) {
        return gridLayout.GetCellWorldPosition(cellCoord);    
    }

    public Vector3 GetCellWorldPosition(Vector3 worldPosition) {
        return gridLayout.GetCellWorldPosition(worldPosition);    
    }

    public Vector3 GetSubgridCenter(Vector2Int bottomLeft, Vector2Int dimensions) {
        return gridLayout.GetSubgridCenter(bottomLeft, dimensions);
    }

    public Vector2Int GetSubgridOriginCoord(Vector3 subgridCenter, Vector2Int subgridDimensions) {
        return gridLayout.GetSubgridOriginCoord(subgridCenter, subgridDimensions);
    }

    public List<Vector2Int> GetPositionsInSubgrid(Vector2Int lowerLeft, Vector2Int subgridDimensions) {
        return gridLayout.GetPositionsInSubgrid(lowerLeft, subgridDimensions);
    }

    #region Debugging
    [SerializeField] private bool showOccupiedCells = true;
    [SerializeField] private bool showGridLines = true;

    private void OnDrawGizmos() {
        if (showOccupiedCells && placementGrid != null) {
            Gizmos.color = new Color(1, 0, 0, 0.5f);
            for (int y = 0; y < placementGrid.Rows; y++) {
                for (int x = 0; x < placementGrid.Columns; x++) {
                    Vector2Int position = new Vector2Int(x, y);
                    if (placementGrid.PositionIsOccupied(position)) {
                        Vector3 pos = gridLayout.GetCellCenter(position);
                        Vector3 cubeSize = new Vector3(1, 0.01f, 1) * Mathf.Min(gridLayout.CellSize.x, gridLayout.CellSize.y);
                        Gizmos.DrawCube(pos, cubeSize);
                    }
                }
            }
        } 

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
    }
    #endregion
}
