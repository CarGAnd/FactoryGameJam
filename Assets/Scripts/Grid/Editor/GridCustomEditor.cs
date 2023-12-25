using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Grid))]
public class GridCustomEditor : Editor
{
    private Grid targetGrid;

    private SerializedProperty gridRotation;
    private SerializedProperty gridOrigin;
    private SerializedProperty showGridLines;
    private SerializedProperty showOccupiedCells;

    private bool showDebug;

    private void OnEnable() {
        targetGrid = (Grid)target;
        gridRotation = serializedObject.FindProperty("<Rotation>k__BackingField");
        gridOrigin = serializedObject.FindProperty("<Origin>k__BackingField");
        showGridLines = serializedObject.FindProperty("showGridLines");
        showOccupiedCells = serializedObject.FindProperty("showOccupiedCells");
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();      
        showDebug = EditorGUILayout.Foldout(showDebug, "Debug");
        if (showDebug) {
            EditorGUI.indentLevel += 1;
            showGridLines.boolValue = EditorGUILayout.Toggle("Show Grid Lines", showGridLines.boolValue);
            showOccupiedCells.boolValue = EditorGUILayout.Toggle("Show Occupied Cells", showOccupiedCells.boolValue);
            EditorGUI.indentLevel -= 1;
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneGUI() {
        Vector3 pivotPosition = targetGrid.Origin + targetGrid.RotationPivot;
        switch (Tools.current) {
            case Tool.Move:
                Vector3 movedPos = Handles.PositionHandle(pivotPosition, targetGrid.Rotation);
                gridOrigin.vector3Value = movedPos - targetGrid.RotationPivot;
                break;
            case Tool.Rotate:
                Quaternion newRotation = Handles.RotationHandle(targetGrid.Rotation, pivotPosition);
                gridRotation.quaternionValue = newRotation;
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
