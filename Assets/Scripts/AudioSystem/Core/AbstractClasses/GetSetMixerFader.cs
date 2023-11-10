using Sirenix.OdinInspector.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class GetSetMixerFader : MonoBehaviour
{
    protected bool CheckIfListIsValid(List<AudioBusContainerClass> faderNamesList)
    {
        if (faderNamesList == null || faderNamesList.Count <= 0)
            return false;
        return true;
    }

    protected bool CheckIfListIsValid(List<AudioVCAContainerClass> faderNamesList)
    {
        if (faderNamesList == null || faderNamesList.Count <= 0)
            return false;
        return true;
    }

    protected void OnVcaValueChanged(string name, float volume)
    {
        SetVCAFaderInstance(name, volume);
    }

    protected void OnBusValueChanged(string name, float volume)
    {
        SetBusFaderInstance(name, volume);
    }

    private void SetBusFaderInstance(string name, float volume)
    {
        if (string.IsNullOrEmpty(name))
        {
            return;
        }
        if (AudioMixerManager.instance.CacheBusID(name))
        {
            float linearValue = AudioMixerManager.instance.ConvertDbScaleToLinear(volume);
            AudioMixerManager.instance.GetBusSetVolume(name, linearValue);
        }
    }

    private void SetVCAFaderInstance(string name, float volume)
    {
        if (string.IsNullOrEmpty(name))
        {
            return;
        }
        if (AudioMixerManager.instance.CacheVcaID(name))
        {
            float linearValue = AudioMixerManager.instance.ConvertDbScaleToLinear(volume);
            AudioMixerManager.instance.GetVCASetVolume(name, linearValue);
        }
    }

    protected void SubscribeOnCollectionChange(CollectionChangeInfo info, object value)
    {
        if (!EditorApplication.isPlaying)
            return;

        if (info.ChangeType == CollectionChangeType.Add || info.ChangeType == CollectionChangeType.Insert)
        {
            if (info.Value is not AudioFaderContainerClass)
                return;

            if (info.Value is AudioBusContainerClass)
            {
                (info.Value as AudioBusContainerClass).ValueChanged += OnBusValueChanged;
            }
            else if (info.Value is AudioVCAContainerClass)
            {
                (info.Value as AudioVCAContainerClass).ValueChanged += OnVcaValueChanged;
            }
        }
    }
}