using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ModulePlacer : MonoBehaviour
{
    [HideInInspector] public UnityEvent moduleRotated;
    [HideInInspector] public UnityEvent<GridObjectSO> moduleChanged;

    [SerializeField] private FactoryGrid grid;
    [SerializeField] private MouseInput mouseInput;
    [SerializeField] private BuildingSelector buildingSelector;
    [SerializeField] private AssemblyLineSystem assemblyLineSystem;

    public Quaternion CurrentPlacementRotation { get; private set; }
    public Facing CurrentFacing { get; private set; }

    private IPlacementStrategy placementHandler;

    private void Awake() {
        placementHandler = new NoPlacement();
        CurrentFacing = Facing.West;
    }

    private void Update() {
        placementHandler.UpdateInput(grid, mouseInput.LastGroundHitPoint, this);
        if (mouseInput.RightMouseButtonPressed()) {
            Vector2Int mouseGridPos = grid.GetCellCoords(mouseInput.LastGroundHitPoint);
            RemoveModule(mouseGridPos);
        }
    }

    public void RotateModuleClockwise() {
        SetModuleRotation(CurrentFacing.RotatedDirection(1));
    }

    public void RotateModuleCounterClockwise() {
        SetModuleRotation(CurrentFacing.RotatedDirection(-1));
    }

    public List<Vector2Int> GetHoveredPositions() {
        return placementHandler.GetHoveredPositions(grid, mouseInput.LastGroundHitPoint, this);
    }

    private void OnSelectedBuildingChanged(GridObjectSO newBuilding) {
        if(newBuilding == null) {
            ExitPlacementMode();
        }
        else {
            placementHandler = newBuilding.GetPlacementHandler();
        }

        moduleChanged?.Invoke(newBuilding);
    }

    public void ExitPlacementMode() {
        placementHandler = new NoPlacement();
    }

    private void SetModuleRotation(Facing facing) {
        CurrentFacing = facing;
        CurrentPlacementRotation = grid.Rotation * CurrentFacing.GetRotationFromFacing();
        moduleRotated?.Invoke();
    }

    private void OnEnable() {
        buildingSelector.selectedObjectChanged.AddListener(OnSelectedBuildingChanged);
    }

    private void OnDisable() {
        buildingSelector.selectedObjectChanged.RemoveListener(OnSelectedBuildingChanged);
    }

    public IGridObject TryPlaceModule(GridObjectSO moduleData, Vector3 mouseHitPosition, Facing facing) {
        Vector2Int buildingDimensions = moduleData.GetLayoutShapeDimensions(facing);
        Vector2Int gridPosition = grid.GetSubgridOriginCoord(mouseHitPosition, buildingDimensions);
        return TryPlaceModule(moduleData, gridPosition, facing);
    }

    public IGridObject TryPlaceModule(GridObjectSO moduleData, Vector2Int lowerLeftPosition, Facing facing) {
        List<Vector2Int> buildingPositions = moduleData.GetLayoutShape(facing);
        for(int i = 0; i < buildingPositions.Count; i++) {
            buildingPositions[i] += lowerLeftPosition;
        }
        bool allPositionsAreFree = grid.AllPositionsAreFree(buildingPositions);
        if (allPositionsAreFree) {
            IGridObject placedObject = PlaceModule(moduleData, lowerLeftPosition, facing);
            return placedObject;
        }
        else {
            return null;
        }
    }

    private IGridObject PlaceModule(GridObjectSO moduleData, Vector2Int lowerLeft, Facing facing) {
        Quaternion rotation = grid.Rotation * facing.GetRotationFromFacing();
        Vector3 spawnPos = grid.GetSubgridCenter(lowerLeft, moduleData.GetLayoutShapeDimensions(facing));
        IGridObject gridObject = moduleData.CreateInstance(spawnPos, rotation, facing, assemblyLineSystem);
        grid.PlaceObject(gridObject, lowerLeft, moduleData.GetLayoutShape(facing));
        gridObject.OnPlacedOnGrid(lowerLeft, grid);
        return gridObject;
    }

    public void RemoveModule(Vector2Int gridPosition) {
        IGridObject gridObject = grid.GetObjectAt(gridPosition);
        if(gridObject != null) {
            gridObject.DestroyObject();
        }
    }    
}
