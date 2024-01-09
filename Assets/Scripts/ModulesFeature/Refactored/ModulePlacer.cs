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

    private IPlacementStrategy placementHandler;

    private void Update() {
        if(placementHandler != null) {
            placementHandler.UpdateInput(grid, mouseInput.LastGroundHitPoint, this);
        }
    }

    public void RotateModuleClockwise() {
        SetModuleRotation(NumRotations + 1);
    }

    public void RotateModuleCounterClockwise() {
        SetModuleRotation(NumRotations - 1);
    }

    public List<Vector2Int> GetHoveredPositions() {
        if(placementHandler != null) {
            return placementHandler.GetHoveredPositions(grid, mouseInput.LastGroundHitPoint, this);
        }
        else {
            return new List<Vector2Int>();
        }
    }

    private void OnSelectedBuildingChanged(GridObjectSO newBuilding) {
        moduleChanged?.Invoke(newBuilding);
        placementHandler = newBuilding.GetPlacementHandler();
        placementHandler.SetModule(newBuilding);
        SetModuleRotation(0);
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
        gridObject.OnPlacedOnGrid(lowerLeft, grid);
    }
}
