using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOS;
using static AssetProperties;
using System;

public class ContainerModule : ModuleBase
{
    public Action OnItemCollected;

    [SerializeField] private FactoryTracker factoryTracker;
    [SerializeField] private IntRef scoreRef;

    [SerializeField] private LevelProperties levelProperties;

    [ShowIf("@PropertyDropdownHelper.IsColorAvailable(levelProperties)")]
    [ValueDropdown("@PropertyDropdownHelper.GetColorNames(levelProperties)")]
    public string colorName;
    
    [ShowIf("@PropertyDropdownHelper.IsRotationAvailable(levelProperties)")]
    [ValueDropdown("@PropertyDropdownHelper.GetRotationNames(levelProperties)")]
    public string rotationName;

    public int NumItemsCollected { get { return NumCorrectItemsCollected + NumWrongItemsCollected; } }
    public int NumCorrectItemsCollected { get; private set; }
    public int NumWrongItemsCollected { get; private set; }

    private Properties expectedProperties;
    private void OnEnable() {
        factoryTracker.RegisterContainer(this);
    }

    private void OnDisable() {
        factoryTracker.DeregisterContainer(this);
    }
    
    private void Start() {
        expectedProperties = Properties.CreateProperties(levelProperties, colorName, rotationName); 
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
        NumCorrectItemsCollected += 1;
        OnItemCollected?.Invoke();
    }

    private void WrongObjectReceived(AssemblyObject aObject) {
        NumWrongItemsCollected += 1;
        OnItemCollected?.Invoke();
        Debug.Log("Wrong object received: " + aObject.name);
    }
}
