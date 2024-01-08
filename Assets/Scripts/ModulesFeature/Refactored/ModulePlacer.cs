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

public interface IPlacementStrategy {
    void UpdateInput(Grid grid, Vector3 mousePosOnGrid, ModulePlacer modulePlacer);
    void SetModule(GridObjectSO newModule);
    List<Vector2Int> GetHoveredPositions(Grid grid, Vector3 mousePosOnGrid, ModulePlacer modulePlacer);
}

public class ClickPlacer : IPlacementStrategy {

    private GridObjectSO currentModule;

    public List<Vector2Int> GetHoveredPositions(Grid grid, Vector3 mousePosOnGrid, ModulePlacer modulePlacer) {
        Vector2Int buildingDimensions = currentModule.GetLayoutShapeDimensions(modulePlacer.NumRotations);
        Vector2Int gridPosition = grid.GetSubgridOriginCoord(mousePosOnGrid, buildingDimensions);
        List<Vector2Int> buildingPositions = currentModule.GetLayoutShape(modulePlacer.NumRotations);
        for(int i = 0; i < buildingPositions.Count; i++) {
            buildingPositions[i] += gridPosition;
        }
        return buildingPositions;
    }

    public void SetModule(GridObjectSO newModule) {
        this.currentModule = newModule;
    }

    public void UpdateInput(Grid grid, Vector3 mousePosOnGrid, ModulePlacer modulePlacer) {
        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            modulePlacer.TryPlaceModule(currentModule, mousePosOnGrid);
        }
        if (Input.mouseScrollDelta.y > 0.1f) {
            modulePlacer.RotateModuleCounterClockwise();
        }
        if(Input.mouseScrollDelta.y < -0.1f) {
            modulePlacer.RotateModuleClockwise();
        }
    }
}

public class ClickAndDragPlacer : IPlacementStrategy {
    private GridObjectSO currentModule;
    private Vector2Int startDragPos;
    private bool isDragging;

    public void SetModule(GridObjectSO newModule) {
        currentModule = newModule;
    }

    public void UpdateInput(Grid grid, Vector3 mousePosOnGrid, ModulePlacer modulePlacer) {
        if (Input.GetKeyDown(KeyCode.Mouse1)) {
            startDragPos = grid.GetCellCoords(mousePosOnGrid);
            isDragging = true;
        }
        if (Input.GetKeyUp(KeyCode.Mouse1) && isDragging) {
            Vector2Int endDragPos = grid.GetCellCoords(mousePosOnGrid);
            List<Vector2Int> path = grid.FindPath(startDragPos, endDragPos);
            foreach(Vector2Int position in path) {
                modulePlacer.TryPlaceModule(currentModule, grid.GetCellCenter(position));
            }
            isDragging = false;
        }
        if (Input.mouseScrollDelta.y > 0.1f) {
            modulePlacer.RotateModuleCounterClockwise();
        }
        if (Input.mouseScrollDelta.y < -0.1f) {
            modulePlacer.RotateModuleClockwise();
        }
    }

    public List<Vector2Int> GetHoveredPositions(Grid grid, Vector3 mousePosOnGrid, ModulePlacer modulePlacer) {
        if (!isDragging) {
            return new List<Vector2Int>() { grid.GetCellCoords(mousePosOnGrid) };
        }
        else {
            return grid.FindPath(startDragPos, grid.GetCellCoords(mousePosOnGrid));
        }
    }
}
