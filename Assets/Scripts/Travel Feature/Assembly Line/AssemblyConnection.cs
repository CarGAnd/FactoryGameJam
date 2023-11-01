using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssemblyConnection : MonoBehaviour, IAssembly
{
    [SerializeField]
    private bool isOutConnection = false;
    private bool isConnected = false;
    private BezierLineRenderer line;
    public bool IsConnected { get => isConnected; private set => isConnected = value; }
    public bool IsOutput { get => isOutConnection; set => isOutConnection = value; }

    public Vector3 GetTransformPosition()
    {
        return transform.position;
    }

    public void Connect(BezierLineRenderer bezierLineRenderer)
    {
        line = bezierLineRenderer;
        IsConnected = true;
    }

    public List<Vector3> GetTravelPositions() {
        return line.GetPositions();
    }
}
