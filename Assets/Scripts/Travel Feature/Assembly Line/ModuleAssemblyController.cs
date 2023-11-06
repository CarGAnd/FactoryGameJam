using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class ModuleAssemblyController<T> : IAssemblyController<T>
{
    public ModuleAssemblyController (GameObject parent) {
        GetAttachedAseemblies(parent);
        SubscribeToAssemblies();
    }
    private Action<ITravelAssemblyLine<T>> recievedObject;
    private List<IAssembly> assemblies;
    public List<IAssembly> Assemblies { get => assemblies; private set => assemblies = value; }
    public Action<ITravelAssemblyLine<T>> RecievedObject { get => recievedObject; set => recievedObject = value; }

    private void GetAttachedAseemblies(GameObject parent)
    {
        assemblies = parent.GetComponentsInChildren<IAssembly>().ToList();
    }
    public virtual List<IAssembly> GetAllAssemblies() {
        return Assemblies;
    }
    public virtual List<IAssembly> GetOutputAssemblies() {
        return Assemblies.Where(assembly => assembly.IsOutput == true).ToList();
    }
    public virtual List<IAssembly> GetIntakeAssemblies() {
        return Assemblies.Where(assembly => assembly.IsOutput == false).ToList();
    }
    public void ReceivedAssemblyObject(ITravelAssemblyLine<T> travelAssemblyObject)
    {
        RecievedObject?.Invoke(travelAssemblyObject);
    }

    private void SubscribeToAssemblies() {
        if (assemblies == null || assemblies.Count <= 0)
            return;

        foreach (IAssembly assembly in assemblies) {
            assembly.SetAction<T>((assemblyObject) => recievedObject?.Invoke(assemblyObject)); 
        }
    }

    public bool DisconnectAssembly(int index)
    {
        if (Assemblies[index] == null)
            return false;

        Assemblies[index].Disconnect();
        return true;
    }

    public bool DisconnectAllAssemblies()
    {
        if (Assemblies == null || Assemblies.Count < 1)
            return false;

        foreach (IAssembly assembly in Assemblies) {
            assembly.Disconnect();
        }

        return true;
    }
}
