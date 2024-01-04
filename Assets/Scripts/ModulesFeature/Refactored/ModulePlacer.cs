using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModulePlacer : MonoBehaviour
{
    [SerializeField] private Grid grid;

    [SerializeField] private GridObjectSO currentModule;


    private void PlaceModule(GridObjectSO moduleData, Vector3 mouseHitPosition) {
        Vector3 spawnPos = CalculateSpawnPosition(moduleData, mouseHitPosition);
        Vector2Int gridPosition = grid.GetCellCoords(mouseHitPosition);
        GameObject moduleObject = Instantiate(moduleData.modulePrefab, spawnPos, Quaternion.identity);
        IGridObject gridObject = moduleObject.GetComponent<IGridObject>();
        grid.PlaceObject(gridObject, gridPosition, moduleData.GetLayoutShape());
    }

    private Vector3 CalculateSpawnPosition(GridObjectSO moduleData, Vector3 mouseHitPos) {
        return grid.GetCellCenter(mouseHitPos);
    }
}
