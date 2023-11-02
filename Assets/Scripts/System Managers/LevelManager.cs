using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SOS;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] private GameEvent spawnPhaseStartEvent;

    public void StartSpawnPhase() {
        spawnPhaseStartEvent.Invoke();
        Debug.Log("Started spawning phase");
    }

    public void RestartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
