using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using FMOD.Studio;
using System;

[AddComponentMenu("Audio System Custom Components/Get Set Audio VCA")]
public class GetSetAudioVCA : GetSetMixerFader
{
    [SerializeField]
    [OnCollectionChanged("SubscribeOnCollectionChange")]
    private List<AudioVCAContainerClass> vcaList = new List<AudioVCAContainerClass>();

    private void OnEnable()
    {
        if (CheckIfListIsValid(vcaList))
        {
            foreach (AudioVCAContainerClass vca in vcaList)
            {
                vca.ValueChanged += OnVcaValueChanged;
            }
        }
    }

    private void OnDisable()
    {
        if (CheckIfListIsValid(vcaList))
        {
            foreach (AudioVCAContainerClass vca in vcaList)
            {
                vca.ValueChanged -= OnVcaValueChanged;
            }
        }
    }
}