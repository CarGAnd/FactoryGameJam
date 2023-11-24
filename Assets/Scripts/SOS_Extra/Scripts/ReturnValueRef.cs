using System;
using UnityEngine;
namespace SOS {
    [Serializable]
    public class ReturnValueRef : GenericReference<ScriptableReturnValue, ReturnValue> {
        protected override string denotation {
            get { return "RV"; }
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