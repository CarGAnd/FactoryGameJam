using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class GridSerializer
{
    private List<SavedObjectPlacement> SaveGrid(Grid grid, Func<IGridObject,string> getID) {
        HashSet<Vector2Int> visitedPositions = new HashSet<Vector2Int>();
        List<SavedObjectPlacement> savedObjects = new List<SavedObjectPlacement>();
        for(int x = 0; x < grid.Columns; x++) {
            for(int y = 0; y < grid.Rows; y++) {
                Vector2Int coord = new Vector2Int(x, y);
                if (grid.PositionIsOccupied(coord) && !visitedPositions.Contains(coord)) {
                    IGridObject obj = grid.GetObjectAt(coord);
                    string id = getID(obj);
                    Vector2Int originPosition = grid.GetObjectOriginCoord(coord);
                    SavedObjectPlacement savedObject = new SavedObjectPlacement
                    {
                        id = id,
                        xPosition = originPosition.x,
                        yPosition = originPosition.y
                    };
                    savedObjects.Add(savedObject);

                    List<Vector2Int> sharedPositions = grid.GetSharedPositions(coord);
                    foreach(Vector2Int position in sharedPositions) {
                        visitedPositions.Add(position);
                    }
                }
            }
        }
        return savedObjects;
    }    

    private void LoadGrid(List<SavedObjectPlacement> savedObjects, Action<string, Vector2Int> placeObject) {
        foreach(SavedObjectPlacement savedObject in savedObjects) {
            placeObject(savedObject.id, new Vector2Int(savedObject.xPosition, savedObject.yPosition));
        }
    }

    private string SerializeSavedObjects(List<SavedObjectPlacement> savedObjects) {
        return JsonConvert.SerializeObject(savedObjects);
    }

    private List<SavedObjectPlacement> DeserializeSavedObjects(string jsonData) {
        return JsonConvert.DeserializeObject<List<SavedObjectPlacement>>(jsonData);
    }

    public string GridToJson(Grid grid, Func<IGridObject,string> getID) {
        List<SavedObjectPlacement> savedObjects = SaveGrid(grid, getID);
        return SerializeSavedObjects(savedObjects);
    }

    public void JsonToGrid(string jsonData, Action<string, Vector2Int> placeObjects) {
        List<SavedObjectPlacement> savedObjects = DeserializeSavedObjects(jsonData);
        LoadGrid(savedObjects, placeObjects);
    }

    [System.Serializable]
    private struct SavedObjectPlacement {
        public string id;
        public int xPosition;
        public int yPosition;
    }
}

