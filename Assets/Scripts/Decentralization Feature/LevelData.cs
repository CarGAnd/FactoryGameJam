using System;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using Unity.VisualScripting;
using UnityEditor;
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
    private string savedSceneName;
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
        savedSceneName = SceneName;
    }

    [ShowIf("@sceneName != savedSceneName")]
    [Button]
    public void TryGetSceneByName() {
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        bool sceneFound = false;

        for (int i = 0; i < sceneCount; i++) {
            Scene sceneAtIndex = SceneManager.GetSceneByBuildIndex(i);
            if (sceneAtIndex.name == SceneName) {
                savedSceneName = SceneName;
                Scene = sceneAtIndex;
                SceneIndex = i;
                sceneFound = true;
                break;
            }
        }

        if (!sceneFound) {
            Debug.LogWarning($"No scene by the name of {sceneName} can be found. Make sure it is added in Build Settings, or alternatively provide its build index.");
        } 
    }
}
