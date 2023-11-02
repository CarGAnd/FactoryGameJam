using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOS;

public class SpawnerModule : MonoBehaviour
{
    [SerializeField] private GameEvent spawnPhaseStartedEvent;
    [SerializeField] private float timeBetweenSpawns;
    [SerializeField] private float totalNumSpawns;

    [SerializeField]
    private GameObject assemblyLinePrefab;
    private ModuleAssemblyController moduleAssemblyController;
    // Start is called before the first frame update
    void Awake() {
        moduleAssemblyController = GetComponent<ModuleAssemblyController>();
        moduleAssemblyController.Initialize();
    }

    private void OnEnable() {
        spawnPhaseStartedEvent.EventInvoked += StartSpawning;
    }

    private void OnDisable() {
        spawnPhaseStartedEvent.EventInvoked -= StartSpawning;
    }

    private void StartSpawning() {
        StartCoroutine(StartSpawningObjects());
    }

    private void CreateAndSendObject() {
        AssemblyLineObject assemblyLineObject = Instantiate(assemblyLinePrefab, transform.position, Quaternion.identity, transform).GetComponent<AssemblyLineObject>();
        SendObject(assemblyLineObject.TravelAssemblyLine);
    }

    private void SendObject(ITravelAssemblyLine<AssemblyLineObject> assemblyLineObject) {
        IAssembly assembly = moduleAssemblyController.GetOutputAssemblies()[0];

        if (!assembly.IsConnected)
            return;

        assemblyLineObject.StartTravel(assembly.GetTravelPositions());
    }

    private IEnumerator StartSpawningObjects() {
        for(int i = 0; i < totalNumSpawns; i++) {
            CreateAndSendObject();
            yield return new WaitForSeconds(timeBetweenSpawns);
        }
    }
}
