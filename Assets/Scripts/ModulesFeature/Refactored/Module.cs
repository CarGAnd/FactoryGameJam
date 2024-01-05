using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Module : MonoBehaviour, IGridObject
{
    [SerializeField] private GridObjectSO moduleData;

    private Vector2Int gridPosition;
    private Grid grid;
    private int numRotations;

    public void SetInitInfo(int numRotations) {
        this.numRotations = numRotations;
    }

    public T GetObject<T>() {
        throw new System.NotImplementedException();
    }

    public List<Vector2Int> GetOccupyingCells(Vector2Int startCell, Grid grid) {
        throw new System.NotImplementedException();
    }

    public List<Vector2Int> GetShapeLayout() {
        return moduleData.GetLayoutShape(numRotations);
    }

    public void OnPlacedOnGrid() {
        throw new System.NotImplementedException();
    }

    public void OnRemovedFromGrid() {
        throw new System.NotImplementedException();
    }

    public void PlaceOnGrid(Vector2Int startPosition, Grid grid) {
        this.grid = grid;
        this.gridPosition = startPosition;
    }

    public void RemoveFromGrid(Grid grid) {
        grid.RemoveObject(gridPosition);
        Destroy(gameObject);
    }
}
