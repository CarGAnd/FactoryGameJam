using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.UI;
using System;

public class TurnModule : ConfigurableModule
{
    [SerializeField] private Vector3 rotationToApply;

    private TMP_Dropdown valueDropdown;
    public override ModuleType ModuleType => ModuleType.TurnModule;

    private void Start() {
        ApplySettings();
    }

    protected override void OnReceivedObject(ITravelAssemblyLine<AssemblyObject> assemblyObject)
    {
        RotateProperty(assemblyObject.Value);
        SendObject(assemblyObject);
    }

    private void RotateProperty(AssemblyObject assemblyObject)
    {
        Properties objectProperties = assemblyObject.Properties;
        try
        {
            Quaternion currentRotation = objectProperties.GetProperty<Quaternion>(PropertyType.Rotation);
            //Debug.Log($"Rotation to apply is {rotationToApply}, and calculated new rocation is {currentRotation * Quaternion.Euler(rotationToApply)}.");
            objectProperties.SetProperty(PropertyType.Rotation, currentRotation * Quaternion.Euler(rotationToApply));
        }
        catch (PropertyNotFoundException)
        {
            objectProperties.SetProperty(PropertyType.Rotation, Quaternion.Euler(rotationToApply));
        }

        assemblyObject.ApplyProperty(PropertyType.Rotation);
    }

    public override void ApplySettings()
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
        base.ApplySettings();
    }

    protected override void SetUIElements()
    {
        base.SetUIElements();

        valueDropdown = UI.GetComponentInChildren<TMP_Dropdown>();;
    }
}
