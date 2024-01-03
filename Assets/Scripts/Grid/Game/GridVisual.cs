using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisual : MonoBehaviour
{
    [SerializeField] private ModulesManager moduleManager;
    [SerializeField] private GameObject indicatorPrefab;

    private Grid buildGrid;
    private GameObject indicatorObject;

    private void Start() {
        buildGrid = moduleManager.BuildGrid;
        CreateIndicatorObject();
    }

    private void OnEnable() {
        moduleManager.MouseOverGridSpace.AddListener(UpdateIndicatorPosition);
        moduleManager.BuildModeToggled.AddListener(SetBuildMode);
    }

    private void OnDisable() {
        moduleManager.MouseOverGridSpace.RemoveListener(UpdateIndicatorPosition);
        moduleManager.BuildModeToggled.RemoveListener(SetBuildMode);
    }

    private void Update() {
        UpdateGrid();
    }

    private void SetBuildMode(bool active) {
        indicatorObject.SetActive(active);
    }

    private void UpdateIndicatorPosition(Vector3 newPos) {
        indicatorObject.transform.position = newPos;
        Color color = moduleManager.CanPlaceModule ? Color.green : Color.red;
        color = new Vector4(color.r, color.g, color.b, 0.6f);
        indicatorObject.GetComponent<MeshRenderer>().material.color = color;
    }

    private void UpdateGrid() {
        Vector3 mouseHitPos = moduleManager.LastHitPoint;
    }

    private void CreateIndicatorObject() {
        indicatorObject = Instantiate(indicatorPrefab);
        indicatorObject.transform.localScale = Vector3.one * buildGrid.CellSize;
        indicatorObject.SetActive(false);
        Vector3 oldRot = indicatorObject.transform.rotation.eulerAngles;
        indicatorObject.transform.rotation = Quaternion.Euler(oldRot.x, buildGrid.Rotation.eulerAngles.y, oldRot.z);
    }
}
