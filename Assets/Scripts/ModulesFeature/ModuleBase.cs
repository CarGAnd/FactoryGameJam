using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModuleBase : MonoBehaviour
{
    protected ModuleAssemblyController<AssemblyObject> moduleAssemblyController;

    void Awake()
    {
        moduleAssemblyController = new ModuleAssemblyController<AssemblyObject>(gameObject);
        moduleAssemblyController.RecievedObject += OnReceivedObject;
    }

    protected abstract void OnReceivedObject(ITravelAssemblyLine<AssemblyObject> assemblyObject);

    protected void SendObject(ITravelAssemblyLine<AssemblyObject> AssemblyObject, int outputIndex = 0)
    {
        IAssembly assembly = moduleAssemblyController.GetOutputAssemblies()[outputIndex];

        if (assembly.ConnectedTo == null)
            return;

        AssemblyObject.StartTravel(assembly.ConnectedTo);
    }
}