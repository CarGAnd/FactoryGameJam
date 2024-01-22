using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisual : MonoBehaviour
{
    [SerializeField] private MouseInput mouseInput;
    [SerializeField] private GameObject indicatorPrefab;
    [SerializeField] private ModulePlacer modulePlacer;

    private FactoryGrid buildGrid;
    private List<GameObject> indicatorObjects;
    private GameObject placementPreview;
    private Vector2Int lastOriginCoord;

    private GridObjectSO selectedObjectData;
    private Vector2Int buildingDimensions;

    private void Awake() {
        indicatorObjects = new List<GameObject>();
        buildGrid = mouseInput.BuildGrid;
        for(int i = 0; i < 9; i++) {
            CreateIndicatorObject();
        }
    }

    private void OnEnable() {
        modulePlacer.moduleChanged.AddListener(OnModuleChanged);
        modulePlacer.moduleRotated.AddListener(OnModuleRotated);
    }

    private void OnDisable() {
        modulePlacer.moduleChanged.RemoveListener(OnModuleChanged);
        modulePlacer.moduleRotated.RemoveListener(OnModuleRotated);
    }

    private void OnModuleRotated() {
        if(selectedObjectData == null) {
            return;
        }

        Quaternion newRotation = modulePlacer.CurrentPlacementRotation;
        placementPreview.transform.rotation = newRotation;

        buildingDimensions = selectedObjectData.GetLayoutShapeDimensions(modulePlacer.NumRotations);
        
        UpdatePreviewPositions(lastOriginCoord, buildingDimensions);
    }

    private void OnModuleChanged(GridObjectSO newBuilding) {
        Destroy(placementPreview);

        if(newBuilding == null) {
            return;
        }

        selectedObjectData = newBuilding;
        buildingDimensions = selectedObjectData.GetLayoutShapeDimensions(modulePlacer.NumRotations);
        
        placementPreview = Instantiate(newBuilding.PreviewPrefab);
    
        UpdatePreviewPositions(lastOriginCoord, buildingDimensions);
    }

    private void SetActiveIndicatorCount(int newCount) {
        while (indicatorObjects.Count < newCount) {
            CreateIndicatorObject();
        }

        for (int i = 0; i < newCount; i++) {
            indicatorObjects[i].SetActive(true);
        }

        for (int i = newCount; i < indicatorObjects.Count; i++) {
            indicatorObjects[i].SetActive(false);
        }
    }

    private void Update() {
        UpdateIndicatorPosition();
    }

    private void UpdateIndicatorPosition() {
        if(selectedObjectData == null) {
            return;
        }

        Vector3 mouseHitPosition = mouseInput.LastGroundHitPoint;
        Vector2Int subgridOriginCoord = buildGrid.GetSubgridOriginCoord(mouseHitPosition, buildingDimensions);

        if(subgridOriginCoord == lastOriginCoord) {
            //Mouse has not moved enough to move the building
            return;
        }

        UpdatePreviewPositions(subgridOriginCoord, buildingDimensions);
    }

    private void UpdatePreviewPositions(Vector2Int buildingOriginCoord, Vector2Int buildingDimensions) {
        UpdatePreviewBuilding(buildingOriginCoord, buildingDimensions);
        UpdateGroundIndicators();
        
        lastOriginCoord = buildingOriginCoord;
    }

    private void UpdateGroundIndicators() {
        List<Vector2Int> hoveredPositions = modulePlacer.GetHoveredPositions();
        SetActiveIndicatorCount(hoveredPositions.Count);
        for(int i = 0; i < hoveredPositions.Count; i++) {
            Vector2Int buildPosition = hoveredPositions[i];
            bool isOccupied = buildGrid.PositionIsOccupied(buildPosition);
            indicatorObjects[i].transform.position = buildGrid.GetCellCenter(buildPosition);
            Color color = isOccupied ? Color.red : Color.green;
            color = new Vector4(color.r, color.g, color.b, 0.6f);
            indicatorObjects[i].GetComponent<MeshRenderer>().material.color = color;
        }
    }

    private void UpdatePreviewBuilding(Vector2Int buildingOriginCoord, Vector2Int buildingDimensions) {
        if(placementPreview != null) {
            Vector3 subgridCenter = buildGrid.GetSubgridCenter(buildingOriginCoord, buildingDimensions);
            placementPreview.transform.position = subgridCenter;
        }
    }

    private void CreateIndicatorObject() {
        GameObject newIndicatorObject = Instantiate(indicatorPrefab);
        newIndicatorObject.transform.localScale = Vector3.one * buildGrid.CellSize;
        Quaternion oldRot = newIndicatorObject.transform.rotation;
        newIndicatorObject.transform.rotation = buildGrid.Rotation * oldRot;
        newIndicatorObject.transform.parent = transform;
        indicatorObjects.Add(newIndicatorObject);
    }
}
