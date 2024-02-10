using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FactoryGrid))]
public class GridInitializer : MonoBehaviour
{
    [SerializeField] private bool showGizmos;
    [SerializeField] private ModulePlacer modulePlacer;
    private FactoryGrid grid;
    [SerializeField] private List<PrePlacedObjectData> prePlacedObjects;
    

    // Start is called before the first frame update
    void Start()
    {
        grid = GetComponent<FactoryGrid>();
        foreach(PrePlacedObjectData objectData in prePlacedObjects) {
            modulePlacer.TryPlaceModule(objectData.objectDefinition, objectData.gridPosition, objectData.facing);
        }
    }

    private void OnDrawGizmos() {
        RemoveNullValues();
        if (!showGizmos) {
            return;
        }

        grid = GetComponent<FactoryGrid>();
        Gizmos.color = Color.yellow;
        foreach(PrePlacedObjectData objectData in prePlacedObjects) {
            Vector3 pos = grid.GetCellCenter(objectData.gridPosition);
            Gizmos.DrawCube(pos, new Vector3(grid.CellSize.x, 0.1f, grid.CellSize.y));
        }
    }

    private void RemoveNullValues() {
        for (int i = prePlacedObjects.Count - 1; i >= 0; i--) {
            if (prePlacedObjects[i] == null) {
                prePlacedObjects.RemoveAt(i);
            }
        }
    }

    public void AddNewObject(PrePlacedObjectData newObjectData) {
        prePlacedObjects.Add(newObjectData);
    }

    public void RemoveObject(PrePlacedObjectData objectToRemove) {
        prePlacedObjects.Remove(objectToRemove);
    }
}

