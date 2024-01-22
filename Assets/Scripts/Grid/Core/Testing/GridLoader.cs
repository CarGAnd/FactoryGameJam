using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridLoader : MonoBehaviour
{
    [SerializeField] private FactoryGrid factoryGrid;
    [SerializeField] private ModulePlacer modulePlacer;
    [SerializeField] private GridObjectSO testObject;

    private string jsonData;

    [Button("Test Save")]
    private void Save() {
        string saveData = factoryGrid.SaveGrid();
        jsonData = saveData;
    }

    /*[Button("Test Load")]
    private void Load() {
        GridSerializer<IGridObject> serializer = new GridSerializer<IGridObject>();
        List<SavedObjectData> savedObjects = serializer.JsonToGrid(jsonData);
        foreach(SavedObjectData savedObject in savedObjects) {
            PlaceSavedObject(savedObject.id, new Vector2Int(savedObject.xPosition, savedObject.yPosition), savedObject.objectJsonData);
        }
    }*/

    [Button("Clear Grid")]
    private void ClearGrid() {
        for(int x = 0; x < factoryGrid.Columns; x++) {
            for(int y = 0; y < factoryGrid.Rows; y++) {
                IGridObject gridObject = factoryGrid.GetObjectAt(new Vector2Int(x, y));
                gridObject.DestroyObject();
            }
        }
    }

    /*private void PlaceSavedObject(string id, Vector2Int position, string jsonData) {
        IGridObject gridObject = modulePlacer.TryPlaceModule(testObject, position, Quaternion.identity);
        gridObject.Deserialize(jsonData);
    }*/
}
