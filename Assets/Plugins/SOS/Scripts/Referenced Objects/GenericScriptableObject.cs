using System;
using UnityEngine;

namespace SOS {
    public abstract class GenericScriptableObject <T>: ScriptableObject
    {
        // Event for when value changed with new value and reference type.
        public Action<T, ReferenceType> OnValueChangedRef;
        public Action<T, T, ReferenceType> OnValueChangedRefFromTo;
        public Action<T> OnValueChanged;
        public Action<T, T> OnValueChangedFromTo;
        // Dynamic value of object, is always global. 
        private T dynamicValue;
        // Global constant value.
        private T globalConstantValue;
        // Global referenceType.
        public ReferenceType referenceType;

        public bool debugRef = false;

        public T DynamicValue { get => dynamicValue; set { DebugRefLog(dynamicValue, value); dynamicValue = value; } }
        public T GlobalConstantValue { get => globalConstantValue; set { DebugRefLog(globalConstantValue, value); globalConstantValue = value; } }

        internal virtual void DebugRefLog(T oldValue, T newValue) {
            if (!debugRef)
                return;

            Debug.Log($"{name} was changed from {oldValue} to {newValue}.");
        }
    }
}
