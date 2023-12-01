using SOS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLoseDecider : MonoBehaviour
{
    [SerializeField] private FactoryTracker factoryTracker;
    [SerializeField] private GameEvent runPhaseEndedEvent;
    [SerializeField] private GameEvent winGameEvent;
    [SerializeField] private GameEvent loseGameEvent;

    private void OnEnable() {
        runPhaseEndedEvent.OnInvoked += CheckWinLose;
    }

    private void OnDisable() {
        runPhaseEndedEvent.OnInvoked -= CheckWinLose;
    }

    private void CheckWinLose() {
        int totalSpawns = factoryTracker.GetNumObjectsInLevel();
        int correctObjectCollected = factoryTracker.GetNumCorrectItemsCollected();

        if(correctObjectCollected >= totalSpawns) {
            winGameEvent.Invoke();
        }
        else {
            loseGameEvent.Invoke();
        }
    }

}
