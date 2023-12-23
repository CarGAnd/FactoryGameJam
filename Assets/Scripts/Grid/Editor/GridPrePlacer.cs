using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridPrePlacer : OdinEditorWindow {

    [MenuItem("Grid/Snapping")]
    private static void OpenWindow() {
        GetWindow<GridPrePlacer>().Show();
    }

    [HorizontalGroup("row1")]
    public Grid snappingGrid;

    [HorizontalGroup("row2"), SerializeField]
    private bool enableSnapping;

    [Button("Find in scene")]
    private void FindGridInScene() {
        snappingGrid = FindObjectOfType<Grid>();
    }

    [HorizontalGroup("row3"), SerializeField]
    private SnapPosition snapPosition;

    [HorizontalGroup("row4")]
    public List<GameObject> objects;

    [Button("Place items on grid")]
    private void PlaceSelectedItemsOnGrid() { 
        foreach(GameObject g in objects) {
            PrePlaceObject(g);
        }
    }

    private void PrePlaceObject(GameObject g) {
        GridInitializer initializer = snappingGrid.gameObject.GetComponent<GridInitializer>();
        if (!initializer.prePlacedObjects.Contains(g)) {
            initializer.prePlacedObjects.Add(g);
        }
    }

    void OnFocus() {
        // Remove delegate listener if it has previously
        // been assigned.
        SceneView.duringSceneGui -= this.OnSceneGUI;

        // Add (or re-add) the delegate.
        SceneView.duringSceneGui += this.OnSceneGUI;
    }

    private void OnDestroy() {
        // When the window is destroyed, remove the delegate
        // so that it will no longer do any drawing.
        SceneView.duringSceneGui -= this.OnSceneGUI;
        if (Tools.current == Tool.None) {
            Tools.current = Tool.Move;
        }
    }

    private bool CanSnapToGrid() {
        return snappingGrid != null && enableSnapping && Selection.activeGameObject != null;
    }

    void OnSceneGUI(SceneView sceneView) {
        if (!CanSnapToGrid()) {
            return;
        }

        if(Tools.current == Tool.Move) {
            Tools.current = Tool.None;
        }

        if (Tools.current == Tool.None) {
            GameObject g = Selection.activeGameObject;
            IGridObject gridObject = g.GetComponent<IGridObject>();
            Vector3 position = Handles.PositionHandle(g.transform.position, g.transform.rotation);
            if (position != g.transform.position) {
                Undo.RecordObject(g.transform, "moved object");
                g.transform.position = GetSnappedPosition(position);
                if (!EditorApplication.isPlaying) {
                    PrePlaceObject(g);
                }
            }
        }
    }

    private Vector3 GetSnappedPosition(Vector3 position) {
        if(snapPosition == SnapPosition.CENTER) {
            return snappingGrid.GetCellCenter(position);
        }
        else {
            return snappingGrid.GetCellWorldPosition(position);
        }
    }

    private enum SnapPosition {
        CENTER,
        CORNER
    }
}
