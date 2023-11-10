using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AudioFaderContainerClass
{
    // realized I didn't need this. but I am gonna keep it. pyt.
}

[InlineEditor]
[Serializable]
public class AudioBusContainerClass : AudioFaderContainerClass
{
    public Action<string, float> ValueChanged;

    [ValueDropdown("@GetBusNames()")]
    [Required]
    [SerializeField]
    public string busName;

    [SerializeField]
    [HideInInspector]
    private float busVolume;

    public AudioBusContainerClass(string _busName, float _busVolume = 0f)
    {
        busName = _busName;
        BusVolume = _busVolume;
    }

    [ShowInInspector]
    [PropertyRange(-80f, 10f)]
    public float BusVolume
    {
        get { return busVolume; }
        set { busVolume = value; ValueChanged?.Invoke(busName, busVolume); }
    }

    private List<string> GetBusNames()
    {
        return GetAudioSO.GetMixerNameReferences().busNameContainer;
    }
}

[InlineEditor]
[Serializable]
public class AudioVCAContainerClass : AudioFaderContainerClass
{
    public Action<string, float> ValueChanged;

    [ValueDropdown("@GetVcaNames()")]
    [Required]
    [SerializeField]
    public string vcaName;

    [SerializeField]
    [HideInInspector]
    private float vcaVolume;

    public AudioVCAContainerClass(string _vcaName, float _vcaVolume = 0f)
    {
        vcaName = _vcaName;
        VcaVolume = _vcaVolume;
    }

    [ShowInInspector]
    [PropertyRange(-80f, 10f)]
    public float VcaVolume
    {
        get { return vcaVolume; }
        set { vcaVolume = value; ValueChanged?.Invoke(vcaName, vcaVolume); }
    }

    private List<string> GetVcaNames()
    {
        return GetAudioSO.GetMixerNameReferences().vcaNameContainer;
    }
}

