using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModulePlacer
{
    private FactoryGrid grid;
    private AssemblyLineSystem assemblyLineSystem;

    public ModulePlacer(FactoryGrid grid, AssemblyLineSystem assemblyLineSystem) {
        this.grid = grid;
        this.assemblyLineSystem = assemblyLineSystem;
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
