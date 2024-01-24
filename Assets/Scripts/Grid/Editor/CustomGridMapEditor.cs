using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector;

[CustomEditor(typeof(CustomGridMap))]
public class CustomGridMapEditor : Editor
{
    private SerializedProperty factoryGrid;
    private SerializedProperty customSpawnMask;
    private SerializedProperty showCustomLayout;

    private CustomGridMap customLayout;
    private FactoryGrid grid;

    private bool isPainting;
    private bool leftMousePressed;
    private bool rightMousePressed;

    private void OnEnable() {
        customLayout = (CustomGridMap)target;
        factoryGrid = serializedObject.FindProperty("grid");
        grid = (FactoryGrid)factoryGrid.objectReferenceValue;
        customSpawnMask = serializedObject.FindProperty("customSpawnMask");
        showCustomLayout = serializedObject.FindProperty("showCustomLayout");
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
        DrawPaintButton();

        if (isPainting) {
            if(GUILayout.Button("Fill Grid")) {
                FillGrid();
            }

            if(GUILayout.Button("Clear Grid")) {
                ClearGrid();
            }
        }

        /*if(grid.Rows != currentHeight || grid.Columns != currentWidth) {
            bool[] newMask = new bool[grid.Rows * grid.Columns];
            for(int y = 0; y < Mathf.Min(currentHeight, grid.Rows); y++) {
                for(int x = 0; x < Mathf.Min(currentWidth, grid.Columns); x++) {
                    newMask[y * grid.Columns + x] = GetMaskValue(x, y);
                }
            }
            customSpawnMask = newMask;
            currentWidth = grid.Columns;
            currentHeight = grid.Rows;
        }   */
        serializedObject.ApplyModifiedProperties();
    }

    private void SetAllGridValues(bool value) {
        for(int i = 0; i < customSpawnMask.arraySize; i++) {
            customSpawnMask.GetArrayElementAtIndex(i).boolValue = value;
        }
    }

    private void FillGrid() {
        SetAllGridValues(true);
    }

    private void ClearGrid() {
        SetAllGridValues(false);
    }

    private void TogglePaintMode() {
        isPainting = !isPainting;
        if (isPainting) {
            showCustomLayout.boolValue = true;
        }
    }

    private void DrawPaintButton() {
        Color guiColor = GUI.color;
        Color c = isPainting ? Color.red : Color.green;
        string text = isPainting ? "Stop Painting" : "Start Painting";
        GUI.color = c;

        if(GUILayout.Button(text)) {
            TogglePaintMode();
        }

        GUI.color = guiColor;
    }

    private void OnSceneGUI() {
        if (!isPainting) {
            return;
        }
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        Event e = Event.current;

        if (e.type == EventType.MouseDown && e.button == 0) {
            leftMousePressed = true;
            e.Use();
        }
        if (e.type == EventType.MouseUp && e.button == 0) {
            leftMousePressed = false;
            e.Use();
        }

        if (e.type == EventType.MouseDown && e.button == 1) {
            rightMousePressed = true;
            e.Use();
        }
        if (e.type == EventType.MouseUp && e.button == 1) {
            rightMousePressed = false;
            e.Use();
        }

        if (leftMousePressed) {
            Paint(e.mousePosition, true);
        }

        if (rightMousePressed) {
            Paint(e.mousePosition, false);
        }
    }

    private void Paint(Vector2 clickPosition, bool value) {
        Ray ray = HandleUtility.GUIPointToWorldRay(clickPosition);
        Plane plane = new Plane(grid.Rotation * Vector3.up, grid.Origin);
        plane.Raycast(ray, out float distance);
        Vector3 worldPosition = ray.GetPoint(distance);
        Vector2Int gridPosition = grid.GetCellCoords(worldPosition);
        customLayout.SetMaskValue(gridPosition.x, gridPosition.y, value);
        //Manually update the scene view as the update rate would otherwise be very choppy
        EditorWindow view = EditorWindow.GetWindow<SceneView>();
        view.Repaint();
    }
}
