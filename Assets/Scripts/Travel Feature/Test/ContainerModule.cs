using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOS;
using static AssetProperties;

public class ContainerModule : ModuleBase
{
    [SerializeField] private IntRef scoreRef;
    [SerializeField] private List<PropertyEntry> properties;
    
    private Properties expectedProperties;

    [Button("Add Property")]
    public void AddNewProperty(PropertyType propertyType) {
        PropertyEntry newEntry = new PropertyEntry(propertyType);
        properties.Add(newEntry);
    }

    private void Start() {
        expectedProperties = new Properties();
        foreach (PropertyEntry entry in properties) {
            expectedProperties.SetProperty(entry.PropertyType, entry.GetValue());
        }
    }

    protected override void OnReceivedObject(ITravelAssemblyLine<AssemblyObject> assemblyObject) {
        AssemblyObject aObject = assemblyObject.Value;
        
        bool fitsContainer = aObject.Properties.CompareProperties(expectedProperties);

        if (fitsContainer) {
            CorrectObjectReceived(aObject);
        }
        else {
            WrongObjectReceived(aObject);
        }
    }

    private void CorrectObjectReceived(AssemblyObject aObject) {
        scoreRef.Value += 1;
    }

    private void WrongObjectReceived(AssemblyObject aObject) {
        Debug.Log("Wrong object received: " + aObject.name);
    }
}
