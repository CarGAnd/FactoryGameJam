using SOS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WinLoseDecider : MonoBehaviour
{
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
    }

}
