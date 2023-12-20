using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

public class MockGridObject : MonoBehaviour, IGridInteractable {
    private List<Vector2Int> shapeLayout;
    private bool isSelected;
    private bool isPlaced;

    public UnityEvent Selected;
    public UnityEvent Deselected;
    public UnityEvent PlacedOnGrid;
    public UnityEvent RemovedFromGrid;

    void OnEnable()
    {
        Selected.AddListener(OnSelected);
        Deselected.AddListener(OnDeselected);
        PlacedOnGrid.AddListener(OnPlacedOnGrid);
        RemovedFromGrid.AddListener(OnRemovedFromGrid);

    }
    
    // Implementing IGridObject
    public List<Cell> GetOccupyingCells(Cell startCell, Grid grid) {
        throw new System.NotImplementedException();
    }

    public T GetObject<T>() {
        throw new System.NotImplementedException();
    }

    public void PlaceOnGrid(Cell startCell, Grid grid) {
        throw new System.NotImplementedException();
        //grid.PlaceObject(this, startCell, shapeLayout);
    }

    public void RemoveFromGrid() {
        throw new System.NotImplementedException();
    }

    public void OnRemovedFromGrid() {
        throw new System.NotImplementedException();
    }

    public void OnPlacedOnGrid() {
        throw new System.NotImplementedException();
    }

    public void OnDeselected() {
        throw new System.NotImplementedException();
    }

    public List<Vector2Int> GetShapeLayout() {
        throw new System.NotImplementedException();
    }

    // Implementing IGridInteractable
    public void OnSelected() {
        throw new System.NotImplementedException();
    }

    public bool IsSelected() {
        throw new System.NotImplementedException();
    }

    public bool IsPlaced() {
        throw new System.NotImplementedException();
    }
}
