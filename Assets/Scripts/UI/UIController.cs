using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOS;
using UnityEngine.SceneManagement;
using TMPro;

public class UIController : MonoBehaviour
{
    [SerializeField] private GameEvent OnUIButtonPressed;
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
        winGameEvent.OnInvoked += ShowWinUI;
        loseGameEvent.OnInvoked += ShowLoseUI;
        levelScore.OnValueChanged += UpdateScore;
    }

    private void OnDisable() {
        winGameEvent.OnInvoked -= ShowWinUI;
        loseGameEvent.OnInvoked -= ShowLoseUI;
        levelScore.OnValueChanged -= UpdateScore;
    }

    private void UpdateScore(int newScore) {
        scoreText.text = "Score: " + newScore;
    }

    private void ShowGameUI() {
        loseUI.SetActive(false);
        winUI.SetActive(false);
        gameplayUI.SetActive(true);
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
        if(LevelManager.Instance != null) {
            LevelManager.Instance.GoToRunPhase();
        }
        else {
            Debug.LogError("UI requires a LevelManager in the scene to change state");
        } 
    }

    public void GoToBuildPhase() {
        if (LevelManager.Instance != null) {
            LevelManager.Instance.GoToBuildPhase();
            ShowGameUI();
            levelScore.Value = 0;
        }
        else {
            Debug.LogError("UI requires a LevelManager in the scene to change state");
        }
    }

    public void RestartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void UIButtonPressed () {
        OnUIButtonPressed.Invoke();
    }
}
