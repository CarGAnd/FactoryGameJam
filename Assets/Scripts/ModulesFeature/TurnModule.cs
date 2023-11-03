using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnModule : ModuleBase
{
    [SerializeField]
    private Vector3 rotationToApply;

    protected override void OnReceivedObject(ITravelAssemblyLine<AssemblyObject> assemblyObject)
    {
        Properties objectProperties = assemblyObject.Value.Properties;
        try{
            Quaternion currentRotation = objectProperties.GetProperty<Quaternion>(PropertyType.Rotation);
            objectProperties.SetProperty(PropertyType.Rotation, currentRotation * Quaternion.Euler(rotationToApply));
        }
        catch(PropertyNotFoundException){
            objectProperties.SetProperty(PropertyType.Rotation, Quaternion.Euler(rotationToApply));
        }

        SendObject(assemblyObject);
    }
}
