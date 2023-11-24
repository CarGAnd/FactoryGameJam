using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SOS {
    [Serializable]
    public class GameEvent : SOSReference<ScriptableGameEvent>
    {
        [SerializeField]
        [HideInInspector]
        private bool allowInvoking = true;
        [SerializeField]
        [HideInInspector]
        private bool subscribeOnPlay = true;

        // Scriptable Object of ScriptableGameEvent type.
        internal override ScriptableGameEvent variable {
            get { return scriptableObject; }
            set { scriptableObject = value; }
        }

        private void OnInspectorInit() {
            UpdateInvokingButton();
            UpdateListeningButton();
        }

        // Toggle between allowing and disallowing invocation of the ScriptableGameEvent Action from this reference.
        [Button("$toggleNameInvoking"), GUIColor("$toggleColorInvoking")]
        [HorizontalGroup("row", Width = 0.266f)]
        [ShowIf("@variable")]
        [Tooltip("Toggle between allowing event invocations from this reference.")]
        private void ToggleAllowInvoking () {
            allowInvoking = !allowInvoking;
            UpdateInvokingButton();
        }

        // Toggle between allowing and disallowing listening to the ScriptableGameEvent Action from this reference.
        [Button("$toggleNameListening")]
        [GUIColor("$toggleColorListening")]
        [HorizontalGroup("row", Width = 0.266f)]
        [Tooltip("Toggle between allowing this reference to listen to its event invocations.")]
        [ShowIf("@variable")]
        private void ToggleAllowListening () {
            subscribeOnPlay = !subscribeOnPlay;
            UpdateListeningButton();
        }

        // Button and method for invoking the ScriptableGameEvent Action.
        [Button, GUIColor(0, 1, 0.5058824f)]
        [HorizontalGroup("row", Width = 0.266f)]
        [ShowIf("@variable")]
        [Tooltip("Invoke the current ScriptableGameEvent.")]
        public void Invoke() {
            if (variable != null && allowInvoking)
                variable.OnInvoked?.Invoke();
        }

        // Updates text and colors of the allow Invocation toggle button.
        private void UpdateInvokingButton() {
            ToggleColor(ref toggleColorInvoking, allowInvoking);
            if (allowInvoking)
                toggleNameInvoking = "Invoking"; 
            else
                toggleNameInvoking = "Not Invoking";
        }

        // Updates text and colors of the allow listening toggle button.
        private void UpdateListeningButton() {
            ToggleColor(ref toggleColorListening, subscribeOnPlay);
            if (subscribeOnPlay)
                toggleNameListening = "Subscribe on Play";
            else
                toggleNameListening = "Do not Subscribe";
        }

        private void ToggleColor(ref Color colorToToggle, bool state) {
            if (state)
                colorToToggle = new Color(0, 0.8f, 0);
            else
                colorToToggle = new Color32(243, 109, 134, 255);
        }

        // Event to subscribe to, scripts only.
        [HideInInspector]
        public Action OnInvoked {
            get {
                if (variable != null && subscribeOnPlay)
                    return variable.OnInvoked;

                return null;
            }
            set {
                if (variable != null && subscribeOnPlay)
                    variable.OnInvoked = value;
            }
        }

        // Small denotion of type, only for inspector.
        [ShowInInspector]
        [HorizontalGroup("row", Width = 0.06f)]
        [PropertyOrder(-1)]
        [GUIColor("$denotionColor")]
        [HideLabel]
        [DisplayAsString]
        [ReadOnly]
        [OnInspectorInit("OnInspectorInit")]
        protected override string denotation {
            get {
                return "GE";
            }
        }

        protected override Color32 denotionColor {
            get {
                return SettingsHelperSOS.GetSettings().GameEventDenotionColor;
            }
        }

        // Creation of Object.
        protected override void CreateScriptableObject () {
            sOSaveLocation = SettingsHelperSOS.GetSettings().GameEventRefPath;
            base.CreateScriptableObject();
        }

        // Fields used for odin inspector visuals
        #region Private Fields

        #pragma warning disable 0414
        private Color toggleColorInvoking = new Color(0, 0.8f, 0);
        private Color toggleColorListening = new Color(0, 0.8f, 0);
        private string toggleNameListening = "Subscribe on Play";
        private string toggleNameInvoking = "Invoking";

        #pragma warning restore 0414

        #endregion

    }
}