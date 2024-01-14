using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridLoader : MonoBehaviour
{
    [SerializeField] private Grid targetGrid;
    [SerializeField] private ModulePlacer modulePlacer;
    [SerializeField] private GridObjectSO testObject;

    private GridSerializer gridSerializer;

    private string jsonData;

    [Button("Test Save")]
    private void Save() {
        gridSerializer = new GridSerializer();
        string saveData = gridSerializer.GridToJson(targetGrid, (IGridObject gridObject) => "");
        jsonData = saveData;
    }

    [Button("Test Load")]
    private void Load() {
        gridSerializer = new GridSerializer();
        gridSerializer.JsonToGrid(jsonData, (string id, Vector2Int position) => PlaceSavedObject(id, position));
    }

    [Button("Clear Grid")]
    private void ClearGrid() {
        for(int x = 0; x < targetGrid.Columns; x++) {
            for(int y = 0; y < targetGrid.Rows; y++) {
                targetGrid.RemoveObject(new Vector2Int(x, y));
            }
        }
    }

    private void PlaceSavedObject(string id, Vector2Int position) {
        modulePlacer.TryPlaceModule(testObject, position, Quaternion.identity);
    }
}
