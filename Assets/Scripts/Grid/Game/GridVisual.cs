using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisual : MonoBehaviour
{
    [SerializeField] private MouseInput mouseInput;
    [SerializeField] private GameObject indicatorPrefab;
    [SerializeField] private BuildingSelector buildingSelector;

    private Grid buildGrid;
    private GridObjectSO selectedObjectData;
    private List<GameObject> indicatorObjects;
    private GameObject placementPreview;

    private void Awake() {
        indicatorObjects = new List<GameObject>();
        buildGrid = mouseInput.BuildGrid;
        for(int i = 0; i < 9; i++) {
            CreateIndicatorObject();
        }
    }

    private void OnEnable() {
        buildingSelector.selectedObjectChanged.AddListener(OnSelectedBuildingChanged);
    }

    private void OnDisable() {
        buildingSelector.selectedObjectChanged.RemoveListener(OnSelectedBuildingChanged);
    }

    private void OnSelectedBuildingChanged(GridObjectSO newBuilding) {
        selectedObjectData = newBuilding;
        Destroy(placementPreview);
        placementPreview = Instantiate(newBuilding.previewPrefab);
        int buildingArea = newBuilding.width * newBuilding.height;
        while(indicatorObjects.Count < buildingArea) {
            CreateIndicatorObject();
        }

        for(int i = 0; i < buildingArea; i++) {
            indicatorObjects[i].SetActive(true);
        }

        for(int i = buildingArea; i < indicatorObjects.Count; i++) {
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
        Vector2Int buildingDimensions = selectedObjectData.GetLayoutShapeDimensions();
        Vector2Int subgridOriginCoord = buildGrid.GetSubgridOriginCoord(mouseHitPosition, buildingDimensions);
        //TODO: everything after here should only run if the subgridOriginCoord has changed between frames (i.e the mouse has moved enough to move the placement of the building)
        Vector3 subgridCenter = buildGrid.GetSubgridCenter(subgridOriginCoord, buildingDimensions);
        placementPreview.transform.position = subgridCenter;
        List<Vector2Int> positions = buildGrid.GetPositionsInSubgrid(subgridOriginCoord, buildingDimensions);
        for(int i = 0; i < positions.Count; i++) {
            bool isOccupied = buildGrid.PositionIsOccupied(positions[i]);
            indicatorObjects[i].transform.position = buildGrid.GetCellCenter(positions[i]);
            Color color = isOccupied ? Color.red : Color.green;
            color = new Vector4(color.r, color.g, color.b, 0.6f);
            indicatorObjects[i].GetComponent<MeshRenderer>().material.color = color;
        }
    }

    private void CreateIndicatorObject() {
        GameObject newIndicatorObject = Instantiate(indicatorPrefab);
        newIndicatorObject.transform.localScale = Vector3.one * buildGrid.CellSize;
        Vector3 oldRot = newIndicatorObject.transform.rotation.eulerAngles;
        newIndicatorObject.transform.rotation = Quaternion.Euler(oldRot.x, buildGrid.Rotation.eulerAngles.y, oldRot.z);
        indicatorObjects.Add(newIndicatorObject);
    }
}
