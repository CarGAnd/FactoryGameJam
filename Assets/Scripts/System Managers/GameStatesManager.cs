using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public static class GameStatesManager
{
    public static UnityEvent<GameplayPhases> GameplayPhaseChanged = new();
    public static UnityEvent<GameStates> GameStateChanged = new();
    public static UnityEvent<MenuStates> MenuStateChanged = new();
    static GameplayPhases currentGameplayPhase = GameplayPhases.BuildPhase;
    static GameStates currentGameState = GameStates.GameStarted;
    static MenuStates currentMenuState = MenuStates.MainMenu;

    public static void SetCurrentGameplayPhase(GameplayPhases newGameplayPhase)
    {
        currentGameplayPhase = newGameplayPhase;
        GameplayPhaseChanged?.Invoke(currentGameplayPhase);
    }

    public static GameplayPhases GetCurrentGameplayPhase()
    {
        return currentGameplayPhase;
    }

    public static void SetCurrentGameState(GameStates newGameState)
    {
        currentGameState = newGameState;
        GameStateChanged?.Invoke(currentGameState);
    }

    public static GameStates GetCurrentGameState()
    {
        return currentGameState;
    }

    public static void SetCurrentMenuState(MenuStates newMenuState)
    {
        currentMenuState = newMenuState;
        MenuStateChanged?.Invoke(currentMenuState);
    }

    public static MenuStates GetCurrentMenuState()
    {
        return currentMenuState;
    }


}

public enum GameplayPhases
{
    BuildPhase = 10,
    RunPhase = 20,
}

public enum GameStates
{
    GameStarted = 10,
    LevelLoaded = 20,
    GameRunning = 30,
    GamePaused = 40,
    LevelFinished = 50,
}

public enum MenuStates
{
    PauseMenu = 10,
    MainMenu = 20,
    LeaderboardMenu = 30,
    SettingsMenu = 40,
    LevelSelectMenu = 50,
}
