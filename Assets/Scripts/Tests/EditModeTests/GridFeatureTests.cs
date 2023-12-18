using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridFeatureTests
{
    /*
    [Test]
    public void GridObjectsCanBeSet() {
        Grid<int> g = new Grid<int>(5, 4, Vector3.zero, 3);
        int x = 2;
        int y = 3;
        int value = 17;
        g.SetObjectAt(x, y, value);

        Assert.True(g.GetObjectAt(x, y) == value);
        Assert.True(g.PositionIsOccupied(x, y));
    }

    [Test]
    public void EmptyCellsAreNotOccupied() {
        Grid<int> intGrid = new Grid<int>(5, 4, Vector3.zero, 3);

        Assert.True(intGrid.GetObjectAt(1, 1) == 0);
        Assert.False(intGrid.PositionIsOccupied(1, 1));

        Grid<GameObject> goGrid = new Grid<GameObject>(5, 4, Vector3.zero, 3);
        Assert.True(goGrid.GetObjectAt(1, 1) == null);
        Assert.False(goGrid.PositionIsOccupied(1, 1));
    }

    [Test]
    public void WorldToGridAndGridToWorldAreConsistent() {
        Grid<int> g = new Grid<int>(5, 4, Vector3.zero, 3);
        int x = 2;
        int y = 3;

        Vector3 worldPos = g.GridToWorld(x, y);
        Vector2Int gridCoords = g.WorldToGrid(worldPos);

        Assert.True(gridCoords.x == x && gridCoords.y == y);
    }*/
}
