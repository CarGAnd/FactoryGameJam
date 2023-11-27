using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridVisual : MonoBehaviour
{
    [SerializeField] private ModulesManager moduleManager;
    [SerializeField] private GameObject indicatorPrefab;
    [SerializeField] private GameObject gridObject;

    private Material gridMaterial;
    private Grid<GameObject> buildGrid;
    private GameObject indicatorObject;

    private void Start() {
        gridMaterial = gridObject.GetComponent<Renderer>().material;
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
        gridObject.SetActive(active);
    }

    private void UpdateIndicatorPosition(Vector3 newPos) {
        indicatorObject.transform.position = newPos;
        Color color = moduleManager.CanPlaceModule ? Color.green : Color.red;
        color = new Vector4(color.r, color.g, color.b, 0.6f);
        indicatorObject.GetComponent<MeshRenderer>().material.color = color;
    }

    private void UpdateGrid() {
        Vector3 mouseHitPos = moduleManager.LastHitPoint;
        gridMaterial.SetVector("_CenterPos", new Vector4(mouseHitPos.x, mouseHitPos.y, mouseHitPos.z));
    }

    private void CreateIndicatorObject() {
        indicatorObject = Instantiate(indicatorPrefab);
        indicatorObject.transform.localScale = Vector3.one * buildGrid.CellSize;
        indicatorObject.SetActive(false);
        Vector3 oldRot = indicatorObject.transform.rotation.eulerAngles;
        indicatorObject.transform.rotation = Quaternion.Euler(oldRot.x, buildGrid.Rotation, oldRot.z);
    }
}
