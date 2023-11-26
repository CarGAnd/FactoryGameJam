using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOS;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    [SerializeField] private FactoryTracker factoryTracker;
    [SerializeField] private GameEvent buildPhaseStartedEvent;
    [SerializeField] private GameEvent runPhaseStartedEvent;
    [SerializeField] private GameEvent runPhaseEndedEvent;
    [SerializeField] private LevelStateRef currentLevelStateRef;
    public LevelStateRef CurrentLevelStateRef { get => currentLevelStateRef; private set => currentLevelStateRef = value; }

    private void Awake(){
        if(Instance == null){
            Instance = this;
        }
        else{
            Destroy(gameObject);
        }
    }

    private void Start() {
        CurrentLevelStateRef.Value = LevelState.LevelLoaded;
    }

    public void GoToBuildPhase() {
        CurrentLevelStateRef.Value = LevelState.BuildPhase;
        buildPhaseStartedEvent.Invoke();
        Debug.Log("Build phase");
    }

    public void GoToRunPhase() {
        if(IsReadyToRun()) {
            CurrentLevelStateRef.Value = LevelState.RunPhase;
            runPhaseStartedEvent.Invoke();
            ObjectTracker objectTracker = new ObjectTracker();
            objectTracker.StartTracking(factoryTracker, this);
            Debug.Log("Run phase");
        }  
    }

    private bool IsReadyToRun() {
        if (!ASpawnerIsConnected()) {
            Debug.Log("Cannot start until at least one spawner has its output connected");
        }

        return ASpawnerIsConnected() && CurrentLevelStateRef.Value == LevelState.BuildPhase;
    }

    private bool ASpawnerIsConnected() {
        int totalOutputs = 0;
        List<SpawnerModule> spawners = factoryTracker.Spawners;
        foreach (SpawnerModule sm in spawners) {
            totalOutputs += sm.GetNumberOfConnectedOutputs();
        }
        return totalOutputs > 0;
    }

    public void GoToLevelCompletedPhase() {
        if (CurrentLevelStateRef.Value == LevelState.RunPhase) {
            CurrentLevelStateRef.Value = LevelState.LevelOver;
            runPhaseEndedEvent.Invoke();
            Debug.Log("Level completed phase");
        }
    }
}

public enum LevelState {
    LevelLoaded = 0,
    BuildPhase = 10,
    RunPhase = 20,
    LevelOver = 30,
    Leaderboard = 40
}
