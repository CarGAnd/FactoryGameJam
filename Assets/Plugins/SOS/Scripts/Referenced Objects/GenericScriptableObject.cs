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
        public T DynamicValue;

        // Global constant value.
        public T GlobalConstantValue;

        // Global referenceType.
        public ReferenceType referenceType;
    }
}
