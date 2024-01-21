using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

public class GridSerializer<T> where T : ISaveable
{
    private List<SavedObjectData> SaveGrid(CellGrid<T> grid, Func<T, string> getID) {
        HashSet<Vector2Int> visitedPositions = new HashSet<Vector2Int>();
        List<SavedObjectData> savedObjects = new List<SavedObjectData>();
        for(int x = 0; x < grid.Columns; x++) {
            for(int y = 0; y < grid.Rows; y++) {
                Vector2Int coord = new Vector2Int(x, y);
                if (grid.PositionIsOccupied(coord) && !visitedPositions.Contains(coord)) {
                    T obj = grid.GetObjectAt(coord);
                    string id = getID(obj);
                    Vector2Int originPosition = grid.GetObjectOriginCoord(coord);
                    SavedObjectData savedObject = new SavedObjectData
                    {
                        id = id,
                        xPosition = originPosition.x,
                        yPosition = originPosition.y,
                        objectJsonData = obj.Serialize()
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

    private string SerializeSavedObjects(List<SavedObjectData> savedObjects) {
        return JsonConvert.SerializeObject(savedObjects);
    }

    private List<SavedObjectData> DeserializeSavedObjects(string jsonData) {
        return JsonConvert.DeserializeObject<List<SavedObjectData>>(jsonData);
    }

    public string GridToJson(CellGrid<T> grid, Func<T,string> getID) {
        List<SavedObjectData> savedObjects = SaveGrid(grid, getID);
        return SerializeSavedObjects(savedObjects);
    }

    public List<SavedObjectData> JsonToGrid(string jsonData) {
        List<SavedObjectData> savedObjects = DeserializeSavedObjects(jsonData);
        return savedObjects;
    }
}

[System.Serializable]
public struct SavedObjectData {
    public string id;
    public int xPosition;
    public int yPosition;
    public object objectJsonData;
}


public interface ISaveable {
    object Serialize();
    void Deserialize(object data);
}

