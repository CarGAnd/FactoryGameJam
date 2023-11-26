using System;
using Unity.Services.Leaderboards.Models;

public struct LeaderboardScore {
    string name;
    double score;
    DateTime dateTime;
    int rank;
    bool highScore;
    bool playerScore;
    bool specialText;

    public LeaderboardScore (LeaderboardEntry entry, bool playerScore, bool highScore) {
        name = entry.PlayerName;
        score = entry.Score;
        dateTime = entry.UpdatedTime;
        rank = entry.Rank;
        this.playerScore = playerScore;
        this.highScore = highScore;
        specialText = playerScore|highScore;
        CleanName();
    }

    public bool HighScore { get => highScore; private set => highScore = value; }
    public bool PlayerScore { get => playerScore; private set => playerScore = value; }
    public bool SpecialText { get => specialText; private set => specialText = value; }

    private void CleanName() {
        int index = name.LastIndexOf('#');
        name = name[..index];
    }

    public string GetEntryName() {
        return name;
    }

    public string GetEntryScore() {
        return score.ToString();
    }

    public string GetEntryDate() {
        return dateTime.ToShortDateString();
    }

    public string GetEntrySpecialText() {
        if (highScore)
            return "New HighScore!";
        if (playerScore)
            return "Your Score";

        return string.Empty;
    }

    internal string GetEntryRank()
    {
        return $"{rank}#";
    }
}