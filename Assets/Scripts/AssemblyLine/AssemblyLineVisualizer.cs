using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssemblyLineVisualizer : MonoBehaviour
{
    public Grid grid;
    private void OnDrawGizmos()
    {
        if(AssemblyLineSystem.Instance != null)
        DrawAssemblyLines();
    }

    private void DrawAssemblyLines()
    {
        foreach (var line in AssemblyLineSystem.Instance.AssemblyLines)
        {
            DrawLine(line);
        }
    }

    private void DrawLine(AssemblyLine line)
    {
        
        Vector3 start = grid.GetCellWorldPosition(line.GetStartPiece().GetGridCoords());
        Vector3 end = grid.GetCellWorldPosition(line.GetEndPiece().GetGridCoords());

        Facing facing = line.GetStartPiece().facing;
        float heightOffset = 1f;
        AdjustLinePosition(ref start, ref end, facing, grid.CellSize, heightOffset);
        Color newColor = GetUniqueColor(line);
        Gizmos.color = newColor;
        Gizmos.DrawLine(start, end);

        // Draw circle at start
        Gizmos.DrawSphere(start, 0.75f);

        // Draw triangle at end
        DrawTriangle(end, Quaternion.LookRotation(end - start), 2.0f);

        foreach(AssemblyLine connectingLine in line.GetAllConnections())
        {
            DrawConnectingLine(connectingLine, newColor);
        }
    }

    private void DrawConnectingLine(AssemblyLine line, Color color)
    {
        Vector3 start = grid.GetCellWorldPosition(line.GetStartPiece().GetGridCoords());
        Vector3 end = grid.GetCellWorldPosition(line.GetEndPiece().GetGridCoords());

        Facing facing = line.GetStartPiece().facing;
        float heightOffset = 1f;
        AdjustLinePosition(ref start, ref end, facing, grid.CellSize, heightOffset);
        Gizmos.color = color;
        Gizmos.DrawLine(start, end);

        // Draw circle at start
        Gizmos.DrawSphere(start, 0.6f);

        // Draw triangle at end
        DrawTriangle(end, Quaternion.LookRotation(end - start), 1.25f);
        foreach(AssemblyLine connectingLine in line.GetAllConnections())
        {
            DrawConnectingLine(connectingLine, color);
        }
    }

    private Color GetUniqueColor(AssemblyLine line)
    {
        int index = AssemblyLineSystem.Instance.AssemblyLines.IndexOf(line);
        float hue = index * 0.618033988749895f % 1; // The golden ratio conjugate is used for better distribution
        return Color.HSVToRGB(hue, 0.7f, 0.9f); // Adjust saturation and value as needed
    }

    private void DrawTriangle(Vector3 position, Quaternion rotation, float size)
    {
        // Draw a triangle using Gizmos
        Vector3 direction = rotation * Vector3.forward;
        Vector3 right = rotation * Vector3.right;

        Vector3 vertex1 = position + direction * size;
        Vector3 vertex2 = position + right * size / 2;
        Vector3 vertex3 = position - right * size / 2;

        Gizmos.DrawLine(vertex1, vertex2);
        Gizmos.DrawLine(vertex2, vertex3);
        Gizmos.DrawLine(vertex3, vertex1);
    }
    private void AdjustLinePosition(ref Vector3 start, ref Vector3 end, Facing facing, Vector2 cellSize, float heightOffset)
    {
        start.z += cellSize.y / 2;
        end.z += cellSize.y / 2;
        start.x += cellSize.x / 2;
        end.x += cellSize.x / 2;

        // Apply height offset for all directions
        start.y = heightOffset;
        end.y = heightOffset;
    }


}

