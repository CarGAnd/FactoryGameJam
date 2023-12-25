using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Grid))]
public class GridCellSpawner : MonoBehaviour
{
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] Vector3 spawnOffset;
    [SerializeField] private Transform cellParent;

    private Grid grid;
    private GameObject[,] cells;

    private void Awake() {
        CreateGrid();
    }

    public void DestroyObjectAt(Vector2Int coord) {
        Destroy(cells[coord.y, coord.x]);
        cells[coord.y, coord.x] = null;
    }

    public void CreateObjectAt(Vector2Int coord) {
        Vector3 spawnPos = grid.GetCellCenter(coord) + spawnOffset;
        Quaternion rotation = grid.Rotation;
        cells[coord.y, coord.x] = Instantiate(cellPrefab, spawnPos, rotation, cellParent);
        cells[coord.y, coord.x].transform.localScale = new Vector3(grid.CellSize.x, 1, grid.CellSize.y);
    }

    public GameObject GetGameObjectAt(Vector2Int coord) {
        return cells[coord.y, coord.x];
    }

    public GameObject GetGameObjectAt(Vector3 worldPos) {
        Vector2Int coord = grid.GetCellCoords(worldPos);
        return GetGameObjectAt(coord);
    }

    public bool CellHasSpawnedPrefab(Vector2Int coord) {
        return cells[coord.y, coord.x] != null;
    }

    [Button("Destroy Grid", ButtonSizes.Medium)]
    private void DestroyGrid() {
        for (int i = cellParent.childCount - 1; i >= 0; i--) {
            DestroyImmediate(cellParent.GetChild(i).gameObject);
        }
        cells = null;
    }

    [Button("Create Grid", ButtonSizes.Medium)]
    private void CreateGrid() {
        DestroyGrid();
        grid = GetComponent<Grid>();
        cells = new GameObject[grid.Rows, grid.Columns];
        for (int y = 0; y < cells.GetLength(0); y++) {
            for (int x = 0; x < cells.GetLength(1); x++) {
                CreateObjectAt(new Vector2Int(x, y));
            }
        }
    }
}
