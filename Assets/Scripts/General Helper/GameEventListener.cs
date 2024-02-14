using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using SOS;

public class GameEventListener : MonoBehaviour
{
    public GameEvent gameEvent;
    public UnityEvent onEventTriggered;

    private void OnEnable() {
        gameEvent.OnInvoked += TriggerEvent;    
    }

    private void OnDisable() {
        gameEvent.OnInvoked -= TriggerEvent;
    }

    private void TriggerEvent() {
        onEventTriggered.Invoke();
    }
}
