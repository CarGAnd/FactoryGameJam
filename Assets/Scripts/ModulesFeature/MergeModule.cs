using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MergeModule : ConfigurableModule
{
    protected override void OnReceivedObject(ITravelAssemblyLine<AssemblyObject> assemblyObject)
    {
        SendObject(assemblyObject);
    }
}
