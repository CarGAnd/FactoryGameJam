using Sirenix.OdinInspector;
using System;
using UnityEngine;


[InlineEditor]
[Serializable]
public class ParamContainer
{
    public Action <string, float> ValueChanged;

    [Required]
    [SerializeField]
    public string LocalParamName;

    [SerializeField]
    [HideInInspector]
    private float localParamValue;

    public ParamContainer(string _paramName, float _paramValue)
    {
        LocalParamName = _paramName;
        localParamValue = _paramValue;
    }

    [ShowInInspector]
    [PropertyRange(0f, 1f)]
    public float LocalParamValue
    {
        get { return localParamValue; }
        set { localParamValue = value; ValueChanged?.Invoke(LocalParamName, localParamValue); }
    }
}