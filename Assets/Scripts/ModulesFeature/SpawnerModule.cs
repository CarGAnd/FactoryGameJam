using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOS;
using Sirenix.OdinInspector;

public class SpawnerModule : ModuleBase
{
    [SerializeField] private FactoryTracker factoryTracker;
    [SerializeField] private GameEvent spawnPhaseStartedEvent;
    [SerializeField] private float timeBetweenSpawns;
    
    [SerializeField] private List<SpawnWave> spawns;

    [SerializeField]
    private GameObject assemblyLinePrefab;

    private Spawner spawner;

    protected override void Awake() {
        base.Awake();
        ModuleIsRemoveable = false;
        spawner = new Spawner(spawns);
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
        spawner.Reset();
        StartCoroutine(StartSpawningObjects());
    }

    public int GetTotalNumSpawns() {
        int total = 0;
        for(int i = 0; i < spawns.Count; i++) {
            total += spawns[i].TotalNumSpawns;
        }
        return total;
    }

    private void CreateAndSendObject() {
        AssemblyObject assemblyLineObject = Instantiate(assemblyLinePrefab, transform.position, Quaternion.identity, transform).GetComponent<AssemblyObject>();
        Properties props = spawner.GetNextObject();
        assemblyLineObject.Properties = props;
        SendObject(assemblyLineObject.TravelAssemblyLine);
    }

    private IEnumerator StartSpawningObjects() {
        while (!spawner.IsFinished) {
            CreateAndSendObject();
            yield return new WaitForSeconds(timeBetweenSpawns);
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

    [ValueDropdown("@PropertyDropdownHelper.GetColorNames(LevelDataGetter.GetCurrent().LevelProperties)")]
    public string colorName;

    [ValueDropdown("@PropertyDropdownHelper.GetRotationNames(LevelDataGetter.GetCurrent().LevelProperties)")]
    public string rotationName;

    public Properties CreateProperties() {
        return Properties.CreateProperties(LevelDataGetter.GetCurrent().LevelProperties, colorName, rotationName);
    }
}

public class Spawner {

    public bool IsFinished { get; private set; }

    private List<SpawnWave> wavesToSpawn;

    private SpawnWave currentWave;
    private int currentWaveNumber;
    private int spawnLeftThisWave;

    public Spawner(List<SpawnWave> waves) {
        this.wavesToSpawn = waves;
        Reset();
    }

    public void Reset() {
        StartWaveNumber(0);
        IsFinished = false;
    }

    private void StartWaveNumber(int number) {
        currentWave = wavesToSpawn[number];
        currentWaveNumber = number;
        spawnLeftThisWave = currentWave.TotalNumSpawns;
    }

    public Properties GetNextObject() {
        Properties props = currentWave.CreateProperties();
        spawnLeftThisWave -= 1;

        if (spawnLeftThisWave <= 0) {
            if(currentWaveNumber + 1 < wavesToSpawn.Count) {
                StartWaveNumber(currentWaveNumber + 1);
            }
            else {
                IsFinished = true;
            }
            
        }
        
        return props;
    }
}
