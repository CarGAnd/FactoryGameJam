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
        StartCoroutine(StartBuildPhase());
    }

    private IEnumerator StartBuildPhase() {
        yield return new WaitForSeconds(1f);
        GoToBuildPhase();
    }

    public void GoToBuildPhase() {
        CurrentLevelStateRef.Value = LevelState.BuildPhase;
        buildPhaseStartedEvent.Invoke();
        Debug.Log("Build phase");
    }

    public void GoToRunPhase() {
        if(CurrentLevelStateRef.Value == LevelState.BuildPhase) {
            CurrentLevelStateRef.Value = LevelState.RunPhase;
            runPhaseStartedEvent.Invoke();
            ObjectTracker objectTracker = new ObjectTracker();
            objectTracker.StartTracking(factoryTracker, this);
            Debug.Log("Run phase");
        }  
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
    LevelLoaded,
    BuildPhase,
    RunPhase,
    LevelOver
}
