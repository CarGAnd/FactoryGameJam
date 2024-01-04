using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModulePlacer : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private MouseInput mouseInput;
    [SerializeField] private BuildingSelector buildingSelector;

    private GridObjectSO currentModule;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            TryPlaceModule(currentModule, mouseInput.LastGroundHitPoint);
        }
    }

    private void OnSelectedBuildingChanged(GridObjectSO newBuilding) {
        currentModule = newBuilding;
    }

    private void OnEnable() {
        buildingSelector.selectedObjectChanged.AddListener(OnSelectedBuildingChanged);
    }

    private void OnDisable() {
        buildingSelector.selectedObjectChanged.RemoveListener(OnSelectedBuildingChanged);
    }

    private void TryPlaceModule(GridObjectSO moduleData, Vector3 mouseHitPosition) {
        Vector2Int gridPosition = grid.GetSubgridOriginCoord(mouseHitPosition, new Vector2Int(moduleData.width, moduleData.height));
        List<Vector2Int> buildingPositions = grid.GetPositionsInSubgrid(gridPosition, currentModule.GetLayoutShapeDimensions());
        bool positionsAreFree = grid.AllPositionsAreFree(buildingPositions);
        if (positionsAreFree) {
            PlaceModule(moduleData, gridPosition);
        }
    }

    private void PlaceModule(GridObjectSO moduleData, Vector2Int lowerLeft) {
        Vector3 spawnPos = grid.GetSubgridCenter(lowerLeft, moduleData.GetLayoutShapeDimensions());
        GameObject moduleObject = Instantiate(moduleData.modulePrefab, spawnPos, Quaternion.identity);
        IGridObject gridObject = moduleObject.GetComponent<IGridObject>();
        grid.PlaceObject(gridObject, lowerLeft, moduleData.GetLayoutShape());
    }
}
