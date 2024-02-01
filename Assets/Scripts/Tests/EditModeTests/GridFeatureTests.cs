using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GridSystem;

public class GridFeatureTests
{
    [Test]
    public void GridObjectsCanBeSet() {
        CellGrid<int> grid = new CellGrid<int>(5, 5);
        int value = 17;
        Vector2Int gridPosition = new Vector2Int(2, 3);
        grid.PlaceObject(value, gridPosition);

        Assert.True(grid.GetObjectAt(gridPosition) == value);
        Assert.True(grid.PositionIsOccupied(gridPosition));
    }

    [Test]
    public void GridObjectsCanSpanMultipleCells() {
        CellGrid<int> grid = new CellGrid<int>(5, 5);
        int value = 17;
        Vector2Int gridPosition = new Vector2Int(2, 3);
        List<Vector2Int> deltas = new List<Vector2Int>()
        {
            new Vector2Int(0,0),
            new Vector2Int(1,0),
            new Vector2Int(0,1),
            new Vector2Int(1,1)
        };
        grid.PlaceObject(value, gridPosition, deltas);

        foreach(Vector2Int delta in deltas) {
            Vector2Int pos = delta + gridPosition;
            Assert.True(grid.GetObjectAt(pos) == value);
            Assert.True(grid.PositionIsOccupied(pos));
            Assert.True(grid.GetObjectOriginCoord(pos) == gridPosition);
        }
    }

    [Test]
    public void EmptyCellsAreNotOccupied() {
        CellGrid<int> intGrid = new CellGrid<int>(5, 5);
        Vector2Int testPosition = new Vector2Int(1, 1);
        Assert.True(intGrid.GetObjectAt(testPosition) == 0);
        Assert.False(intGrid.PositionIsOccupied(testPosition));

        CellGrid<GameObject> goGrid = new CellGrid<GameObject>(5, 5);
        Assert.True(goGrid.GetObjectAt(testPosition) == null);
        Assert.False(goGrid.PositionIsOccupied(testPosition));
    }

    [Test]
    public void WorldToGridAndGridToWorldAreConsistent() {
        GameObject g = new GameObject();
        GridSystem.GridLayout layout = g.AddComponent<GridSystem.GridLayout>();
        layout.MoveGrid(new Vector3(1, 2, 3));
        layout.RotateGrid(Quaternion.Euler(new Vector3(34, 54, 17)));
        layout.AdjustCellSize(3, 5);

        Vector2Int gridPosition = new Vector2Int(4, 5);

        Vector3 worldPos = layout.GetCellWorldPosition(gridPosition);
        Vector2Int gridCoords = layout.GetCellCoords(worldPos);

        Assert.True(gridCoords.x == gridPosition.x && gridCoords.y == gridPosition.y);

        Vector3 worldPosCenter = layout.GetCellCenter(gridPosition);
        Vector2Int gridCoordsCenter = layout.GetCellCoords(worldPosCenter);

        Assert.True(gridCoordsCenter.x == gridPosition.x && gridCoordsCenter.y == gridPosition.y);
    }
}
