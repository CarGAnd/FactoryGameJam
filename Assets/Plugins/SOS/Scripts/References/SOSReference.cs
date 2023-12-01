using System;
using System.Drawing;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace SOS {
    [InlineProperty]
    [LabelWidth(150)]
    [Serializable]
    public abstract class SOSReference<A> where A : ScriptableObject
    {
        // Name of scriptable object.
        [SerializeField]
        [LabelWidth(40)]
        [HorizontalGroup("row2", Order = 0)]
        [HideIf("variable")]
        [Tooltip("Please provide a unique name.")]
        protected string name;

        // Small denotion of type, only for inspector. 
        [ShowInInspector]
        [HorizontalGroup("row1", Width = 0.06f)]
        [PropertyOrder(-1)]
        [GUIColor("$denotionColor")]
        [HideLabel]
        [DisplayAsString]
        [ReadOnly]
        protected abstract string denotation { get; }

        [SerializeField]
        [HideInInspector]
        protected A scriptableObject;

        // Scriptable Object of specific type.
        [ShowInInspector]
        [HorizontalGroup("row1", Width = 0.14f, Order = 1)]
        [PropertyOrder(100)]
        [DrawWithUnity]
        [HideLabel]
        [Required]
        internal virtual A variable {
            get {return scriptableObject;}
            set {scriptableObject = value;}
        }

        // When true, the reference only returns the local constant, whereas all other references of the same object are unchanged.
        [SerializeField]
        [HideInInspector]
        protected bool useLocalConstant = false;
        protected string sOSaveLocation = "Assets/ScriptableObjects/";
        protected abstract Color32 denotionColor { get; }

        // Creation of Object.
        [HorizontalGroup("row1", Width = 0.54f)]
        [LabelWidth(150)]
        [Button("Create", 50), GUIColor(0, 0.9f, 0)]
        [DisableIf("@string.IsNullOrEmpty(name)")]
        [HideIf("@variable")]
        protected virtual void CreateScriptableObject () {
            variable = ScriptableObject.CreateInstance<A>();
            AssetDatabase.CreateAsset(variable, Path.Combine(SettingsHelperSOS.GetSettings().SosDynamicParentPath, sOSaveLocation, name + ".asset"));
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        // Sets the variable dirty, saving the data between engine restarts, only during editing.
        protected virtual void SetDirty () {
            if (!EditorApplication.isPlaying && SettingsHelperSOS.GetSettings().SetDirtyInEditor) {
                EditorUtility.SetDirty(variable);
            }
        }
    }
}
