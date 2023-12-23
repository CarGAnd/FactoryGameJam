using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class GridInitializer : MonoBehaviour
{
    [SerializeField] private bool showGizmos;
    private Grid grid;
    [SerializeField] public List<GameObject> prePlacedObjects;
    

    // Start is called before the first frame update
    void Start()
    {
        grid = GetComponent<Grid>();
        foreach(GameObject g in prePlacedObjects) {
            IGridObject gridObject = new TestGridObject();
            grid.PlaceObject(gridObject, grid.GetCellCoords(g.transform.position));
        }
    }

    private void OnDrawGizmos() {
        if (!showGizmos) {
            return;
        }

        grid = GetComponent<Grid>();
        Gizmos.color = Color.yellow;
        foreach(GameObject g in prePlacedObjects) {
            Vector3 pos = grid.GetCellCenter(g.transform.position);
            Gizmos.DrawWireSphere(pos, Mathf.Min(grid.CellSize.x, grid.CellSize.y) / 3f);
        }
    }
}

public class TestGridObject : IGridObject {
    public T GetObject<T>() {
        throw new System.NotImplementedException();
    }

    public List<Vector2Int> GetOccupyingCells(Vector2Int startCell, Grid grid) {
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

    public void PlaceOnGrid(Vector2Int startCell, Grid grid) {
        throw new System.NotImplementedException();
    }

    public void RemoveFromGrid() {
        throw new System.NotImplementedException();
    }
}