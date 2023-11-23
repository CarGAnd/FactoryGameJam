using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOS;
using Sirenix.OdinInspector;
using UnityEngine.Events;

public class SpawnerModule : ModuleBase
{
    public UnityEvent ObjectSpawned;

    [SerializeField] private FactoryTracker factoryTracker;
    [SerializeField] private GameEvent buildPhaseStartedEvent;
    [SerializeField] private GameEvent spawnPhaseStartedEvent;

    [SerializeField] private FloatRef timeBetweenSpawns;
    [SerializeField] private FloatRef timeBetweenGhostSpawns;

    [SerializeField] private List<SpawnWave> spawns;

    [SerializeField]
    private GameObject assemblyLinePrefab;

    private Spawner spawner;
    private GhostSpawner ghostSpawner;

    //Contains both ghost and normal objects for now
    private List<GameObject> spawnedObjects;

    protected override void Awake() {
        base.Awake();
        spawnedObjects = new List<GameObject>();
        ModuleIsRemoveable = false;
        spawner = new Spawner(spawns, assemblyLinePrefab);
        ghostSpawner = new GhostSpawner(spawns, assemblyLinePrefab);
    }

    private void OnEnable() {
        spawnPhaseStartedEvent.EventInvoked += StartSpawning;
        buildPhaseStartedEvent.EventInvoked += StartSpawningGhosts;
        factoryTracker.OnObjectDestroyed += OnObjectDestroyed;
        factoryTracker.RegisterSpawner(this);
    }

    private void OnDisable() {
        spawnPhaseStartedEvent.EventInvoked -= StartSpawning;
        buildPhaseStartedEvent.EventInvoked -= StartSpawningGhosts;
        factoryTracker.OnObjectDestroyed -= OnObjectDestroyed;
        factoryTracker.DeregisterSpawner(this);
    }

    private void StartSpawningGhosts() {
        ghostSpawner.Reset();
        StartCoroutine(SpawnGhostsCoroutine());
    }

    private IEnumerator SpawnGhostsCoroutine() {
        while (!ghostSpawner.IsFinished) {
            CreateAndSendObject(ghostSpawner);
            yield return new WaitForSeconds(timeBetweenGhostSpawns.Value);
        }
    }

    private void OnObjectDestroyed(GameObject obj) {
        spawnedObjects.Remove(obj);
    }

    private void DestroySpawnedObjects() {
        for(int i = spawnedObjects.Count - 1; i >= 0; i--) {
            Destroy(spawnedObjects[i].gameObject);
        }
        spawnedObjects = new List<GameObject>();
    }

    private void StartSpawning() {
        ghostSpawner.StopSpawning();
        DestroySpawnedObjects();
        spawner.Reset();
        StartCoroutine(SpawnObjectsCoroutine());
    }

    public int GetTotalNumSpawns() {
        int total = 0;
        for(int i = 0; i < spawns.Count; i++) {
            total += spawns[i].TotalNumSpawns;
        }
        return total;
    }

    private void CreateAndSendObject(Spawner spawner) {
        AssemblyObject assemblyLineObject = spawner.GetNextObject();
        assemblyLineObject.transform.position = transform.position;
        assemblyLineObject.transform.parent = transform;
        spawnedObjects.Add(assemblyLineObject.gameObject);
        SendObject(assemblyLineObject.TravelAssemblyLine);
    }

    private IEnumerator SpawnObjectsCoroutine() {
        while (!spawner.IsFinished) {
            CreateAndSendObject(spawner);
            ObjectSpawned?.Invoke();
            yield return new WaitForSeconds(timeBetweenSpawns.Value);
        }
    }

    protected override void OnReceivedObject(ITravelAssemblyLine<AssemblyObject> assemblyObject) {
        throw new System.NotImplementedException();
        //The spawner does not have any inputs, so this should never happen
    }

    public override void SelectModule() {
        throw new System.NotImplementedException();
    }
}

[System.Serializable]
public class SpawnWave {
    [field: SerializeField] public int TotalNumSpawns { get; private set; }

    public SpawnWave(int numSpawns) {
        this.TotalNumSpawns = numSpawns;
    }

    [ValueDropdown("@PropertyDropdownHelper.GetColorNames(LevelDataGetter.GetCurrent().LevelProperties)")]
    public string colorName;

    [ValueDropdown("@PropertyDropdownHelper.GetRotationNames(LevelDataGetter.GetCurrent().LevelProperties)")]
    public string rotationName;

    public Properties CreateProperties(LevelProperties levelProperties) {
        return Properties.CreateProperties(levelProperties, colorName, rotationName);
    }

    public Properties CreateProperties() {
        return Properties.CreateProperties(LevelDataGetter.GetCurrent().LevelProperties, colorName, rotationName);
    }
}

public class Spawner {

    public bool IsFinished { get; protected set; }

    protected List<SpawnWave> wavesToSpawn;

    protected SpawnWave currentWave;
    protected int currentWaveNumber;
    protected int spawnLeftThisWave;
    protected GameObject spawnPrefab;

    public Spawner(List<SpawnWave> waves, GameObject prefab) {
        this.wavesToSpawn = waves;
        this.spawnPrefab = prefab;
        Reset();
    }

    public void Reset() {
        StartWaveNumber(0);
        IsFinished = false;
    }

    protected void StartWaveNumber(int number) {
        currentWave = wavesToSpawn[number];
        currentWaveNumber = number;
        spawnLeftThisWave = currentWave.TotalNumSpawns;
    }

    protected AssemblyObject CreateObject() {
        AssemblyObject assemblyLineObject = MonoBehaviour.Instantiate(spawnPrefab).GetComponent<AssemblyObject>();
        Properties props = currentWave.CreateProperties();
        assemblyLineObject.Properties = props;
        return assemblyLineObject;
    }

    public virtual AssemblyObject GetNextObject() {
        AssemblyObject aObject = CreateObject();
        spawnLeftThisWave -= 1;

        if (spawnLeftThisWave <= 0) {
            if(currentWaveNumber + 1 < wavesToSpawn.Count) {
                StartWaveNumber(currentWaveNumber + 1);
            }
            else {
                IsFinished = true;
            }
            
        }
        
        return aObject;
    }
}

public class GhostSpawner : Spawner {

    public GhostSpawner(List<SpawnWave> waves, GameObject prefab) : base(waves, prefab) {

    }

    public override AssemblyObject GetNextObject() {
        AssemblyObject aObject = CreateObject();
        aObject.IsGhost = true;

        if (currentWaveNumber + 1 < wavesToSpawn.Count) {
            StartWaveNumber(currentWaveNumber + 1);
        }
        else {
            StartWaveNumber(0);
        }

        return aObject;
    }

    public void StopSpawning() {
        IsFinished = true;
    }
}
