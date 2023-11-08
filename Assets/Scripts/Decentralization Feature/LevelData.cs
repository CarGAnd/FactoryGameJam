using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

[Serializable]
public struct LevelData {
    [SerializeField]
    [OnValueChanged("GetSceneNameByIndex")]
    private int sceneIndex;
    [SerializeField]
    private string sceneName;
    [SerializeField]
    private LevelProperties levelProperties;

    private Scene scene;

    public int SceneIndex { get => sceneIndex; private set => sceneIndex = value; }
    public string SceneName { get => sceneName; private set => sceneName = value; }
    public Scene Scene { get => scene; private set => scene = value; }
    public LevelProperties LevelProperties { get => levelProperties; private set => levelProperties = value; }

    public void GetSceneNameByIndex() {
        if (sceneIndex >= SceneManager.sceneCountInBuildSettings) {
            Debug.LogWarning($"There should be no buildindex of {SceneIndex}. Maximum should be {SceneManager.sceneCountInBuildSettings}.");
            return;
        }

        Scene = SceneManager.GetSceneByBuildIndex(SceneIndex);

        if (Scene == null) {
            Debug.LogError($"Scene at {SceneIndex} does not exist.");
            return;
        }

        SceneName = Scene.name;
    }
}
