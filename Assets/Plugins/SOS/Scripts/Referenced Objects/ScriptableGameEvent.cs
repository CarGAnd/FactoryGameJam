using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace SOS {
    [CreateAssetMenu (fileName = "GameEvent", menuName = "SOS/Game Event")]
    public class ScriptableGameEvent : ScriptableObject
    {
        public Action OnInvoked;

        [Button(ButtonSizes.Gigantic), GUIColor(0, 1, 0.5058824f)]
        private void Invoke() {
            OnInvoked?.Invoke();
        }
    }
}