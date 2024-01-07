using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisual : MonoBehaviour
{
    [SerializeField] private MouseInput mouseInput;
    [SerializeField] private GameObject indicatorPrefab;
    [SerializeField] private ModulePlacer modulePlacer;

    private Grid buildGrid;
    private List<GameObject> indicatorObjects;
    private GameObject placementPreview;
    private Vector2Int lastOriginCoord;

    private GridObjectSO selectedObjectData;
    private List<Vector2Int> buildingLayoutShape;
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

        buildingLayoutShape = selectedObjectData.GetLayoutShape(modulePlacer.NumRotations);
        buildingDimensions = selectedObjectData.GetLayoutShapeDimensions(modulePlacer.NumRotations);
        
        UpdatePreviewPositions(lastOriginCoord, buildingDimensions);
    }

    private void OnModuleChanged(GridObjectSO newBuilding) {
        selectedObjectData = newBuilding;
        buildingLayoutShape = newBuilding.GetLayoutShape(modulePlacer.NumRotations);
        buildingDimensions = selectedObjectData.GetLayoutShapeDimensions(modulePlacer.NumRotations);
        
        Destroy(placementPreview);
        placementPreview = Instantiate(newBuilding.PreviewPrefab);
        
        int buildingArea = buildingLayoutShape.Count;
        while(indicatorObjects.Count < buildingArea) {
            CreateIndicatorObject();
        }

        for(int i = 0; i < buildingArea; i++) {
            indicatorObjects[i].SetActive(true);
        }

        for(int i = buildingArea; i < indicatorObjects.Count; i++) {
            indicatorObjects[i].SetActive(false);
        }
        
        UpdatePreviewPositions(lastOriginCoord, buildingDimensions);
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
        Vector3 subgridCenter = buildGrid.GetSubgridCenter(buildingOriginCoord, buildingDimensions);
        placementPreview.transform.position = subgridCenter;

        for(int i = 0; i < buildingLayoutShape.Count; i++) {
            Vector2Int buildPosition = buildingLayoutShape[i] + buildingOriginCoord;
            bool isOccupied = buildGrid.PositionIsOccupied(buildPosition);
            indicatorObjects[i].transform.position = buildGrid.GetCellCenter(buildPosition);
            Color color = isOccupied ? Color.red : Color.green;
            color = new Vector4(color.r, color.g, color.b, 0.6f);
            indicatorObjects[i].GetComponent<MeshRenderer>().material.color = color;
        }

        lastOriginCoord = buildingOriginCoord;
    }

    private void CreateIndicatorObject() {
        GameObject newIndicatorObject = Instantiate(indicatorPrefab);
        newIndicatorObject.transform.localScale = Vector3.one * buildGrid.CellSize;
        Vector3 oldRot = newIndicatorObject.transform.rotation.eulerAngles;
        newIndicatorObject.transform.rotation = Quaternion.Euler(oldRot.x, buildGrid.Rotation.eulerAngles.y, oldRot.z);
        indicatorObjects.Add(newIndicatorObject);
    }
}
