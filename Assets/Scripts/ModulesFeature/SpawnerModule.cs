using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOS;

public class SpawnerModule : ModuleBase
{
    [SerializeField] private FactoryTracker factoryTracker;
    [SerializeField] private GameEvent spawnPhaseStartedEvent;
    [SerializeField] private float timeBetweenSpawns;
    [field: SerializeField] public int TotalNumSpawns { get; private set; }

    [SerializeField]
    private GameObject assemblyLinePrefab;

    protected override void Awake() {
        base.Awake();
        ModuleIsRemoveable = false;
    }

    private void OnEnable() {
        spawnPhaseStartedEvent.EventInvoked += StartSpawning;
        factoryTracker.RegisterSpawner(this);
    }

    private void OnDisable() {
        spawnPhaseStartedEvent.EventInvoked -= StartSpawning;
        factoryTracker.DeregisterSpawner(this);
    }

    private void StartSpawning() {
        StartCoroutine(StartSpawningObjects());
    }

    private void CreateAndSendObject() {
        AssemblyObject assemblyLineObject = Instantiate(assemblyLinePrefab, transform.position, Quaternion.identity, transform).GetComponent<AssemblyObject>();
        SendObject(assemblyLineObject.TravelAssemblyLine);
    }

    private IEnumerator StartSpawningObjects() {
        for(int i = 0; i < TotalNumSpawns; i++) {
            CreateAndSendObject();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }

    protected override void OnReceivedObject(ITravelAssemblyLine<AssemblyObject> assemblyObject) {
        throw new System.NotImplementedException();
    }

    public override void SelectModule() {
        throw new System.NotImplementedException();
    }
}
