using System;
using UnityEngine;
namespace SOS {
    [Serializable]
    public class ModuleTypeRef : GenericReference<ScriptableModuleType, ModuleType> {
        protected override string denotation {
            get { return "MT"; }
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