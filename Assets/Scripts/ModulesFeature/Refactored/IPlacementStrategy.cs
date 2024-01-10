using System.Collections.Generic;
using UnityEngine;

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
            modulePlacer.TryPlaceModule(currentModule, mousePosOnGrid, modulePlacer.CurrentPlacementRotation);
        }
        if (Input.mouseScrollDelta.y > 0.1f) {
            modulePlacer.RotateModuleCounterClockwise();
        }
        if (Input.mouseScrollDelta.y < -0.1f) {
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
            isDragging = false;
            Vector2Int endDragPos = grid.GetCellCoords(mousePosOnGrid);
            Path path = grid.FindPath(startDragPos, endDragPos);
            if(path.IsEmpty()) {
                //If no path is found, we cannot place modules
                return;
            }
            modulePlacer.PlaceModulesAlongPath(currentModule, path);
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
            Path path = grid.FindPath(startDragPos, grid.GetCellCoords(mousePosOnGrid));
            if(!path.IsEmpty()) {
                return path.GetPositions();
            }
            else {
                return new List<Vector2Int>() { grid.GetCellCoords(mousePosOnGrid) };
            }
        }
    }
}

