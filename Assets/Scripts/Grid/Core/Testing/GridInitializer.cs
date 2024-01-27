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
            Destroy(objectData.previewObject);
        }
    }

    private void OnDrawGizmos() {
        if (!showGizmos) {
            return;
        }

        grid = GetComponent<FactoryGrid>();
        Gizmos.color = Color.yellow;
        foreach(PrePlacedObjectData objectData in prePlacedObjects) {
            Vector3 pos = grid.GetCellCenter(objectData.gridPosition);
            Gizmos.DrawWireSphere(pos, Mathf.Min(grid.CellSize.x, grid.CellSize.y) / 3f);
        }
    }

    public void PrePlaceObject(GridObjectSO objectDefinition, Facing facing, Vector2Int gridPosition) {
        Vector3 worldPosition = grid.GetCellCenter(gridPosition);
        GameObject previewObject = Instantiate(objectDefinition.PreviewPrefab, worldPosition, facing.GetRotationFromFacing());
        PrePlacedObjectData newData = new PrePlacedObjectData
        {
            previewObject = previewObject,
            facing = facing,
            objectDefinition = objectDefinition,
            gridPosition = gridPosition
        };
        prePlacedObjects.Add(newData);
    }

    [System.Serializable]
    private struct PrePlacedObjectData {
        public GameObject previewObject;
        public Facing facing;
        public GridObjectSO objectDefinition;
        public Vector2Int gridPosition;
    }
}

