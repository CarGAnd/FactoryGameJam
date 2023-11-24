using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace SOS {
    public class SOSWindow : OdinMenuEditorWindow
    {
        private SOSSettings settings;
        private SOSOverview overview;

        [MenuItem("SOS/Settings")]
        private static void OpenWindow()
        {
            GetWindow<SOSWindow>().Show();
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            settings = AssetDatabase.LoadAssetAtPath<SOSSettings>("Assets/SOS/ScriptableObjects/Settings/SOSSettings_01.asset");
            overview = AssetDatabase.LoadAssetAtPath<SOSOverview>("Assets/SOS/ScriptableObjects/Settings/SOSOverview.asset");

            OdinMenuTree tree = new OdinMenuTree(true, GetTreeConfig()) {
                { "SOS Settings",                             settings,                                                   EditorIcons.SettingsCog },
                { "Overview",                                 overview,                                                   EditorIcons.Checkmark },
            };

            return tree;
        }

        // Setup Odin Menu Tree with Search and caching expandend states
            private OdinMenuTreeDrawingConfig GetTreeConfig() {
                OdinMenuTreeDrawingConfig config = new OdinMenuTreeDrawingConfig
                {
                    DrawSearchToolbar = true,
                    DefaultMenuStyle = OdinMenuStyle.TreeViewStyle,
                    UseCachedExpandedStates = true
                };
                return config;
            }
    }
}