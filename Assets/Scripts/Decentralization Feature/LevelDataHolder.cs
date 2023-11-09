using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "LevelInformation", menuName = "Level/LevelInformation", order = 0)]
public class LevelDataHolder : ScriptableObject
{
    [InfoBox("It is only possible to set scene by index / name, while in that specific scene is loaded, and only if the scene has been added in Build Settings.")]
    [SerializeField]
    [ListDrawerSettings(DefaultExpandedState = true, OnTitleBarGUI = "UpdateByIndex")]
    List<LevelData> levelData;

    public LevelData GetCurrentLevelInformation() {
        LevelData? data = levelData.Find(info => info.SceneIndex == SceneManager.GetActiveScene().buildIndex);

        return CheckValid(data);
    }

    public LevelData GetLevelInformation(int index) {
        LevelData? data = levelData.Find(info => info.SceneIndex == index);

        return CheckValid(data);
    }

    public LevelData GetLevelInformation(Scene scene) {
        LevelData? data = levelData.Find(info => info.Scene == scene);

        return CheckValid(data);
    }

    public LevelData GetLevelInformation(string name) {
        LevelData? data = levelData.Find(info => info.SceneName == name);

        return CheckValid(data);
    }

    private LevelData CheckValid(LevelData? data) {
        if (data == null) {
            IfEmptyDebug(SceneManager.GetActiveScene().buildIndex);
            return default;
        }

        return data.Value;
    }

    private void IfEmptyDebug<T>(T type) {
        if (type is string)
            Debug.LogWarning($"A scene by the name of {type} was not found. Check Build Settings if the scene has been added, and LevelDataHolder if the name is correct.");
        else if (type is int)
            Debug.LogWarning($"A scene with a buildindex of {type} was not found. Check Build Settings if the scene has been added.");
        else if (type is Scene) {
            Scene? scene = type as Scene?;
            Debug.LogWarning($"The scene with name of {scene.Value.name} and buildindex of {scene.Value.buildIndex} was not found. Check Build Settings if the scene has been added, and LevelDataHolder if the name is correct.");
        }
            
    }

    private void UpdateByIndex()
    {
        if (SirenixEditorGUI.ToolbarButton(EditorIcons.Refresh))
        {
            if (levelData == null || levelData.Count < 1)
                return;

            foreach (LevelData levelInformation in levelData) {
                levelInformation.GetSceneNameByIndex();
            }
        }
    }
}