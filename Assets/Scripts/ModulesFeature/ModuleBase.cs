using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ModuleBase : MonoBehaviour
{
    protected ModuleAssemblyController<AssemblyObject> moduleAssemblyController;
    public bool ModuleIsRemoveable { get; protected set; }

    protected virtual void Awake()
    {
        ModuleIsRemoveable = true;
        moduleAssemblyController = new ModuleAssemblyController<AssemblyObject>(gameObject);
        moduleAssemblyController.RecievedObject += OnReceivedObject;
    }

    protected abstract void OnReceivedObject(ITravelAssemblyLine<AssemblyObject> assemblyObject);

    protected void SendObject(ITravelAssemblyLine<AssemblyObject> assemblyObject, int outputIndex = 0)
    {
        IAssembly assembly = moduleAssemblyController.GetOutputAssemblies()[outputIndex];

        if (assembly.ConnectedTo == null) {
            assemblyObject.Value.DestroyStuckObject();
            return;
        }
            

        // Debug.Log($"Assembly at {gameObject.name} was not null.");
        assemblyObject.InitializeAtAssemblyLine(assembly, assemblyObject.Value.transform);
    }

    public abstract void SelectModule();

    public int GetNumberOfConnectedOutputs() {
        int total = 0;
        List<IAssembly> outputs = moduleAssemblyController.GetOutputAssemblies();
        foreach(IAssembly ia in outputs) {
            if (ia.IsConnected) {
                total += 1;
            }
        }
        return total;
    }
}