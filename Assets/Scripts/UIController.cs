using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOS;
using UnityEngine.SceneManagement;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameEvent startSpawnPhaseEvent;
    [SerializeField] private GameEvent winGameEvent;
    [SerializeField] private GameEvent loseGameEvent;
    [SerializeField] private IntRef levelScore;

    [SerializeField] private TextMeshProUGUI scoreText;

    [SerializeField] private GameObject winUI;
    [SerializeField] private GameObject loseUI;
    [SerializeField] private GameObject gameplayUI;

    private void Awake() {
        //TODO: Move this somewhere else
        levelScore.Value = 0;
    }

    private void OnEnable() {
        winGameEvent.EventInvoked += ShowWinUI;
        loseGameEvent.EventInvoked += ShowLoseUI;
        levelScore.ValueChanged += UpdateScore;
    }

    private void OnDisable() {
        winGameEvent.EventInvoked -= ShowWinUI;
        loseGameEvent.EventInvoked -= ShowLoseUI;
        levelScore.ValueChanged -= UpdateScore;
    }

    private void UpdateScore(int newScore, ReferenceType refType) {
        scoreText.text = "Score: " + newScore;
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

    public void StartSpawnPhase() {
        startSpawnPhaseEvent.Invoke();
    }

    public void RestartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
