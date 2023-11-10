using FMODUnity;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct FmodEventGroup
{
    public FmodEventGroup(string groupName)
    {
        FmodEventGroupName = groupName;
        FmodEventNames = new List<FmodEvent>();
    }

    public string FmodEventGroupName;

    public List<FmodEvent> FmodEventNames;

    public void AddToFmodEventGroup(string fmodEventName)
    {
        if (string.IsNullOrEmpty(fmodEventName))
        {
            return;
        }

        if (FmodEventNames == null)
        {
            FmodEventNames = new List<FmodEvent>();
        }

        FmodEventNames.Add(new FmodEvent(fmodEventName));
    }

    public List<string> GetAllFmodEventNames()
    {
        List<string> allFmodEventNames = new List<string>();
        foreach (FmodEvent fmodEvent in FmodEventNames)
        {
            allFmodEventNames.Add(fmodEvent.FmodEventName);
        }
        return allFmodEventNames;
    }
    public EventReference GetFmodEventReferenceByName(string name)
    {
        foreach (FmodEvent fmodEvent in FmodEventNames)
        {
            if (fmodEvent.FmodEventName == name)
            {
                return fmodEvent.eventReference;
            }
        }
        return default;
    }
}

[Serializable]
public struct FmodEvent
{
    public FmodEvent(string eventName)
    {
        FmodEventName = eventName;
        eventReference = default;
    }

    public string FmodEventName;
    public EventReference eventReference;
}