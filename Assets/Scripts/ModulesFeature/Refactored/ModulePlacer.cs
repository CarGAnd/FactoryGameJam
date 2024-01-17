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
        if(newBuilding == null) {
            return;
        }
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

    public void TryPlaceModule(GridObjectSO moduleData, Vector3 mouseHitPosition, Quaternion rotation) {
        Vector2Int buildingDimensions = moduleData.GetLayoutShapeDimensions(NumRotations);
        Vector2Int gridPosition = grid.GetSubgridOriginCoord(mouseHitPosition, buildingDimensions);
        TryPlaceModule(moduleData, gridPosition, rotation);
    }

    public void TryPlaceModule(GridObjectSO moduleData, Vector2Int lowerLeftPosition, Quaternion rotation) {
        List<Vector2Int> buildingPositions = moduleData.GetLayoutShape(NumRotations);
        for(int i = 0; i < buildingPositions.Count; i++) {
            buildingPositions[i] += lowerLeftPosition;
        }
        bool allPositionsAreFree = grid.AllPositionsAreFree(buildingPositions);
        if (allPositionsAreFree) {
            PlaceModule(moduleData, lowerLeftPosition, rotation);
        }
    }

    private void PlaceModule(GridObjectSO moduleData, Vector2Int lowerLeft, Quaternion rotation) {
        Vector3 spawnPos = grid.GetSubgridCenter(lowerLeft, moduleData.GetLayoutShapeDimensions(NumRotations));
        IGridObject gridObject = moduleData.CreateInstance(spawnPos, rotation, NumRotations);
        grid.PlaceObject(gridObject, lowerLeft, moduleData.GetLayoutShape(NumRotations));
        gridObject.OnPlacedOnGrid(lowerLeft, grid);
    }

    public void PlaceModulesAlongPath(GridObjectSO moduleData, Path path) {
        List<Vector2Int> pathPositions = path.GetPositions();
        List<Vector2Int> pathDirections = path.GetDirections();
        for(int i = 0; i < pathPositions.Count; i++) {
            //TODO: figure out a consistent way of managing rotations instead of using 3 different representations (int, Quaternion, Facing)
            TryPlaceModule(moduleData, pathPositions[i], RotationFromDirection(pathDirections[i]));
        }    
    }

    private Quaternion RotationFromDirection(Vector2Int direction) {
        if(direction == Vector2Int.left) {
            return Quaternion.Euler(new Vector3(0, 0, 0));
        }
        else if(direction == Vector2Int.up) {
            return Quaternion.Euler(new Vector3(0, 90, 0));
        }
        else if(direction == Vector2Int.right) {
            return Quaternion.Euler(new Vector3(0, 180, 0));
        }
        else {
            return Quaternion.Euler(new Vector3(0, 270, 0));
        }
    }
}
