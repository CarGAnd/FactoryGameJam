using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class ModuleAssemblyController : MonoBehaviour, IAssemblyController<AssemblyLineObject>
{
    private Action<ITravelAssemblyLine<AssemblyLineObject>> recievedObject;
    public void Initialize() {
        GetAttachedAseemblies(gameObject);
    }

    private List<IAssembly> assemblies;
    public List<IAssembly> Assemblies { get => assemblies; private set => assemblies = value; }
    public Action<ITravelAssemblyLine<AssemblyLineObject>> RecievedObject { get => recievedObject; set => recievedObject = value; }

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
    public void ReceivedAssemblyObject(ITravelAssemblyLine<AssemblyLineObject> travelAssemblyLineObject)
    {
        RecievedObject?.Invoke(travelAssemblyLineObject);
    }
}
