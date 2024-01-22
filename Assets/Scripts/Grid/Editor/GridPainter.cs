using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridPainter : OdinEditorWindow {

    [MenuItem("Grid/Cell Painter")]
    private static void OpenWindow() {
        GetWindow<GridPainter>().Show();
    }

    public GridCellSpawner cellSpawner;
    public GridLayout gridLayout;

    private bool isPainting;
    private bool leftMousePressed;
    private bool rightMousePressed;

    void OnFocus() {
        // Remove delegate listener if it has previously
        // been assigned.
        SceneView.duringSceneGui -= this.OnSceneGUI;

        // Add (or re-add) the delegate.
        SceneView.duringSceneGui += this.OnSceneGUI;
        cellSpawner = FindFirstObjectByType<GridCellSpawner>();
        gridLayout = FindFirstObjectByType<GridLayout>();
    }

    private void TogglePaintMode() {
        isPainting = !isPainting;
    }

    [OnInspectorGUI]
    private void DrawStatus() {
        Color guiColor = GUI.color;
        Color c = isPainting ? Color.red : Color.green;
        string text = isPainting ? "Stop Painting" : "Start Painting";
        GUI.color = c;

        if(GUILayout.Button(text)) {
            TogglePaintMode();
        }

        GUI.color = guiColor;
    }


    private void OnDestroy() {
        // When the window is destroyed, remove the delegate
        // so that it will no longer do any drawing.
        SceneView.duringSceneGui -= this.OnSceneGUI;
    }

    [Button("Fill Grid")]
    private void ClearSpawnMask() {
        SetAllGridValues(false);
    }

    [Button("Clear grid")]
    private void FillSpawnMask() {
        SetAllGridValues(true);
    }

    private void SetAllGridValues(bool value) {
        
    }

    void OnSceneGUI(SceneView sceneView) {
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
        Plane plane = new Plane(gridLayout.Rotation * Vector3.up, gridLayout.Origin);
        plane.Raycast(ray, out float distance);
        Vector3 worldPosition = ray.GetPoint(distance);
        Vector2Int gridPosition = gridLayout.GetCellCoords(worldPosition);
        cellSpawner.SetMaskValue(gridPosition.x, gridPosition.y, value);
        //Manually update the scene view as the update rate would otherwise be very choppy
        EditorWindow view = EditorWindow.GetWindow<SceneView>();
        view.Repaint();
    }

}
