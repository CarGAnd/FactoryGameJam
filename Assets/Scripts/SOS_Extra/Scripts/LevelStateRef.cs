using System;
using UnityEngine;
namespace SOS {
    [Serializable]
    public class LevelStateRef : GenericReference<ScriptableLevelState, LevelState> {
        protected override string denotation {
            get { return "LS"; }
        }

        protected override Color32 denotionColor {
            get { return SettingsHelperSOS.GetSettings().IntDenotionColor; }
        }

        protected override void CreateScriptableObject () {
            sOSaveLocation = SettingsHelperSOS.GetSettings().IntRefPath;
            base.CreateScriptableObject();
        }
    }
}