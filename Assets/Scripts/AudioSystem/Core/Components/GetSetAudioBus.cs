using FMOD.Studio;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[AddComponentMenu("Audio System Custom Components/Get Set Audio Bus")]
public class GetSetAudioBus : GetSetMixerFader
{
    [SerializeField]
    [OnCollectionChanged("SubscribeOnCollectionChange")]
    private List<AudioBusContainerClass> busList = new List<AudioBusContainerClass>();

    private void OnEnable()
    {
        if(CheckIfListIsValid(busList))
        {
            foreach(AudioBusContainerClass bus in busList)
            {
                bus.ValueChanged += OnBusValueChanged;
            }
        }
    }

    private void OnDisable()
    {
        if (CheckIfListIsValid(busList))
        {
            foreach(AudioBusContainerClass bus in busList)
            {
                bus.ValueChanged -= OnBusValueChanged;
            }
        }
    }
}