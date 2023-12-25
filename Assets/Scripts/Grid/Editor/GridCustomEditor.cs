using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
/*
[CustomEditor(typeof(Grid))]
public class GridCustomEditor : Editor
{
    private bool showGridLines;
    private bool showOccupiedCells;
    private Grid targetGrid;

    public override void OnInspectorGUI() {
        DrawDefaultInspector();
    }

    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected)]
    private static void DrawGizmos(Grid grid, GizmoType gizmoType) {
        //if (showGridLines) {
            Gizmos.color = Color.green;
            for (int y = 0; y < grid.Rows + 1; y++) {
                Vector3 start = grid.GetCellWorldPosition(new Vector2Int(0, y));
                Vector3 end = grid.GetCellWorldPosition(new Vector2Int(grid.Columns, y));
                Gizmos.DrawLine(start, end);
            }

            for (int x = 0; x < grid.Columns + 1; x++) {
                Vector3 start = grid.GetCellWorldPosition(new Vector2Int(x, 0));
                Vector3 end = grid.GetCellWorldPosition(new Vector2Int(x, grid.Rows));
                Gizmos.DrawLine(start, end);
            }
        //}

        //if (showOccupiedCells) {
            Gizmos.color = Color.red;
            for (int y = 0; y < grid.Rows; y++) {
                for (int x = 0; x < grid.Columns; x++) {
                    if (!grid.PositionIsOccupied(new Vector2Int(x,y))) {
                        Vector3 pos = grid.GetCellCenter(new Vector2Int(x, y));
                        Gizmos.DrawWireSphere(pos, Mathf.Min(grid.CellSize.x, grid.CellSize.y) / 3f);
                    }
                }
            }
        //}
    }
}*/
