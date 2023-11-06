using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public interface IAssembly
{
    public void SetAction<T>(Action<ITravelAssemblyLine<T>> action);
    public Action<ITravelAssemblyLine<T>> GetAction<T>();
    public bool IsConnected {get;}
    public bool IsOutput {get; set;}
    public IAssembly ConnectedTo {get;}
    IAssemblyTravelMethod AssemblyTravelMethod { get;}

    public abstract Vector3 GetTransformPosition();
    public abstract void Connect(BezierLineRenderer bezierLineRenderer, IAssembly _connectedTo);
    public abstract void Disconnect();
    public abstract List<Vector3> GetTravelPositions();
    public abstract void OnObjectArrived<T> (ITravelAssemblyLine<T> assemblyObject);
}
