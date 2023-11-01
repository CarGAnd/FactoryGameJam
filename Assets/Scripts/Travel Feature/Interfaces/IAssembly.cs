using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public interface IAssembly
{
    public bool IsConnected {get;}
    public bool IsOutput {get; set;}

    public abstract Vector3 GetTransformPosition();
    public abstract void Connect(BezierLineRenderer bezierLineRenderer);
    public abstract List<Vector3> GetTravelPositions();
}
