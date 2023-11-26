using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using SOS;
using Unity.Services.Authentication;
using Sirenix.OdinInspector;

public class StartScreenController : MonoBehaviour
{
    [SerializeField] private GameObject UIParent;
    [SerializeField] private Button startGameButton, QuitGameButton;
    [SerializeField] private TMP_InputField nameInputField;
    [SerializeField] private LevelStateRef currentLevelState;
    [SerializeField] private GameEvent buildPhaseStartedEvent;

    private string tempPlayerName;

    private void Start () {
        DisableStartButtonOnNoName();
    }

    private void OnEnable() {
        startGameButton.onClick.AddListener(StartGame);
        QuitGameButton.onClick.AddListener(QuitGame);
        nameInputField.onValueChanged.AddListener(UpdateTempPlayerName);
        currentLevelState.OnValueChangedFromTo += DisableOnGameStart;
    }

    private void OnDisable() {
        startGameButton.onClick.RemoveListener(StartGame);
        QuitGameButton.onClick.RemoveListener(QuitGame);
        nameInputField.onValueChanged.RemoveListener(UpdateTempPlayerName);
        currentLevelState.OnValueChangedFromTo -= DisableOnGameStart;
    }

    private void DisableOnGameStart(LevelState state1, LevelState state2)
    {
        if (state1 == LevelState.LevelLoaded && state2 == LevelState.BuildPhase)
            ToggleVisuals(false);

        else if (state2 == LevelState.LevelLoaded)
            ToggleVisuals(true);
    }

    private void ToggleVisuals(bool toggle)
    {
        UIParent.SetActive(toggle);
    }

    private void UpdateTempPlayerName(string playerName)
    {
        tempPlayerName = playerName;
        DisableStartButtonOnNoName();
    }

    private async void UpdatePlayerNameAsync() {
        if (!string.IsNullOrEmpty(tempPlayerName))
            await OnlineUtilities.SetPlayerName(tempPlayerName);
    }

    private void QuitGame()
    {
        Application.Quit();
    }

    private void StartGame()
    {
        UpdatePlayerNameAsync();
        currentLevelState.Value = LevelState.BuildPhase;
        buildPhaseStartedEvent.Invoke();
    }

    private bool CheckPlayerName() {
        if (!string.IsNullOrEmpty(AuthenticationService.Instance.PlayerName))
            return true;
        
        if (!string.IsNullOrEmpty(tempPlayerName))
            return true;

        return false;
    }

    private void DisableStartButtonOnNoName() {
        if (!CheckPlayerName()) {
            startGameButton.interactable = false;
        } else {
            startGameButton.interactable = true;
        }
    }
}
