using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GridController : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;
    [SerializeField] private int cellSize;
    [SerializeField] private Vector3 origin;

    [SerializeField] private Grid<ModuleBase> grid;

    private void Start() {
        grid = new Grid<ModuleBase>(width, height, origin, cellSize);
    }

    public void PlaceModule(int gridX, int gridY, ModuleBase module) {
        grid.SetObjectAtCoordinates(gridX, gridY, module);
    }

    public void PlaceModule(Vector3 worldPos, ModuleBase module) {
        Vector2Int gridPos = grid.WorldToGrid(worldPos);
        PlaceModule(gridPos.x, gridPos.y, module);
    }

    public bool CellIsEmpty() {
        return true;
    }

    private void OnValidate() {
        
    }
}
