using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisual : MonoBehaviour
{
    [SerializeField] private MouseInput mouseInput;
    [SerializeField] private GameObject indicatorPrefab;
    [SerializeField] private ModulesManager moduleManager;

    private Grid buildGrid;
    private GameObject indicatorObject;

    private void Start() {
        buildGrid = mouseInput.BuildGrid;
        CreateIndicatorObject();
    }

    private void OnEnable() {
        mouseInput.MouseOverGridSpace.AddListener(UpdateIndicatorPosition);
        moduleManager.BuildModeToggled.AddListener(SetBuildMode);
    }

    private void OnDisable() {
        mouseInput.MouseOverGridSpace.RemoveListener(UpdateIndicatorPosition);
        moduleManager.BuildModeToggled.RemoveListener(SetBuildMode);
    }

    private void SetBuildMode(bool active) {
        indicatorObject.SetActive(active);
    }

    private void UpdateIndicatorPosition(Vector3 newPos) {
        indicatorObject.transform.position = newPos;
        Color color = buildGrid.PositionIsOccupied(buildGrid.GetCellCoords(newPos)) ? Color.red : Color.green;
        color = new Vector4(color.r, color.g, color.b, 0.6f);
        indicatorObject.GetComponent<MeshRenderer>().material.color = color;
    }

    private void CreateIndicatorObject() {
        indicatorObject = Instantiate(indicatorPrefab);
        indicatorObject.transform.localScale = Vector3.one * buildGrid.CellSize;
        indicatorObject.SetActive(false);
        Vector3 oldRot = indicatorObject.transform.rotation.eulerAngles;
        indicatorObject.transform.rotation = Quaternion.Euler(oldRot.x, buildGrid.Rotation.eulerAngles.y, oldRot.z);
    }
}
