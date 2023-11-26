using System;
using TMPro;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

public class LeaderboardScoreInstanceVisual : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI nameText, scoreText, dateText, rankText, specialText;

    private LeaderboardScore leaderboardScore;

    public void InitializeScoreVisual(LeaderboardScore leaderboardScore) {
        this.leaderboardScore = leaderboardScore;
        nameText.text = leaderboardScore.GetEntryName();
        scoreText.text = leaderboardScore.GetEntryScore();
        dateText.text = leaderboardScore.GetEntryDate();
        rankText.text = leaderboardScore.GetEntryRank();

        SetSpecialText();
    }

    private void SetSpecialText()
    {
        if (!leaderboardScore.SpecialText)
            return;

        specialText.text = leaderboardScore.GetEntrySpecialText();
        specialText.gameObject.SetActive(true);
    }

    internal void Remove()
    {
        Destroy(gameObject);
    }
}