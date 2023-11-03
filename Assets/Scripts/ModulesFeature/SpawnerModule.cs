using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOS;

public class SpawnerModule : MonoBehaviour
{
    [SerializeField] private FactoryTracker factoryTracker;
    [SerializeField] private GameEvent spawnPhaseStartedEvent;
    [SerializeField] private float timeBetweenSpawns;
    [field: SerializeField] public int TotalNumSpawns { get; private set; }

    [SerializeField]
    private GameObject assemblyLinePrefab;
    private ModuleAssemblyController<AssemblyObject> moduleAssemblyController;
    // Start is called before the first frame update
    void Awake() {
        moduleAssemblyController = new ModuleAssemblyController<AssemblyObject>(gameObject);
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

    private void SendObject(ITravelAssemblyLine<AssemblyObject> assemblyLineObject) {
        IAssembly assembly = moduleAssemblyController.GetOutputAssemblies()[0];

        if (!assembly.IsConnected)
            return;

        assemblyLineObject.InitializeAtAssemblyLine(assembly, assemblyLineObject.Value.transform);
    }

    private IEnumerator StartSpawningObjects() {
        for(int i = 0; i < TotalNumSpawns; i++) {
            CreateAndSendObject();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }
}
