using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GridBFS {
    private static List<Vector2Int> FindPath(Grid grid, Vector2Int startCell, Func<Vector2Int, bool> goalCondition, Func<Vector2Int, bool> isWalkableCell) {
        if (goalCondition(startCell)) {
            //if the condition is satisfied in the starting cell then the path is just the starting cell
            return new List<Vector2Int>() { startCell };
        }
        
        Queue<PathCell> frontier = new Queue<PathCell>();
        frontier.Enqueue(new PathCell(null, startCell));
        List<Vector2Int> visited = new List<Vector2Int>();

        while (frontier.Count > 0) {
            PathCell currentCell = frontier.Dequeue();
            Vector2Int currentCoord = currentCell.coord;
            List<Vector2Int> neighbors = grid.GetCellNeighbors(currentCoord);
            foreach (Vector2Int coord in neighbors) {
                if (!grid.CellWithinBounds(coord) || !isWalkableCell(coord)) {
                    continue;
                }
                PathCell cell = new PathCell(currentCell, coord);
                if (goalCondition(coord)) {
                    return GetPath(cell);
                }
                else if (!visited.Contains(coord)) {
                    frontier.Enqueue(cell);
                    visited.Add(coord);
                }
            }
        }
        //no path possible
        return null;
    }


    public static Path FindPath(Grid grid, Vector2Int startCoord, Vector2Int endCoord) {
        List<Vector2Int> positions = null;
        
        if(grid.PositionIsOccupied(endCoord)) {
            return new Path(positions);
        }
        
        positions = FindPath(grid, startCoord, (Vector2Int coord) => coord == endCoord, (Vector2Int coord) => !grid.PositionIsOccupied(coord));
        return new Path(positions);
    }

    public static Vector2Int FindClosestUnoccupiedCell(Grid grid, Vector2Int startCoord) {
        List<Vector2Int> path = FindPath(grid, startCoord, (Vector2Int coord) => !grid.PositionIsOccupied(coord), (Vector2Int coord) => true);
        return path[path.Count - 1];
    }

    private static List<Vector2Int> GetPath(PathCell endCell) {
        List<Vector2Int> pathList = new List<Vector2Int>();
        while (endCell != null) {
            pathList.Add(endCell.coord);
            endCell = endCell.prevCell;
        }
        pathList.Reverse();
        return pathList;
    }

    private class PathCell {
        public PathCell prevCell;
        public Vector2Int coord;

        public PathCell(PathCell prevCell, Vector2Int coord) {
            this.prevCell = prevCell;
            this.coord = coord;
        }
    }
}

public class Path {

    private List<Vector2Int> pathCoords;

    public Path(List<Vector2Int> pathCoords) {
        if(pathCoords == null) {
            this.pathCoords = new List<Vector2Int>();
        }
        else {
            this.pathCoords = pathCoords;
        }
    }

    public List<Vector2Int> GetPositions() {
        return pathCoords;
    }

    public List<Vector2Int> GetDirections() {
        if(pathCoords.Count == 1) {
            return new List<Vector2Int>() { Vector2Int.right };
        }
        List<Vector2Int> pathDirections = new List<Vector2Int>();
        for(int i = 0; i < pathCoords.Count - 1; i++) {
            pathDirections.Add(pathCoords[i + 1] - pathCoords[i]);
        }
        //We assume that the last object in the list has the same direction as the second last object, as we don't have a "next" object to compare to
        pathDirections.Add(pathDirections[pathCoords.Count - 2]); 

        return pathDirections;
    }

    public bool IsEmpty() {
        return pathCoords.Count == 0;
    }
}

