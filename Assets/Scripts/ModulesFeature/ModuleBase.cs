using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ModuleAssemblyController))]
public abstract class ModuleBase : MonoBehaviour
{
    protected ModuleAssemblyController moduleAssemblyController;

    void Awake()
    {
        moduleAssemblyController = GetComponent<ModuleAssemblyController>();
        moduleAssemblyController.Initialize();
        moduleAssemblyController.RecievedObject += OnReceivedObject;
    }

    protected abstract void OnReceivedObject(ITravelAssemblyLine<AssemblyObject> assemblyObject);

    protected void SendObject(ITravelAssemblyLine<AssemblyObject> AssemblyObject, int outputIndex = 0)
    {
        IAssembly assembly = moduleAssemblyController.GetOutputAssemblies()[outputIndex];

        if (!assembly.IsConnected)
            return;

        AssemblyObject.StartTravel(assembly.GetTravelPositions());
    }
}