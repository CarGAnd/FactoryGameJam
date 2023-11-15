using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOS;
using System;

public class ContainerModule : ModuleBase
{
    public Action OnItemCollected;

    [SerializeField] private FactoryTracker factoryTracker;
    [SerializeField] private IntRef scoreRef;
    [SerializeField] private GameEvent runPhaseStartedEvent;

    [ShowIf("@PropertyDropdownHelper.IsColorAvailable(LevelDataGetter.GetCurrent().LevelProperties)")]
    [ValueDropdown("@PropertyDropdownHelper.GetColorNames(LevelDataGetter.GetCurrent().LevelProperties)")]
    public string colorName;
    
    [ShowIf("@PropertyDropdownHelper.IsRotationAvailable(LevelDataGetter.GetCurrent().LevelProperties)")]
    [ValueDropdown("@PropertyDropdownHelper.GetRotationNames(LevelDataGetter.GetCurrent().LevelProperties)")]
    public string rotationName;

    public int NumItemsCollected { get { return NumCorrectItemsCollected + NumWrongItemsCollected; } }
    public int NumCorrectItemsCollected { get; private set; }
    public int NumWrongItemsCollected { get; private set; }

    private Properties expectedProperties;

    protected override void Awake() {
        base.Awake();
        ModuleIsRemoveable = false;
    }

    private void OnEnable() {
        runPhaseStartedEvent.EventInvoked += ResetCollectedCounts;
        factoryTracker.RegisterContainer(this);
    }

    private void OnDisable() {
        runPhaseStartedEvent.EventInvoked -= ResetCollectedCounts;
        factoryTracker.DeregisterContainer(this);
    }

    private void ResetCollectedCounts() {
        NumCorrectItemsCollected = 0;
        NumWrongItemsCollected = 0;
    }
    
    private void Start() {
        expectedProperties = Properties.CreateProperties(LevelDataGetter.GetCurrent().LevelProperties, colorName, rotationName); 
    }
    
    protected override void OnReceivedObject(ITravelAssemblyLine<AssemblyObject> assemblyObject) {
        AssemblyObject aObject = assemblyObject.Value;

        if (aObject.IsGhost) {
            Destroy(aObject.gameObject);
            return;
        }
        
        bool fitsContainer = aObject.Properties.CompareProperties(expectedProperties);

        if (fitsContainer) {
            CorrectObjectReceived(aObject);
        }
        else {
            WrongObjectReceived(aObject);
        }

        OnItemCollected?.Invoke();

        //TODO: For now we destroy the object. Later we might want a more fancy animation
        aObject.DestroyObject();
    }

    private void CorrectObjectReceived(AssemblyObject aObject) {
        scoreRef.Value += 1;
        NumCorrectItemsCollected += 1;
    }

    private void WrongObjectReceived(AssemblyObject aObject) {
        NumWrongItemsCollected += 1;
        Debug.Log("Wrong object received: " + aObject.name);
    }

    public override void SelectModule() {
        throw new System.NotImplementedException();
    }
}
