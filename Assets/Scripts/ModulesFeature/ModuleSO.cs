using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ModuleSO : ScriptableObject, IGridObject
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private GameObject modulePrefab;

    public T GetObject<T>() {
        throw new System.NotImplementedException();
    }

    public List<Cell> GetOccupyingCells(Cell startCell, Grid grid) {
        throw new System.NotImplementedException();
    }

    public List<Vector2Int> GetShapeLayout() {
        throw new System.NotImplementedException();
    }

    public void OnPlacedOnGrid() {
        throw new System.NotImplementedException();
    }

    public void OnRemovedFromGrid() {
        throw new System.NotImplementedException();
    }

    public void PlaceOnGrid(Cell startCell, Grid grid) {
        throw new System.NotImplementedException();
    }

    public void RemoveFromGrid() {
        throw new System.NotImplementedException();
    }
}
