using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.Utilities.Editor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "LevelInformation", menuName = "Level/LevelInformation", order = 0)]
public class LevelDataHolder : ScriptableObject
{
    [SerializeField]
    [ListDrawerSettings(DefaultExpandedState = true, OnTitleBarGUI = "UpdateByIndex")]
    List<LevelData> levelInformations;

    public LevelData GetCurrentLevelInformation() {
        return levelInformations.Find(info => info.SceneIndex == SceneManager.GetActiveScene().buildIndex);
    }

    public LevelData GetLevelInformation(int index) {
        return levelInformations.Find(info => info.SceneIndex == index);
    }

    public LevelData GetLevelInformation(Scene scene) {
        return levelInformations.Find(info => info.Scene == scene);
    }

    public LevelData GetLevelInformation(string name) {
        return levelInformations.Find(info => info.SceneName == name);
    }

    private void UpdateByIndex()
    {
        if (SirenixEditorGUI.ToolbarButton(EditorIcons.Refresh))
        {
            if (levelInformations == null || levelInformations.Count < 1)
                return;

            foreach (LevelData levelInformation in levelInformations) {
                levelInformation.GetSceneNameByIndex();
            }
        }
    }
}