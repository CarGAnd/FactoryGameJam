using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteModeVisuals : MonoBehaviour
{
    [SerializeField] private DeleteMode deleteMode;
    [SerializeField] private PlayerModeManager playerModeManager;
    [SerializeField] private GameObject indicatorPrefab;

    private FactoryGrid grid;
    private List<GameObject> indicatorObjects;

    private void Awake() {
        indicatorObjects = new List<GameObject>();
        grid = playerModeManager.Grid;
        SetActiveIndicatorCount(5);
    }

    private void OnEnable() {
        
    }

    private void OnDisable() {
        
    }

    private void Update() {
        Vector2Int gridPos = grid.GetCellCoords(deleteMode.LastMouseGridPosition);
        if (grid.CellWithinBounds(gridPos)) {
            UpdateGroundIndicators(gridPos); 
        }
    }

    private void UpdateGroundIndicators(Vector2Int hoveredPosition) {
        List<Vector2Int> sharedPositions = grid.GetSharedPositions(hoveredPosition);
        SetActiveIndicatorCount(sharedPositions.Count);
        for(int i = 0; i < sharedPositions.Count; i++) {
            Vector2Int buildPosition = sharedPositions[i];
            indicatorObjects[i].transform.position = grid.GetCellCenter(buildPosition);
        }
    }

    private void SetActiveIndicatorCount(int newCount) {
        while (indicatorObjects.Count < newCount) {
            GameObject newIndicator = CreateIndicatorObject();
            indicatorObjects.Add(newIndicator);
        }

        for (int i = 0; i < newCount; i++) {
            indicatorObjects[i].SetActive(true);
        }

        for (int i = newCount; i < indicatorObjects.Count; i++) {
            indicatorObjects[i].SetActive(false);
        }
    }

    private GameObject CreateIndicatorObject() {
        GameObject newIndicatorObject = Instantiate(indicatorPrefab);
        newIndicatorObject.transform.localScale = Vector3.one * grid.CellSize;
        Quaternion oldRot = newIndicatorObject.transform.rotation;
        newIndicatorObject.transform.rotation = grid.Rotation * oldRot;
        newIndicatorObject.transform.parent = transform;
        return newIndicatorObject;
    }
}
