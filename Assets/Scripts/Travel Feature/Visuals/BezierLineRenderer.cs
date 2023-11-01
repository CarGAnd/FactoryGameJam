using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class BezierLineRenderer : MonoBehaviour
{
    public LineRenderer Line;

    public void InitializeBezierCurve(Vector3[] positions, float bendFactor, int curveResolution) {
        if (Line == null) {
            Line = GetComponent<LineRenderer>();
        }
        
        transform.localScale = new Vector3(1,1,1);

        EasySmooth(positions, curveResolution, bendFactor);
    }

    public List<Vector3> GetPositions() {
        Vector3[] newPos = new Vector3[Line.positionCount];
        Line.GetPositions(newPos);
        return newPos.ToList();
    }

    private void EasySmooth(Vector3[] controlPoints, int curveResolution, float bendFactor) {
        if (controlPoints.Length != 4)
        {
            Debug.LogError("BezierCurve script requires exactly 4 control points.");
            return;
        }

        Line.positionCount = curveResolution + 1;

        // Calculate and set the positions of points on the curve
        for (int i = 0; i <= curveResolution; i++)
        {
            float t = (float)i / curveResolution;
            Vector3 point = CalculateCubicBezierPoint(t, controlPoints[0], controlPoints[1], controlPoints[2], controlPoints[3], bendFactor);
            Line.SetPosition(i, point);
        }
    }

    private Vector3 CalculateCubicBezierPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float bendFactor)
    {
        float u = 1 - t;
        float tt = t * t;
        float uu = u * u;
        float uuu = uu * u;
        float ttt = tt * t;

        Vector3 p = uuu * p0; // (1-t)^3 * P0
        p += 3 * uu * t * (p1 + (p1 - p0) * bendFactor); // 3(1-t)^2 * t * (P1 + (P1 - P0) * bendFactor)
        p += 3 * u * tt * (p2 + (p2 - p3) * bendFactor); // 3(1-t) * t^2 * (P2 + (P2 - P3) * bendFactor)
        p += ttt * p3; // t^3 * P3

        return p;
    }
}