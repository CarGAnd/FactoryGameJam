using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PrePlacedObjectData))]
public class PrePlacedObjectCustomEditor : Editor 
{
    //The current tool is shared among all objects to prevent it from resetting when selecting another object
    private static Tool currentTool;

    private FactoryGrid snappingGrid;
    private PrePlacedObjectData objectData;

    private void OnEnable() {
        snappingGrid = (FactoryGrid) serializedObject.FindProperty("grid").objectReferenceValue;
        objectData = (PrePlacedObjectData)target;
    }

    void OnSceneGUI() {
        if(Tools.current == Tool.Move) {
            currentTool = Tool.Move;
            Tools.current = Tool.None;
        }
        else if(Tools.current == Tool.Rotate) {
            currentTool = Tool.Rotate;
            Tools.current = Tool.None;
        }

        if(currentTool == Tool.Move) {
            HandleMoveTool();
        }
        else if(currentTool == Tool.Rotate) {
            HandleRotateTool();
        }
    }

    private void HandleMoveTool() {
        GameObject g = objectData.gameObject;
        Vector3 position = Handles.PositionHandle(g.transform.position, g.transform.rotation);
        if (position != g.transform.position) {
            Undo.RecordObject(g.transform, "moved object");
            g.transform.position = GetSnappedPosition(position);
            Vector2Int gridPosition = snappingGrid.GetCellCoords(position);
            objectData.gridPosition = gridPosition;
        }
    }

    private void HandleRotateTool() {
        GameObject g = objectData.gameObject;
        Quaternion gridAlignedRotation = snappingGrid.Rotation * objectData.facing.GetRotationFromFacing();
        Quaternion rotation = Handles.RotationHandle(gridAlignedRotation, g.transform.position);
        if (rotation != gridAlignedRotation) {
            //TODO: implement this in a proper way. Right now it bugs out after a full rotation
            Undo.RecordObject(g.transform, "Rotated object");
            float yRotDiff = (Quaternion.Inverse(gridAlignedRotation) * rotation).y * Mathf.Rad2Deg * 2;
            if(yRotDiff > 45) {
                Facing newFacing = objectData.facing.RotatedDirection(1);
                g.transform.rotation = snappingGrid.Rotation * newFacing.GetRotationFromFacing();
                objectData.facing = newFacing;
            }
            else if(yRotDiff < -45) {
                Facing newFacing = objectData.facing.RotatedDirection(-1);
                g.transform.rotation = snappingGrid.Rotation * newFacing.GetRotationFromFacing();
                objectData.facing = newFacing;
            }
        }
    }

    private Vector3 GetSnappedPosition(Vector3 position) {
        return snappingGrid.GetCellCenter(position);
    }

    
}
