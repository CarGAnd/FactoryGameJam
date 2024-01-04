using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisual : MonoBehaviour
{
    [SerializeField] private MouseInput mouseInput;
    [SerializeField] private GameObject indicatorPrefab;
    [SerializeField] private BuildingSelector buildingSelector;

    private Grid buildGrid;
    private GameObject indicatorObject;
    private GridObjectSO selectedObjectData;

    private void Awake() {
        buildGrid = mouseInput.BuildGrid;
        CreateIndicatorObject();
    }

    private void OnEnable() {
        buildingSelector.selectedObjectChanged.AddListener(OnSelectedBuildingChanged);
    }

    private void OnDisable() {
        buildingSelector.selectedObjectChanged.RemoveListener(OnSelectedBuildingChanged);
    }

    private void OnSelectedBuildingChanged(GridObjectSO newBuilding) {
        indicatorObject.transform.localScale = new Vector3(newBuilding.width * buildGrid.CellSize.x, newBuilding.height * buildGrid.CellSize.y, 1);
        selectedObjectData = newBuilding;
    }

    private void SetBuildMode(bool active) {
        indicatorObject.SetActive(active);
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
        Vector3 newPosition = buildGrid.GetSubgridCenter(subgridOriginCoord, buildingDimensions);
        indicatorObject.transform.position = newPosition + Vector3.up * 0.01f;
        bool isOccupied = false;
        List<Vector2Int> positions = buildGrid.GetPositionsInSubgrid(subgridOriginCoord, buildingDimensions);
        foreach(Vector2Int position in positions) {
            if (buildGrid.PositionIsOccupied(position)) {
                isOccupied = true;
                break;
            }
        }
        Color color = isOccupied ? Color.red : Color.green;
        color = new Vector4(color.r, color.g, color.b, 0.6f);
        indicatorObject.GetComponent<MeshRenderer>().material.color = color;
    }

    private void CreateIndicatorObject() {
        indicatorObject = Instantiate(indicatorPrefab);
        indicatorObject.transform.localScale = Vector3.one * buildGrid.CellSize;
        Vector3 oldRot = indicatorObject.transform.rotation.eulerAngles;
        indicatorObject.transform.rotation = Quaternion.Euler(oldRot.x, buildGrid.Rotation.eulerAngles.y, oldRot.z);
    }
}
