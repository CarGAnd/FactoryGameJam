using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace SOS {
    public abstract class GenericReference<A, B> : SOSReference<A> where A : GenericScriptableObject<B>  {
        #region Internal Fields
        [ShowInInspector]
        [HorizontalGroup("row1", Width = 0.38f)]
        [PropertyOrder(100)]
        [DrawWithUnity]
        [HideLabel]
        [Required]
        internal override A variable {
            get {return scriptableObject;}
            set {
                scriptableObject = value;
                useLocalConstant = false;

                if (variable != null)
                    OnToggleType(variable.referenceType);
            }
        }
        #endregion
        #region Protected Fields

        // Local constant Value, only exists for this reference.
        [SerializeReference]
        [HideInInspector]
        protected B localConstantValue;

        [SerializeField]
        [HideInInspector]
        protected bool debugRef = false;

        #pragma warning disable 0414
        protected Color typeColor = new Color(0.4f, 0.8f, 1);
        protected string debugRefName = "Not Debugging";
        protected Color debugRefColor = new Color32(243, 109, 134, 255);
        #pragma warning restore 0414

        #endregion
        #region ReferenceType
        // Whether to use local, global or dynamic values, only for inspector.
        // Toggles colour change on get/set.
        [ShowInInspector]
        [HorizontalGroup("row1", Width = 0.54f)]
        [ShowIf("@variable && !useLocalConstant")]
        [PropertyOrder(50)]
        [EnumPaging]
        [GUIColor("$typeColor")]
        [OnInspectorInit("@OnToggleType(referenceType)")]
        [HideLabel]
        protected ReferenceType referenceType {
            get {
                if (variable == null || useLocalConstant) {
                    OnToggleType(ReferenceType.LocalConstant);
                    return ReferenceType.LocalConstant;
                }

                OnToggleType(variable.referenceType);
                return variable.referenceType;
            }
            set {
                if (variable != null) {
                    if (value == ReferenceType.LocalConstant)
                        useLocalConstant = true;
                    else
                        variable.referenceType = value;
                    OnToggleType(value);
                    SetDirty();
                }
            }
        }

        // Only shown when local value is chosen, only for inspector.
        // Toggles colour change on get/set.
        [ShowInInspector]
        [HorizontalGroup("row1", Width = 0.54f)]
        [ShowIf("@variable && useLocalConstant")]
        [PropertyOrder(50)]
        [EnumPaging]
        [GUIColor("$typeColor")]
        [OnInspectorInit("@OnToggleType(referenceType)")]
        [HideLabel]
        protected ReferenceType localReferenceType {
            get {
                return ReferenceType.LocalConstant;
            }
            set {
                if (variable!=null) {
                    if (value != ReferenceType.LocalConstant) {
                        useLocalConstant = false;
                        variable.referenceType = value;
                        OnToggleType(value);
                        SetDirty();
                    }
                }
            }
        }

        // Dynamic value, only for inspector.
        [ShowInInspector]
        [HorizontalGroup("row2", Width = 0.6f)]
        [ShowIf("@variable && referenceType == ReferenceType.Dynamic && !useLocalConstant")]
        [HideLabel]
        [OnInspectorInit("UpdateInvokingButton")]
        protected virtual B dynamicValue {
            get {
                if (variable != null && referenceType == ReferenceType.Dynamic) {
                    return variable.DynamicValue;
                }

                return localConstantValue;
            }
        }
        #endregion
        #region Values
        // Local or Global value of object, only for inspector.
        [ShowInInspector]
        [HorizontalGroup("row2", Width = 0.6f)]
        [ShowIf("@variable && (referenceType != ReferenceType.Dynamic || useLocalConstant)")]
        [HideLabel]
        [OnInspectorInit("UpdateInvokingButton")]
        protected virtual B value {
            get {
                if (variable != null) {
                    if (useLocalConstant)
                        return localConstantValue;
                    else if (referenceType == ReferenceType.Dynamic)
                        return variable.DynamicValue;
                    else if (referenceType == ReferenceType.GlobalConstant)
                        return variable.GlobalConstantValue;
                }

                return localConstantValue;
            }
            set {
                if (useLocalConstant) {
                    OnValueChangedRefFromTo?.Invoke(localConstantValue, value, ReferenceType.Dynamic);
                    OnValueChangedFromTo?.Invoke(localConstantValue, value);
                    localConstantValue = value;
                }
                else if (variable != null && referenceType == ReferenceType.GlobalConstant){
                    OnValueChangedRefFromTo?.Invoke(variable.GlobalConstantValue, value, ReferenceType.Dynamic);
                    OnValueChangedFromTo?.Invoke(variable.GlobalConstantValue, value);
                    variable.GlobalConstantValue = value;
                    SetDirty();
                }

                variable.OnValueChangedRef?.Invoke(value, referenceType);
                variable.OnValueChanged?.Invoke(value);
            }
        }

        // Dynamic Value of object, for set/get in scripts.
        public B Value {
            get { return value; }
            set {
                if (variable != null) {
                    DebugRef(variable.DynamicValue, value);
                    OnValueChangedRefFromTo?.Invoke(variable.DynamicValue, value, ReferenceType.Dynamic);
                    OnValueChangedFromTo?.Invoke(variable.DynamicValue, value);
                    variable.DynamicValue = value;

                    variable.OnValueChangedRef?.Invoke(value, ReferenceType.Dynamic);
                    variable.OnValueChanged?.Invoke(value);
                }
            }
        }

        // Dynamic Value of object, for set/get in scripts, with SetDirty which will save values after unity exit.
        public B ValueDirty {
            get { return value; }
            set {
                if (variable != null) {
                    DebugRef(variable.DynamicValue, value);
                    OnValueChangedRefFromTo?.Invoke(variable.DynamicValue, value, ReferenceType.Dynamic);
                    OnValueChangedFromTo?.Invoke(variable.DynamicValue, value);
                    variable.DynamicValue = value;
                    EditorUtility.SetDirty(variable);

                    variable.OnValueChangedRef?.Invoke(value, ReferenceType.Dynamic);
                    variable.OnValueChanged?.Invoke(value);
                }
            }
        }
        #endregion Values
        #region Events
        // Event to subscribe to, includes new value and referencetype, scripts only.
        public Action<B, ReferenceType> OnValueChangedRef {
            get {
                if (variable == null)
                    return null;

                return variable.OnValueChangedRef;
            }
            set {
                if (variable != null)
                    variable.OnValueChangedRef = value;
            }
        }

        // Event to subscribe to, includes old and new value, and referencetype, scripts only.
        public Action<B, B, ReferenceType> OnValueChangedRefFromTo {
            get {
                if (variable == null)
                    return null;

                return variable.OnValueChangedRefFromTo;
            }
            set {
                if (variable != null)
                    variable.OnValueChangedRefFromTo = value;
            }
        }

        // Event to subscribe to, includes new value, scripts only.
        public Action<B> OnValueChanged {
            get {
                if (variable == null)
                    return null;

                return variable.OnValueChanged;
            }
            set {
                if (variable != null)
                    variable.OnValueChanged = value;
            }
        }

        // Event to subscribe to, includes old and new value, scripts only.
        public Action<B, B> OnValueChangedFromTo {
            get {
                if (variable == null)
                    return null;

                return variable.OnValueChangedFromTo;
            }
            set {
                if (variable != null)
                    variable.OnValueChangedFromTo = value;
            }
        }
        #endregion Events
        #region Toggles
        [HorizontalGroup("row2", Width = 0.38f)]
        [LabelWidth(150)]
        [Button("@debugRefName"), GUIColor("$debugRefColor")]
        [ShowIf("@variable")]
        protected void ToggleDebugRef () {
            debugRef =! debugRef;
            UpdateInvokingButton();
        }

        private void UpdateInvokingButton() {
            ToggleColor(ref debugRefColor, debugRef);
            if (debugRef)
                debugRefName = "Debugging";
            else
                debugRefName = "Not Debugging";
        }

        private void ToggleColor(ref Color colorToToggle, bool state) {
            if (state)
                colorToToggle = new Color(0, 0.8f, 0);
            else
                colorToToggle = new Color32(243, 109, 134, 255);
        }

        // Toggle colors.
        protected virtual void OnToggleType(ReferenceType _referenceType) {
            if (useLocalConstant) {
                typeColor = new Color(0.4f, 0.8f, 1);
            } else if (_referenceType == ReferenceType.GlobalConstant) {
                typeColor = new Color32(243, 109, 134, 255);
            } else if (_referenceType == ReferenceType.Dynamic) {
                typeColor = new Color(0, 0.8f, 0);
            }
        }
        #endregion Toggles
        #region Creation
        protected override void CreateScriptableObject () {
            base.CreateScriptableObject();
            SetVariableStartMode(variable, SettingsHelperSOS.GetSettings().StartReferenceIn);
        }

        private void SetVariableStartMode(A _var, ReferenceType _referenceType) {
            if (_referenceType == ReferenceType.LocalConstant) {
                useLocalConstant = true;
            } else {
                _var.referenceType = _referenceType;
            }
        }
        #endregion
        #region Debug
        protected virtual void DebugRef(B oldValue, B newValue) {
            if (!debugRef)
                return;

            Debug.Log($"{variable.name} was changed from {oldValue} to {newValue}.");
        }
        #endregion
    }
}