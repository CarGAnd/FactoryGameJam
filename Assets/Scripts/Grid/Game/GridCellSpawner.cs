using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FactoryGrid))]
public class GridCellSpawner : MonoBehaviour
{
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] Vector3 spawnOffset;
    [SerializeField] private Transform cellParent;
    [SerializeField] private SpawnType spawnType;

    [SerializeField, HideInInspector] private bool[] customSpawnMask;
    [SerializeField, HideInInspector] private int currentWidth;
    [SerializeField, HideInInspector] private int currentHeight;

    private FactoryGrid grid;
    private GameObject[,] cells;

    private void Awake() {
        CreateCells();
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
        if (!IsWithingBounds(coord.x, coord.y)) {
            return false;
        }
        return cells[coord.y, coord.x] != null;
    }

    public void SetMaskValue(int x, int y, bool value) {
        if (!IsWithingBounds(x, y)) {
            return;
        }
        customSpawnMask[y * currentWidth + x] = value;
    }

    private bool IsWithingBounds(int x, int y) {
        return x >= 0 && x < currentWidth && y >= 0 && y < currentHeight; 
    }

    [Button("Destroy Prefabs", ButtonSizes.Medium)]
    private void DestroyCells() {
        for (int i = cellParent.childCount - 1; i >= 0; i--) {
            DestroyImmediate(cellParent.GetChild(i).gameObject);
        }
        cells = null;
    }

    [Button("Create Prefabs", ButtonSizes.Medium)]
    private void CreateCells() {
        DestroyCells();
        grid = GetComponent<FactoryGrid>();
        cells = new GameObject[grid.Rows, grid.Columns];

        if(spawnType == SpawnType.FillGrid) {
            FillGridWithPrefabs();
        }
        else if(spawnType == SpawnType.Custom) {
            MaskGridWithPrefabs();
        }
    }

    private void FillGridWithPrefabs() {
        for (int y = 0; y < cells.GetLength(0); y++) {
            for (int x = 0; x < cells.GetLength(1); x++) {
                CreateObjectAt(new Vector2Int(x, y));
            }
        }
    }

    private void MaskGridWithPrefabs() {
        for (int y = 0; y < cells.GetLength(0); y++) {
            for (int x = 0; x < cells.GetLength(1); x++) {
                if (GetMaskValue(x, y)) {
                    CreateObjectAt(new Vector2Int(x,y));
                }
            }
        }
    }

    private bool GetMaskValue(int x, int y) {
        return customSpawnMask[y * currentWidth + x];
    }

    private enum SpawnType {
        FillGrid,
        Custom
    }

    private void OnValidate() {
        grid = GetComponent<FactoryGrid>();
        if(grid.Rows != currentHeight || grid.Columns != currentWidth) {
            bool[] newMask = new bool[grid.Rows * grid.Columns];
            for(int y = 0; y < Mathf.Min(currentHeight, grid.Rows); y++) {
                for(int x = 0; x < Mathf.Min(currentWidth, grid.Columns); x++) {
                    newMask[y * grid.Columns + x] = GetMaskValue(x, y);
                }
            }
            customSpawnMask = newMask;
            currentWidth = grid.Columns;
            currentHeight = grid.Rows;
        }    
    }

    [SerializeField] private bool showCustomLayout;

    private void OnDrawGizmosSelected() {
        if(spawnType == SpawnType.Custom && showCustomLayout) {
            for(int y = 0; y < currentHeight; y++) {
                for(int x = 0; x < currentWidth; x++) {
                    Color c = GetMaskValue(x, y) ? Color.green : Color.red;
                    c.a = 0.5f;
                    Gizmos.color = c;
                    Gizmos.DrawCube(grid.GetCellCenter(new Vector2Int(x, y)), new Vector3(grid.CellSize.x, 0.1f, grid.CellSize.y));
                }
            }
        }
    }
}


