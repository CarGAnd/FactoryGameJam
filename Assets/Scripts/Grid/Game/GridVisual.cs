using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisual : MonoBehaviour
{
    [SerializeField] private MouseInput mouseInput;
    [SerializeField] private GameObject indicatorPrefab;
    [SerializeField] private ModulePlacer modulePlacer;

    private Grid buildGrid;
    private GridObjectSO selectedObjectData;
    private List<GameObject> indicatorObjects;
    private GameObject placementPreview;
    private Vector2Int lastOriginCoord;

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
        if(placementPreview == null) {
            return;
        }
        Quaternion newRotation = modulePlacer.CurrentPlacementRotation;
        placementPreview.transform.rotation = newRotation;
    }

    private void OnModuleChanged(GridObjectSO newBuilding) {
        selectedObjectData = newBuilding;
        Destroy(placementPreview);
        placementPreview = Instantiate(newBuilding.PreviewPrefab);
        int buildingArea = newBuilding.Width * newBuilding.Height;
        while(indicatorObjects.Count < buildingArea) {
            CreateIndicatorObject();
        }

        for(int i = 0; i < buildingArea; i++) {
            indicatorObjects[i].SetActive(true);
        }

        for(int i = buildingArea; i < indicatorObjects.Count; i++) {
            indicatorObjects[i].SetActive(false);
        }
        
        Vector2Int newBuildingDimensions = selectedObjectData.GetLayoutShapeDimensions(modulePlacer.NumRotations);
        UpdatePreviewPositions(lastOriginCoord, newBuildingDimensions);
    }

    private void Update() {
        UpdateIndicatorPosition();
    }

    private void UpdateIndicatorPosition() {
        if(selectedObjectData == null) {
            return;
        }
        Vector3 mouseHitPosition = mouseInput.LastGroundHitPoint;
        Vector2Int buildingDimensions = selectedObjectData.GetLayoutShapeDimensions(modulePlacer.NumRotations);
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

        List<Vector2Int> positions = buildGrid.GetPositionsInSubgrid(buildingOriginCoord, buildingDimensions);
        for(int i = 0; i < positions.Count; i++) {
            bool isOccupied = buildGrid.PositionIsOccupied(positions[i]);
            indicatorObjects[i].transform.position = buildGrid.GetCellCenter(positions[i]);
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
