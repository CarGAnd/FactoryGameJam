using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementModeVisuals : MonoBehaviour
{
    [SerializeField] private PlacementMode placementMode;
    [SerializeField] private PlayerModeManager playerModeManager;
    
    [Header("Prefabs")]
    [SerializeField] private GameObject indicatorPrefab;
    [SerializeField] private GameObject arrowPrefab;
    private GameObject gridPlanePrefab;

    private FactoryGrid buildGrid;
    private List<GameObject> indicatorObjects;
    private GameObject placementPreview;
    private GameObject arrowObject;
    private GameObject gridPlaneObject;
    private Vector2Int lastOriginCoord;

    private GridObjectSO selectedObjectData;
    private Vector2Int buildingDimensions;

    private void Awake() {
        buildGrid = playerModeManager.Grid;
        indicatorObjects = new List<GameObject>();
        for(int i = 0; i < 9; i++) {
            GameObject newIndicator = CreateIndicatorObject();
            indicatorObjects.Add(newIndicator);
        }
        CreateArrowObject();
        //CreateGridPlane();
    }

    private void OnEnable() {
        placementMode.moduleChanged.AddListener(OnModuleChanged);
        placementMode.moduleRotated.AddListener(OnModuleRotated);
        placementMode.enterPlacementMode.AddListener(OnEnterPlacementMode);
        placementMode.exitPlacementMode.AddListener(OnExitPlacementMode);
    }

    private void OnDisable() {
        placementMode.moduleChanged.RemoveListener(OnModuleChanged);
        placementMode.moduleRotated.RemoveListener(OnModuleRotated);
        placementMode.enterPlacementMode.RemoveListener(OnEnterPlacementMode);
        placementMode.exitPlacementMode.RemoveListener(OnExitPlacementMode);
    }

    private void OnEnterPlacementMode() {
        placementPreview.SetActive(true);
        arrowObject.SetActive(true);
        UpdateGroundIndicators();
    }

    private void OnExitPlacementMode() {
        placementPreview.SetActive(false);
        arrowObject.SetActive(false);
        SetActiveIndicatorCount(0);
    }

    private void OnModuleRotated() {
        if(selectedObjectData == null) {
            return;
        }

        Quaternion newRotation = placementMode.CurrentPlacementRotation;
        placementPreview.transform.rotation = newRotation;

        buildingDimensions = selectedObjectData.GetLayoutShapeDimensions(placementMode.CurrentFacing);
        
        UpdatePreviewPositions(lastOriginCoord, buildingDimensions);
    }

    private void OnModuleChanged(GridObjectSO newBuilding) {
        Destroy(placementPreview);
        arrowObject.SetActive(newBuilding != null);

        if(newBuilding == null) {
            SetActiveIndicatorCount(0);
            return;
        }
        selectedObjectData = newBuilding;

        buildingDimensions = selectedObjectData.GetLayoutShapeDimensions(placementMode.CurrentFacing);
        
        placementPreview = Instantiate(newBuilding.PreviewPrefab);
    
        UpdatePreviewPositions(lastOriginCoord, buildingDimensions);
        OnModuleRotated();
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

    private void Update() {
        UpdatePreview();
    }

    private void UpdatePreview() {
        if(selectedObjectData == null) {
            return;
        }

        Vector3 mouseHitPosition = placementMode.CurrentMouseWorldPos;
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
        UpdateArrowObject(buildingOriginCoord, buildingDimensions);
        
        lastOriginCoord = buildingOriginCoord;
    }

    private void UpdateGroundIndicators() {
        List<Vector2Int> hoveredPositions = placementMode.GetHoveredPositions();
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

    private void UpdateArrowObject(Vector2Int buildingOriginCoord, Vector2Int buildingDimensions) {
        Vector3 buildingCenter = buildGrid.GetSubgridCenter(buildingOriginCoord, buildingDimensions);
        Vector3 arrowDelta = placementMode.CurrentPlacementRotation * (new Vector3(-buildGrid.CellSize.x * buildingDimensions.x, 0, 0) / 2f + Vector3.left * buildGrid.CellSize.x / 2f);
        Vector3 arrowPosition = buildingCenter + arrowDelta;
        arrowObject.transform.position = arrowPosition;
        Quaternion moduleRot = placementMode.CurrentPlacementRotation;
        arrowObject.transform.rotation = Quaternion.Euler(90, 90, 0) * Quaternion.Euler(moduleRot.eulerAngles.x, moduleRot.eulerAngles.z, -moduleRot.eulerAngles.y);
    }

    private GameObject CreateIndicatorObject() {
        GameObject newIndicatorObject = Instantiate(indicatorPrefab);
        newIndicatorObject.transform.localScale = Vector3.one * buildGrid.CellSize;
        Quaternion oldRot = newIndicatorObject.transform.rotation;
        newIndicatorObject.transform.rotation = buildGrid.Rotation * oldRot;
        newIndicatorObject.transform.parent = transform;
        return newIndicatorObject;
    }

    private void CreateArrowObject() {
        arrowObject = Instantiate(arrowPrefab);
        arrowObject.transform.rotation = Quaternion.Euler(90, 90, 0);
    }

    private void CreateGridPlane() {
        gridPlaneObject = Instantiate(gridPlanePrefab);
        Vector3 gridSize = new Vector3(buildGrid.CellSize.x * buildGrid.Columns, 0, buildGrid.CellSize.y * buildGrid.Rows); 

        gridPlaneObject.transform.position = buildGrid.Origin + gridSize / 2 + Vector3.up * 0.01f;
        gridPlaneObject.transform.rotation = Quaternion.Euler(0, buildGrid.Rotation.eulerAngles.y, 0);
        gridPlaneObject.transform.localScale = gridSize / 10f;

        Material gridPlaneMat = gridPlaneObject.GetComponent<Renderer>().material;
        gridPlaneMat.SetFloat("_Rotation", buildGrid.Rotation.eulerAngles.y);
        gridPlaneMat.SetVector("_TileSize", new Vector4(buildGrid.CellSize.x, buildGrid.CellSize.y, 0, 0));
        gridPlaneMat.SetVector("_GridOffset", new Vector4(Mathf.Abs(buildGrid.Origin.x % buildGrid.CellSize.x), Mathf.Abs(buildGrid.Origin.z % buildGrid.CellSize.y)));
    }
}
