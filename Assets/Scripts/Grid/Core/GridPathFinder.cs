using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPathFinder
{
   
}

public class BFSPathFind {
    public List<Vector2Int> FindPathAsCoordinates(Grid grid, Vector2Int startCell, Vector2Int endCell) {
        if (startCell == endCell) {
            //if start and end are the same the path is just the starting position
            return new List<Vector2Int>() { startCell };
        }

        if (grid.PositionIsOccupied(endCell)) {
            //If the goal cell is occupied it is impossible to find a path
            return null;
        }

        Queue<PathCell> frontier = new Queue<PathCell>();
        frontier.Enqueue(new PathCell(null, startCell));
        List<Vector2Int> visited = new List<Vector2Int>();

        while (frontier.Count > 0) {
            PathCell currentCell = frontier.Dequeue();
            Vector2Int currentCoord = currentCell.coord;
            List<Vector2Int> neighbors = grid.GetCellNeighbors(currentCoord);
            foreach (Vector2Int coord in neighbors) {
                if (!grid.CellWithinBounds(coord) || grid.PositionIsOccupied(coord)) {
                    continue;
                }
                PathCell cell = new PathCell(currentCell, coord);
                if (coord == endCell) {
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

    private List<Vector2Int> GetPath(PathCell endCell) {
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

    public List<Vector2Int> GetPath() {
        return pathCoords;
    }

    public List<Vector2Int> GetPathDirections() {
        List<Vector2Int> pathDirections = new List<Vector2Int>(pathCoords.Count);
        for(int i = 0; i < pathCoords.Count - 1; i++) {
            pathDirections[i] = pathCoords[i + 1] - pathCoords[i];
        }
        pathDirections[pathCoords.Count - 1] = pathDirections[pathCoords.Count - 2]; 

        return pathDirections;
    }

    public bool IsEmpty() {
        return pathCoords.Count == 0;
    }
}

