using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElseGateModule : Module
{
    [SerializeField]
    private Property compareProperty;

    private OnAwake(){
        
    }
    override void OnRecievedObject(ITravelAssemblyLine<AssemblyObject> assemblyObject)
    {
        IAssembly output1 = moduleAssemblyController.GetOutputAssemblies()[0];
        IAssembly output2 = moduleAssemblyController.GetOutputAssemblies()[1];

        
        SendObject(assemblyObject);

    }
}
