using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModuleAssemblyConnection : MonoBehaviour, IAssembly
{
    private Action<ITravelAssemblyLine<AssemblyObject>> receivedItem;

    [SerializeField]
    private bool isOutConnection = false;
    private bool isConnected = false;
    private IAssembly connectedTo;
    private BezierLineRenderer line;
    public bool IsConnected { get => isConnected; private set => isConnected = value; }
    public bool IsOutput { get => isOutConnection; set => isOutConnection = value; }
    public IAssembly ConnectedTo {get { return connectedTo; } private set {connectedTo = value; } }

    public Vector3 GetTransformPosition()
    {
        return transform.position;
    }

    public List<Vector3> GetTravelPositions() {
        return line.GetPositions();
    }

    public void Connect(BezierLineRenderer bezierLineRenderer, IAssembly _connectedTo)
    {
        line = bezierLineRenderer;
        ConnectedTo = _connectedTo;
        IsConnected = true;
    }

    public void OnObjectArrived<T>(ITravelAssemblyLine<T> assemblyObject)
    {
        GetAction<T>()?.Invoke(assemblyObject);
    }

    public void SetAction<T>(Action<ITravelAssemblyLine<T>> action)
    {
        if (typeof(T) == typeof(AssemblyObject))
            receivedItem = action as Action<ITravelAssemblyLine<AssemblyObject>>;
    }

    public Action<ITravelAssemblyLine<T>> GetAction<T>()
    {
        if (typeof(T) == typeof(AssemblyObject))
            return receivedItem as Action<ITravelAssemblyLine<T>>;

        return null;
    }

}
