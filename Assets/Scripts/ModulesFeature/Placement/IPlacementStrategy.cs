using System.Collections.Generic;
using UnityEngine;

public interface IPlacementStrategy {
    void UpdateInput(MouseInput mouseInput);
    List<Vector2Int> GetHoveredPositions(Vector3 mousePosOnGrid);
}

public class ClickPlacer : IPlacementStrategy {

    private GridObjectSO currentModule;
    private FactoryGrid grid;
    private PlacementMode placementMode;

    public ClickPlacer(GridObjectSO obj, FactoryGrid grid, PlacementMode placementMode) {
        this.currentModule = obj;
        this.grid = grid;
        this.placementMode = placementMode;
    }

    public List<Vector2Int> GetHoveredPositions(Vector3 mousePosOnGrid) {
        Vector2Int buildingDimensions = currentModule.GetLayoutShapeDimensions(placementMode.CurrentFacing);
        Vector2Int gridPosition = grid.GetSubgridOriginCoord(mousePosOnGrid, buildingDimensions);
        List<Vector2Int> buildingPositions = currentModule.GetLayoutShape(placementMode.CurrentFacing);
        for(int i = 0; i < buildingPositions.Count; i++) {
            buildingPositions[i] += gridPosition;
        }
        return buildingPositions;
    }

    public void UpdateInput(MouseInput mouseInput) {
        if (mouseInput.LeftMouseButtonWasPressed()) {
            placementMode.TryPlaceModule(currentModule, mouseInput.LastGroundHitPoint, placementMode.CurrentFacing);
        }
        placementMode.UpdateRotationInput(mouseInput);    
    }
}

public class PathPlacer : IPlacementStrategy {
    private GridObjectSO currentModule;
    private PlacementMode placementMode;
    private FactoryGrid grid;

    private Vector2Int startDragPos;
    private bool isDragging;

    public PathPlacer(GridObjectSO obj, FactoryGrid grid, PlacementMode placementMode) {
        this.currentModule = obj;
        this.grid = grid;
        this.placementMode = placementMode;
    }

    public void UpdateInput(MouseInput mouseInput) {
        Vector3 mousePosOnGrid = mouseInput.LastGroundHitPoint;
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
                placementMode.TryPlaceModule(currentModule, endDragPos, placementMode.CurrentFacing);
            }
            else {
                PlaceModulesAlongPath(currentModule, path);
            }            
        }
        placementMode.UpdateRotationInput(mouseInput);
    }

    public List<Vector2Int> GetHoveredPositions(Vector3 mousePosOnGrid) {
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

    private void PlaceModulesAlongPath(GridObjectSO moduleData, Path path) {
        List<Vector2Int> pathPositions = path.GetPositions();
        List<Vector2Int> pathDirections = path.GetDirections();
        //The first n-1 modules are rotated to match the path
        for(int i = 0; i < pathPositions.Count - 1; i++) {
            placementMode.TryPlaceModule(moduleData, pathPositions[i], FacingFromDirection(pathDirections[i]));
        }
        //The last module is rotated according to the user input
        placementMode.TryPlaceModule(moduleData, pathPositions[pathPositions.Count - 1], placementMode.CurrentFacing);
    }

    private Facing FacingFromDirection(Vector2Int direction) {
        if(direction == Vector2Int.left) {
            return Facing.West;
        }
        else if(direction == Vector2Int.up) {
            return Facing.North;
        }
        else if(direction == Vector2Int.right) {
            return Facing.East;
        }
        else {
            return Facing.South;
        }
    }
}

public class BoxPlacer : IPlacementStrategy {

    private GridObjectSO currentModule;
    private Vector2Int startDragPos;
    private bool isDragging;
    private FactoryGrid grid;
    private PlacementMode placementMode;

    public BoxPlacer(GridObjectSO gridObject, FactoryGrid grid, PlacementMode placementMode) {
        this.currentModule = gridObject;
        this.grid = grid;
        this.placementMode = placementMode;
    }

    public List<Vector2Int> GetHoveredPositions(Vector3 mousePosOnGrid) {
        if (isDragging) {
            return GetPositionsInBox(startDragPos, grid.GetCellCoords(mousePosOnGrid), grid);
        }
        else {
            return new List<Vector2Int>() { grid.GetCellCoords(mousePosOnGrid) };
        }
    }

    public void UpdateInput(MouseInput mouseInput) {
        Vector3 mousePosOnGrid = mouseInput.LastGroundHitPoint;
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            isDragging = true;
            startDragPos = grid.GetCellCoords(mousePosOnGrid);
        }    
        if(Input.GetKeyUp(KeyCode.Mouse0) && isDragging) {
            isDragging = false;
            Vector2Int endDragPos = grid.GetCellCoords(mousePosOnGrid);
            List<Vector2Int> draggedSubgrid = GetPositionsInBox(startDragPos, endDragPos, grid);
            foreach(Vector2Int pos in draggedSubgrid) {
                placementMode.TryPlaceModule(currentModule, pos, placementMode.CurrentFacing);
            }
        }
    }

    private List<Vector2Int> GetPositionsInBox(Vector2Int startPos, Vector2Int endPos, FactoryGrid grid) {
        Vector2Int lowerLeft = new Vector2Int(Mathf.Min(startPos.x, endPos.x), Mathf.Min(startPos.y, endPos.y));
        Vector2Int upperRight = new Vector2Int(Mathf.Max(startPos.x, endPos.x), Mathf.Max(startPos.y, endPos.y));
        Vector2Int boxDimensions = upperRight - lowerLeft + Vector2Int.one;
        return grid.GetPositionsInSubgrid(lowerLeft, boxDimensions);
    }
}

public class DragPlacer : IPlacementStrategy {

    private bool isDragging;
    private Vector2Int lastPlacedPosition;
    private GridObjectSO gridObject;
    private Facing lastFacing;
    private bool lastObjectWasPlaced;
    private FactoryGrid grid;
    private PlacementMode placementMode;

    public DragPlacer(GridObjectSO gridObject, FactoryGrid grid, PlacementMode placementMode) {
        this.gridObject = gridObject;
        this.grid = grid;
        this.placementMode = placementMode;
    }

    public List<Vector2Int> GetHoveredPositions(Vector3 mousePosOnGrid) {
        return new List<Vector2Int>() { grid.GetCellCoords(mousePosOnGrid) };
    }

    public void UpdateInput(MouseInput mouseInput) {
        Vector3 mousePosOnGrid = mouseInput.LastGroundHitPoint;
        if (Input.GetKeyDown(KeyCode.Mouse0)) {
            isDragging = true;
            IGridObject placedObject = placementMode.TryPlaceModule(gridObject, mousePosOnGrid, placementMode.CurrentFacing);
            lastObjectWasPlaced = placedObject != null;
            lastFacing = placementMode.CurrentFacing;
            lastPlacedPosition = grid.GetCellCoords(mousePosOnGrid);
        }    
        if(Input.GetKeyUp(KeyCode.Mouse0) && isDragging) {
            isDragging = false;
        }
        if (isDragging) {
            Vector2Int currentMouseGridPos = grid.GetCellCoords(mousePosOnGrid);
            if(currentMouseGridPos != lastPlacedPosition) {
                Vector2Int dir = currentMouseGridPos - lastPlacedPosition;
                Facing facing = FacingFromDirection(dir);
                IGridObject placedObject = placementMode.TryPlaceModule(gridObject, currentMouseGridPos, facing);
                if(facing != lastFacing && lastObjectWasPlaced) {
                    grid.GetObjectAt(lastPlacedPosition).DestroyObject();
                    placementMode.TryPlaceModule(gridObject, lastPlacedPosition, facing);
                }
                lastPlacedPosition = currentMouseGridPos;
                lastFacing = facing;
                lastObjectWasPlaced = placedObject != null;
            }
        }
        placementMode.UpdateRotationInput(mouseInput);
    }

    private Facing FacingFromDirection(Vector2Int direction) {
        if(direction == Vector2Int.left) {
            return Facing.West;
        }
        else if(direction == Vector2Int.up) {
            return Facing.North;
        }
        else if(direction == Vector2Int.right) {
            return Facing.East;
        }
        else {
            return Facing.South;
        }
    }
}

public class NoPlacement : IPlacementStrategy {

    public NoPlacement() {
    
    }

    public List<Vector2Int> GetHoveredPositions(Vector3 mousePosOnGrid) {
        return new List<Vector2Int>();    
    }

    public void UpdateInput(MouseInput mouseInput) {
        
    }
}
