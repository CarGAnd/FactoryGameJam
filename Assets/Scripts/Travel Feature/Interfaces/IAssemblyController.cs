using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections;
using UnityEngine;

public interface IAssemblyController<T>
{
    public Action<ITravelAssemblyLine<T>> RecievedObject {get; set;}
    public List<IAssembly> Assemblies {get;}
    public abstract List<IAssembly> GetAllAssemblies();
    public abstract List<IAssembly> GetOutputAssemblies();
    public abstract List<IAssembly> GetIntakeAssemblies();
    public abstract bool DisconnectAssembly(int index);
    public abstract bool DisconnectAllAssemblies();
    public abstract void ReceivedAssemblyObject(ITravelAssemblyLine<T> travelAssemblyLineObject);
}
