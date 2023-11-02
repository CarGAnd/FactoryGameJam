using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ModuleAssemblyController))]
public class Module : MonoBehaviour
{
    private ModuleAssemblyController moduleAssemblyController;
    // Start is called before the first frame update
    void Awake()
    {
        moduleAssemblyController = GetComponent<ModuleAssemblyController>();
        moduleAssemblyController.Initialize();
        moduleAssemblyController.RecievedObject += OnRecievedObject;
    }

    abstract void OnRecievedObject(ITravelAssemblyLine<AssemblyObject> assemblyObject)
    {
        IAssembly assembly = moduleAssemblyController.GetOutputAssemblies()[0];
        SendObject(assemblyObject, assembly);
    }

    private void SendObject(ITravelAssemblyLine<AssemblyObject> AssemblyObject, IAssembly assembly) {
        if (!assembly.IsConnected)
            return;

        AssemblyObject.StartTravel(assembly.GetTravelPositions());
    }
}
