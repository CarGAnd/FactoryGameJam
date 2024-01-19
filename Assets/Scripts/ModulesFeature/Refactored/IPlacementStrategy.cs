using System.Collections.Generic;
using UnityEngine;

public interface IPlacementStrategy {
    void UpdateInput(Grid grid, Vector3 mousePosOnGrid, ModulePlacer modulePlacer);
    List<Vector2Int> GetHoveredPositions(Grid grid, Vector3 mousePosOnGrid, ModulePlacer modulePlacer);
}

public class ClickPlacer : IPlacementStrategy {

    private GridObjectSO currentModule;

    public ClickPlacer(GridObjectSO obj) {
        this.currentModule = obj;
    }

    public List<Vector2Int> GetHoveredPositions(Grid grid, Vector3 mousePosOnGrid, ModulePlacer modulePlacer) {
        Vector2Int buildingDimensions = currentModule.GetLayoutShapeDimensions(modulePlacer.NumRotations);
        Vector2Int gridPosition = grid.GetSubgridOriginCoord(mousePosOnGrid, buildingDimensions);
        List<Vector2Int> buildingPositions = currentModule.GetLayoutShape(modulePlacer.NumRotations);
        for(int i = 0; i < buildingPositions.Count; i++) {
            buildingPositions[i] += gridPosition;
        }
        return buildingPositions;
    }

    public void UpdateInput(Grid grid, Vector3 mousePosOnGrid, ModulePlacer modulePlacer) {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
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


    public ClickAndDragPlacer(GridObjectSO obj) {
        this.currentModule = obj;
    }

    public void UpdateInput(Grid grid, Vector3 mousePosOnGrid, ModulePlacer modulePlacer) {
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            startDragPos = grid.GetCellCoords(mousePosOnGrid);
            isDragging = true;
        }
        if (Input.GetKeyUp(KeyCode.Mouse0) && isDragging) {
            isDragging = false;
            Vector2Int endDragPos = grid.GetCellCoords(mousePosOnGrid);
            Path path = grid.FindPath(startDragPos, endDragPos);
            if(path.IsEmpty()) {
                //If no path is found, we cannot place modules
                return;
            }
            if(path.GetPositions().Count == 1) {
                modulePlacer.TryPlaceModule(currentModule, endDragPos, modulePlacer.CurrentPlacementRotation);
            }
            else {
                PlaceModulesAlongPath(currentModule, path, modulePlacer);
            }            
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

    public void PlaceModulesAlongPath(GridObjectSO moduleData, Path path, ModulePlacer modulePlacer) {
        List<Vector2Int> pathPositions = path.GetPositions();
        List<Vector2Int> pathDirections = path.GetDirections();
        //The first n-1 modules are rotated to match the path
        for(int i = 0; i < pathPositions.Count - 1; i++) {
            //TODO: figure out a consistent way of managing rotations instead of using 3 different representations (int, Quaternion, Facing)
            modulePlacer.TryPlaceModule(moduleData, pathPositions[i], RotationFromDirection(pathDirections[i]));
        }
        //The last module is rotated according to the user input
        modulePlacer.TryPlaceModule(moduleData, pathPositions[pathPositions.Count - 1], modulePlacer.CurrentPlacementRotation);
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

public class NoPlacement : IPlacementStrategy {
    public List<Vector2Int> GetHoveredPositions(Grid grid, Vector3 mousePosOnGrid, ModulePlacer modulePlacer) {
        return new List<Vector2Int>() { grid.GetCellCoords(mousePosOnGrid) };    
    }

    public void UpdateInput(Grid grid, Vector3 mousePosOnGrid, ModulePlacer modulePlacer) {
        
    }
}

