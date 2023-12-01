using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOS;
using System;
using UnityEngine.Events;

public class ContainerModule : ModuleBase
{
    //True if the objects properties matches the containers properties, otherwise false
    public UnityEvent<bool> ObjectArrivedAtContainer = new UnityEvent<bool>();

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
        runPhaseStartedEvent.OnInvoked += ResetCollectedCounts;
        factoryTracker.RegisterContainer(this);
    }

    private void OnDisable() {
        runPhaseStartedEvent.OnInvoked -= ResetCollectedCounts;
        factoryTracker.DeregisterContainer(this);
    }

    private void ResetCollectedCounts() {
        NumCorrectItemsCollected = 0;
        NumWrongItemsCollected = 0;
    }
    
    private void Start() {
        expectedProperties = Properties.CreateProperties(LevelDataGetter.GetCurrent().LevelProperties, colorName, rotationName); 
    }

    private void CollectObject(AssemblyObject aObject) {
        if (aObject.IsGhost) {
            return;
        }

        bool fitsContainer = aObject.Properties.CompareProperties(expectedProperties);

        if (fitsContainer) {
            CorrectObjectReceived();
        }
        else {
            WrongObjectReceived();
        }
    }
    
    protected override void OnReceivedObject(ITravelAssemblyLine<AssemblyObject> assemblyObject) {
        AssemblyObject aObject = assemblyObject.Value;

        CollectObject(aObject);

        //TODO: For now we destroy the object. Later we might want a more fancy animation
        aObject.DestroyObject();
    }

    private void CorrectObjectReceived() {
        //This check is only here because this object is null when running tests
        //TODO: have the score counting done somewhere else, likely using the onItemCollectedEvent
        if(scoreRef != null) {
            scoreRef.Value += 1;
        }
        NumCorrectItemsCollected += 1;
        ObjectArrivedAtContainer?.Invoke(true);
    }

    private void WrongObjectReceived() {
        NumWrongItemsCollected += 1;
        ObjectArrivedAtContainer?.Invoke(false);
    }

    public override void SelectModule() {
        throw new System.NotImplementedException();
    }
}
