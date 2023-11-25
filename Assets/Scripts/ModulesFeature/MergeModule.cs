using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeModule : ConfigurableModule
{
    public override ModuleType ModuleType => ModuleType.MergeModule;

    protected override void OnReceivedObject(ITravelAssemblyLine<AssemblyObject> assemblyObject)
    {
        SendObject(assemblyObject);
    }
}
