using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOS;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameEvent winGameEvent;
    [SerializeField] private GameEvent loseGameEvent;

    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject loseUI;
    [SerializeField] private GameObject gameplayUI;

    private void OnEnable() {
        winGameEvent.EventInvoked += ShowWinUI;
        loseGameEvent.EventInvoked += ShowLoseUI;
    }

    private void OnDisable() {
        winGameEvent.EventInvoked -= ShowWinUI;
        loseGameEvent.EventInvoked -= ShowLoseUI;
    }

    private void ShowLoseUI() {
        loseUI.SetActive(true);
        winUI.SetActive(false);
        gameplayUI.SetActive(false);
    }

    private void ShowWinUI() {
        winUI.SetActive(true);
        loseUI.SetActive(false); 
        gameplayUI.SetActive(false);
    }
}
