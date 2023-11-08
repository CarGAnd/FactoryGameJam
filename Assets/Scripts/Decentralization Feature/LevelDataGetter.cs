using UnityEditor;
using UnityEngine.SceneManagement;

public static class LevelDataGetter
{
    private static readonly string dataPath = "Assets/Scriptable Objects/LevelDataHolder/LevelDataHolder.asset";
    
    public static LevelData GetCurrent() {
        return AssetDatabase.LoadAssetAtPath<LevelDataHolder>(dataPath).GetCurrentLevelInformation();
    }

    public static LevelData Get(string name) {
        return AssetDatabase.LoadAssetAtPath<LevelDataHolder>(dataPath).GetLevelInformation(name);
    }

    public static LevelData Get(int index) {
        return AssetDatabase.LoadAssetAtPath<LevelDataHolder>(dataPath).GetLevelInformation(index);
    }

    public static LevelData Get(Scene scene) {
        return AssetDatabase.LoadAssetAtPath<LevelDataHolder>(dataPath).GetLevelInformation(scene);
    }
}
