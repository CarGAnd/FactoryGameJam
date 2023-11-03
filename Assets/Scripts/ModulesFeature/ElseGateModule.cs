using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine.Rendering;

public class ElseGateModule : ModuleBase
{
    [SerializeField]
    private PropertyType propertyToCompare;

    [ShowIf("propertyToCompare", PropertyType.Color)]
    [SerializeField]
    private Color colorToCompare;

    [ShowIf("propertyToCompare", PropertyType.Rotation)]
    [SerializeField]
    private Vector3 rotationToCompare;

    private Quaternion rotationToCompareQuat;
    void Start(){
        if(propertyToCompare == PropertyType.Rotation){
            rotationToCompareQuat = Quaternion.Euler(rotationToCompare);
        }
        
    }
    protected override void OnReceivedObject(ITravelAssemblyLine<AssemblyObject> assemblyObject)
    {
        switch(propertyToCompare)
        {
            case PropertyType.Color:
                if(CompareProperty(assemblyObject.Value.Properties, colorToCompare))
                {
                    SendObject(assemblyObject);
                }
                else
                {
                    SendObject(assemblyObject, 1);
                }
                break;
            case PropertyType.Rotation:
                if(CompareProperty(assemblyObject.Value.Properties, rotationToCompareQuat))
                {
                    SendObject(assemblyObject);
                }
                else
                {
                    SendObject(assemblyObject, 1);
                }
                break;
            default:
                Debug.LogError("Unsupported property type: " + propertyToCompare);
                return;
        }
    }

    private bool CompareProperty(Properties property, object value)
    {
        return property.CompareProperty(propertyToCompare, value);
    }
}
