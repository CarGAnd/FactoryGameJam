using System;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace SOS {
    public abstract class GenericReference<A, B> : SOSReference<A> where A : GenericScriptableObject<B>  {
        [ShowInInspector]
        [HorizontalGroup("row", Width = 0.14f)]
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

        // Local constant Value, only exists for this reference.
        [SerializeReference]
        [HideInInspector]
        protected B localConstantValue;

        // Whether to use local, global or dynamic values, only for inspector.
        // Toggles colour change on get/set.
        [ShowInInspector]
        [HorizontalGroup("row", Width = 0.35f)]
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
        [HorizontalGroup("row", Width = 0.35f)]
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
        [HorizontalGroup("row", Width = 0.45f)]
        [ShowIf("@variable && referenceType == ReferenceType.Dynamic && !useLocalConstant")]
        [HideLabel]
        protected virtual B dynamicValue {
            get {
                if (variable != null && referenceType == ReferenceType.Dynamic) {
                    return variable.DynamicValue;
                }

                return localConstantValue;
            }
        }

        // Local or Global value of object, only for inspector.
        [ShowInInspector]
        [HorizontalGroup("row", Width = 0.45f)]
        [ShowIf("@variable && (referenceType != ReferenceType.Dynamic || useLocalConstant)")]
        [HideLabel]
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
                    OnValueChangedRefFromTo?.Invoke(variable.DynamicValue, value, ReferenceType.Dynamic);
                    OnValueChangedFromTo?.Invoke(variable.DynamicValue, value);
                    variable.DynamicValue = value;
                    EditorUtility.SetDirty(variable);

                    variable.OnValueChangedRef?.Invoke(value, ReferenceType.Dynamic);
                    variable.OnValueChanged?.Invoke(value);
                }
            }
        }

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

        // Fields used for odin inspector visuals
        #region Private Fields

        #pragma warning disable 0414
        protected Color typeColor = new Color(0.4f, 0.8f, 1);
        #pragma warning restore 0414

        #endregion
    }
}