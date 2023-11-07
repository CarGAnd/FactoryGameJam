using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;

public class TurnModule : ModuleBase
{
    [SerializeField]
    private Vector3 rotationToApply;

    [SerializeField]
    private GameObject UIScreen;
    [SerializeField]
    private TMP_Dropdown valueDropdown;

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
        assemblyObject.Value.ApplyProperties();
        SendObject(assemblyObject);
    }

    public override void SelectModule()
    {
        UIScreen.SetActive(true);
        
    }

    public void ApplySettings()
    {
        Vector3 newRotation = new Vector3(0, 0, 0);
        switch(valueDropdown.value)
        {
            case 0:
                newRotation = new Vector3(0, 45, 0);
                break;
            case 1:
                newRotation = new Vector3(0, -45, 0);
                break;
        }
        
        rotationToApply = newRotation;
        UIScreen.SetActive(false);
        ModulesManager.Instance.DeselectModule();
    }
}
