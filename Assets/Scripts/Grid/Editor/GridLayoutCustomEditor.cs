using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GridLayout))]
public class GridLayoutCustomEditor : Editor 
{
    private GridLayout targetGridLayout;

    private SerializedProperty gridRotation;
    private SerializedProperty gridOrigin;
    
    private SerializedProperty showGridLines;
    private SerializedProperty debugGridSize;

    private bool showDebug;
    
    private void OnEnable() {
        targetGridLayout = (GridLayout)target;
        gridRotation = serializedObject.FindProperty("<Rotation>k__BackingField");
        gridOrigin = serializedObject.FindProperty("<Origin>k__BackingField");
        showGridLines = serializedObject.FindProperty("showGridLines");
        debugGridSize = serializedObject.FindProperty("debugGridSize");
    }

    public override void OnInspectorGUI() {
        DrawDefaultInspector();      
        showDebug = EditorGUILayout.Foldout(showDebug, "Debug");
        if (showDebug) {
            EditorGUI.indentLevel += 1;
            showGridLines.boolValue = EditorGUILayout.Toggle("Show Grid lines", showGridLines.boolValue);
            if (targetGridLayout.GetComponent<GridSize>() != null) {
                EditorGUILayout.HelpBox("Grid size is determined by GridSize component", MessageType.Info);
            }
            else {
                debugGridSize.vector2IntValue = EditorGUILayout.Vector2IntField("Debug grid size", debugGridSize.vector2IntValue);
            }
            EditorGUI.indentLevel -= 1;
        }
        serializedObject.ApplyModifiedProperties();
    }

    private void OnSceneGUI() {
        Vector3 pivotPosition = targetGridLayout.Origin + targetGridLayout.RotationPivot;
        switch (Tools.current) {
            case Tool.Move:
                Vector3 movedPos = Handles.PositionHandle(pivotPosition, targetGridLayout.Rotation);
                gridOrigin.vector3Value = movedPos - targetGridLayout.RotationPivot;
                break;
            case Tool.Rotate:
                Quaternion newRotation = Handles.RotationHandle(targetGridLayout.Rotation, pivotPosition);
                gridRotation.quaternionValue = newRotation;
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }

}
