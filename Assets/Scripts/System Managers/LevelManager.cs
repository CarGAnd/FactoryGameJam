using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOS;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private FactoryTracker factoryTracker;
    [SerializeField] private GameEvent buildPhaseStartedEvent;
    [SerializeField] private GameEvent runPhaseStartedEvent;
    [SerializeField] private GameEvent runPhaseEndedEvent;

    private LevelState currentLevelState;

    private void Start() {
        currentLevelState = LevelState.LevelLoaded;
        StartCoroutine(StartBuildPhase());
    }

    private IEnumerator StartBuildPhase() {
        yield return new WaitForSeconds(1f);
        GoToBuildPhase();
    }

    public void GoToBuildPhase() {
        currentLevelState = LevelState.BuildPhase;
        buildPhaseStartedEvent.Invoke();
        Debug.Log("Build phase");
    }

    public void GoToRunPhase() {
        if(currentLevelState == LevelState.BuildPhase) {
            currentLevelState = LevelState.RunPhase;
            runPhaseStartedEvent.Invoke();
            ObjectTracker objectTracker = new ObjectTracker();
            objectTracker.StartTracking(factoryTracker, this);
            Debug.Log("Run phase");
        }  
    }

    public void GoToLevelCompletedPhase() {
        if (currentLevelState == LevelState.RunPhase) {
            currentLevelState = LevelState.LevelOver;
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
