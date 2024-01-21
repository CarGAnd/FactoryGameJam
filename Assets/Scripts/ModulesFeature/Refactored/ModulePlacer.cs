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
    [SerializeField] private AssemblyLineSystem assemblyLineSystem;

    public Quaternion CurrentPlacementRotation { get; private set; }
    public int NumRotations { get; private set; }

    private IPlacementStrategy placementHandler;

    private void Awake() {
        placementHandler = new NoPlacement();
    }

    private void Update() {
        placementHandler.UpdateInput(grid, mouseInput.LastGroundHitPoint, this);
        if (mouseInput.RightMouseButtonPressed()) {
            Vector2Int mouseGridPos = grid.GetCellCoords(mouseInput.LastGroundHitPoint);
            RemoveModule(mouseGridPos);
        }
    }

    public void RotateModuleClockwise() {
        SetModuleRotation(NumRotations + 1);
    }

    public void RotateModuleCounterClockwise() {
        SetModuleRotation(NumRotations - 1);
    }

    public List<Vector2Int> GetHoveredPositions() {
        return placementHandler.GetHoveredPositions(grid, mouseInput.LastGroundHitPoint, this);
    }

    private void OnSelectedBuildingChanged(GridObjectSO newBuilding) {
        moduleChanged?.Invoke(newBuilding);

        if(newBuilding == null) {
            ExitPlacementMode();
            return;
        }

        placementHandler = newBuilding.GetPlacementHandler();
        SetModuleRotation(0);
    }

    public void ExitPlacementMode() {
        placementHandler = new NoPlacement();
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

    public IGridObject TryPlaceModule(GridObjectSO moduleData, Vector3 mouseHitPosition, Quaternion rotation) {
        Vector2Int buildingDimensions = moduleData.GetLayoutShapeDimensions(NumRotations);
        Vector2Int gridPosition = grid.GetSubgridOriginCoord(mouseHitPosition, buildingDimensions);
        return TryPlaceModule(moduleData, gridPosition, rotation);
    }

    public IGridObject TryPlaceModule(GridObjectSO moduleData, Vector2Int lowerLeftPosition, Quaternion rotation) {
        List<Vector2Int> buildingPositions = moduleData.GetLayoutShape(NumRotations);
        for(int i = 0; i < buildingPositions.Count; i++) {
            buildingPositions[i] += lowerLeftPosition;
        }
        bool allPositionsAreFree = grid.AllPositionsAreFree(buildingPositions);
        if (allPositionsAreFree) {
            IGridObject placedObject = PlaceModule(moduleData, lowerLeftPosition, rotation);
            return placedObject;
        }
        else {
            return null;
        }
    }

    private IGridObject PlaceModule(GridObjectSO moduleData, Vector2Int lowerLeft, Quaternion rotation) {
        Vector3 spawnPos = grid.GetSubgridCenter(lowerLeft, moduleData.GetLayoutShapeDimensions(NumRotations));
        IGridObject gridObject = moduleData.CreateInstance(spawnPos, rotation, NumRotations, assemblyLineSystem);
        grid.PlaceObject(gridObject, lowerLeft, moduleData.GetLayoutShape(NumRotations));
        gridObject.OnPlacedOnGrid(lowerLeft, grid);
        return gridObject;
    }

    public void RemoveModule(Vector2Int gridPosition) {
        IGridObject gridObject = grid.GetObjectAt(gridPosition);
        gridObject.DestroyObject();
    }    
}
