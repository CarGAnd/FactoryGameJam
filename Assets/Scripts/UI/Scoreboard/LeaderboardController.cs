using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using SOS;
using Unity.Services.Leaderboards.Models;
using System.Linq;

public class LeaderboardController : MonoBehaviour
{
    [SerializeField] private GameObject scoreInstanceVisualPrefab;
    [SerializeField] private Transform scoreInstanceParent;
    [SerializeField] private GameObject UIParent;
    [SerializeField] private LevelStateRef levelState;
    [SerializeField] private IntRef score;
    [SerializeField] private int scoreRangeLimit = 5;

    private double oldScore;
    private List<LeaderboardScore> scoreInstances;
    private List<LeaderboardScoreInstanceVisual> instantiatedVisuals;

    async void Awake()
    {
        await UnityServices.InitializeAsync();
        await OnlineUtilities.SignInAnonymously();
    }

    private void OnEnable() {
        SubscribeToLevelState();
    }

    private void OnDisable() {
        UnsubscribeFromLevelState();
    }

    private void SubscribeToLevelState()
    {
        levelState.OnValueChanged += InitializeLeaderboard;
    }

    private void UnsubscribeFromLevelState()
    {
        levelState.OnValueChanged -= InitializeLeaderboard;
    }

    private void InitializeLeaderboard(LevelState state)
    {
        if (state != LevelState.Leaderboard)
            return;

        AddScoreEntry();
        VisualizeScores();
        ToggleVisuals(true);
    }

    private void ToggleVisuals(bool toggle) {
        UIParent.SetActive(toggle);
    }

    private async void VisualizeScores() {
        FillSortedEnties(await GetRangedScores());
        UpdateScoreVisuals(scoreInstances);
    }

    private void AddScoreEntry()
    {
        SetOldScore();
        AddScore(score.Value);
    }

    private async void SetOldScore() {
        oldScore = await GetPlayerScore();
    }

    public async Task<Double> GetPlayerScore()
    {
        var scoresResponse =
            await LeaderboardsService.Instance.GetScoresAsync(LevelDataGetter.GetCurrent().LeaderboardID);

        if (scoresResponse.Results == null || scoresResponse.Results.Count <= 0)
            return double.MinValue;

        return scoresResponse.Results[0].Score;
    }

    // TODO: ADD DateTime in PlayerScoreOptions.
    public async void AddScore(double scoreToAdd)
    {
        await LeaderboardsService.Instance.AddPlayerScoreAsync(LevelDataGetter.GetCurrent().LeaderboardID, scoreToAdd, new AddPlayerScoreOptions());
    }

    public async Task<List<LeaderboardEntry>> GetRangedScores()
    {
        var scoresResponse =
            await LeaderboardsService.Instance.GetPlayerRangeAsync(LevelDataGetter.GetCurrent().LeaderboardID, new GetPlayerRangeOptions{RangeLimit = scoreRangeLimit});
        
        return scoresResponse.Results;
    }

    private void FillSortedEnties(List<LeaderboardEntry> entries) {
        ConstructSortedScoreList();
        foreach(LeaderboardEntry leaderboardEntry in entries) {
            scoreInstances.Add(new LeaderboardScore(leaderboardEntry, IsCurrentPlayer(leaderboardEntry.PlayerId), IsHighscore(leaderboardEntry.Score)));
        }
    }

    public void UpdateScoreVisuals(List<LeaderboardScore> _scoreInstances) {
        ConstructInstantiatedList();
        CleanInstantiatedVisuals();
        SortScoreList();
        foreach (LeaderboardScore entry in _scoreInstances) {
            LeaderboardScoreInstanceVisual visual = Instantiate(scoreInstanceVisualPrefab, scoreInstanceParent).GetComponent<LeaderboardScoreInstanceVisual>();
            visual.InitializeScoreVisual(entry);
            instantiatedVisuals.Add(visual);
        }
    }

    private void SortScoreList()
    {
        scoreInstances.OrderBy(entry => entry.Score);
    }

    private bool IsHighscore(double score)
    {
        return score < oldScore;
    }

    private bool IsCurrentPlayer(string playerId)
    {
        return playerId == AuthenticationService.Instance.PlayerId;
    }

    private void ConstructInstantiatedList() {
        if (instantiatedVisuals != null)
            return;

        instantiatedVisuals = new List<LeaderboardScoreInstanceVisual>();
    }

    private void ConstructSortedScoreList() {
        scoreInstances = new List<LeaderboardScore>();
    }

    private void CleanInstantiatedVisuals() {
        foreach (LeaderboardScoreInstanceVisual visual in instantiatedVisuals) {
            visual.Remove();
        }
    }
}