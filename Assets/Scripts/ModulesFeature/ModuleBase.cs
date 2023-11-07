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

    public void DestroyModule() {
        if (!ModuleIsRemoveable) {
            return;
        }

        moduleAssemblyController.DisconnectAllAssemblies();
        Destroy(gameObject);

        ModulesManager.Instance.DeselectModule();
    }

    public abstract void SelectModule();
}