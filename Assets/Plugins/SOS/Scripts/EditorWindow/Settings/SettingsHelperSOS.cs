using UnityEditor;
using System;

namespace SOS {
    public static class SettingsHelperSOS
    {
        public static SOSSettings GetSettings() {
            return AssetDatabase.LoadAssetAtPath<SOSSettings>("Assets/Plugins/SOS/ScriptableObjects/Settings/SOSSettings_01.asset");
        }
    }
}
