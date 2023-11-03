using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITravelAssemblyLine<T>
{
    public IAssemblyTravelMethod AssemblyTravelMethod {
        get;
    }

    public T Value {
        get;
    }

    public abstract void OnTravelFinished();
    public abstract void InitializeAtAssemblyLine(IAssembly _assembly, Transform _transform);
    public abstract void UpdateTravel();
}
