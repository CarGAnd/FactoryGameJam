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

    protected void SendObject(ITravelAssemblyLine<AssemblyObject> AssemblyObject, int outputIndex = 0)
    {
        IAssembly assembly = moduleAssemblyController.GetOutputAssemblies()[outputIndex];

        if (assembly.ConnectedTo == null)
            return;

        // Debug.Log($"Assembly at {gameObject.name} was not null.");
        AssemblyObject.InitializeAtAssemblyLine(assembly, AssemblyObject.Value.transform);
    }

    public void RemoveModule() {
        if (!ModuleIsRemoveable) {
            return;
        }

        List<IAssembly> inputs = moduleAssemblyController.GetIntakeAssemblies();
        List<IAssembly> outputs = moduleAssemblyController.GetIntakeAssemblies();

        foreach(IAssembly ia in inputs) {
            if (ia.IsConnected) {
                //RemoveConnection();
            }
        }

        foreach (IAssembly ia in outputs) {
            if (ia.IsConnected) {
                //RemoveConnection();
            }
        }

        Destroy(gameObject);
    }
}