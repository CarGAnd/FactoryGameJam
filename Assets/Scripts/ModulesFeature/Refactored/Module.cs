using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Module : MonoBehaviour, IGridObject
{
    [SerializeField] private GridObjectSO moduleData;

    public T GetObject<T>() {
        throw new System.NotImplementedException();
    }

    public List<Vector2Int> GetOccupyingCells(Vector2Int startCell, Grid grid) {
        throw new System.NotImplementedException();
    }

    public List<Vector2Int> GetShapeLayout() {
        return moduleData.GetLayoutShape(0);
    }

    public void OnPlacedOnGrid() {
        throw new System.NotImplementedException();
    }

    public void OnRemovedFromGrid() {
        throw new System.NotImplementedException();
    }

    public void PlaceOnGrid(Vector2Int startCell, Grid grid) {
        throw new System.NotImplementedException();
    }

    public void RemoveFromGrid(Grid grid) {
        throw new System.NotImplementedException();
    }
}
