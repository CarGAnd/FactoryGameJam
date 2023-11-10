using Sirenix.OdinInspector;
using System;
using UnityEngine;


[InlineEditor]
[Serializable]
public class GlobalParamContainer
{
    public Action<string, float> ValueChanged;

    [Required]
    [SerializeField]
    [FMODUnity.ParamRef]
    public string GlobalParameterName;

    [SerializeField]
    [HideInInspector]
    private float globalParameterValue;

    public GlobalParamContainer(string _paramName, float _paramValue)
    {
        GlobalParameterName = _paramName;
        globalParameterValue = _paramValue;
    }

    [ShowInInspector]
    [PropertyRange(0f, 1f)]
    public float GlobalParamValue
    {
        get { return globalParameterValue; }
        set { globalParameterValue = value; ValueChanged?.Invoke(GlobalParameterName, globalParameterValue); }
    }
}