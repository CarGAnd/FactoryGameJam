using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ModulePlacer : MonoBehaviour
{
    [HideInInspector] public UnityEvent moduleRotated;
    [HideInInspector] public UnityEvent<GridObjectSO> moduleChanged;

    [SerializeField] private Grid grid;
    [SerializeField] private MouseInput mouseInput;
    [SerializeField] private BuildingSelector buildingSelector;

    public Quaternion CurrentPlacementRotation { get; private set; }
    public int NumRotations { get; private set; }

    private GridObjectSO currentModule;
    
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            TryPlaceModule(currentModule, mouseInput.LastGroundHitPoint);
        }
        if (Input.mouseScrollDelta.y > 0.1f) {
            RotateModuleCounterClockwise();
        }
        if(Input.mouseScrollDelta.y < -0.1f) {
            RotateModuleClockwise();
        }
    }

    private void RotateModuleClockwise() {
        SetModuleRotation(NumRotations + 1);
    }

    private void RotateModuleCounterClockwise() {
        SetModuleRotation(NumRotations - 1);
    }

    private void OnSelectedBuildingChanged(GridObjectSO newBuilding) {
        currentModule = newBuilding;
        SetModuleRotation(0);
        moduleChanged?.Invoke(newBuilding);
    }

    private void SetModuleRotation(int numRotations) {
        NumRotations = numRotations % 4;
        CurrentPlacementRotation = Quaternion.Euler(new Vector3(0, 90 * NumRotations, 0));
        moduleRotated?.Invoke();
    }

    private void OnEnable() {
        buildingSelector.selectedObjectChanged.AddListener(OnSelectedBuildingChanged);
    }

    private void OnDisable() {
        buildingSelector.selectedObjectChanged.RemoveListener(OnSelectedBuildingChanged);
    }

    public void TryPlaceModule(GridObjectSO moduleData, Vector3 mouseHitPosition) {
        Vector2Int buildingDimensions = moduleData.GetLayoutShapeDimensions(NumRotations);
        Vector2Int gridPosition = grid.GetSubgridOriginCoord(mouseHitPosition, buildingDimensions);
        List<Vector2Int> buildingPositions = grid.GetPositionsInSubgrid(gridPosition, buildingDimensions);
        bool allPositionsAreFree = grid.AllPositionsAreFree(buildingPositions);
        if (allPositionsAreFree) {
            PlaceModule(moduleData, gridPosition);
        }
    }

    private void PlaceModule(GridObjectSO moduleData, Vector2Int lowerLeft) {
        Vector3 spawnPos = grid.GetSubgridCenter(lowerLeft, moduleData.GetLayoutShapeDimensions(NumRotations));
        IGridObject gridObject = moduleData.CreateInstance(spawnPos, CurrentPlacementRotation, NumRotations);
        grid.PlaceObject(gridObject, lowerLeft, moduleData.GetLayoutShape(NumRotations));
        gridObject.PlaceOnGrid(lowerLeft, grid);
    }
}
