using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "FmodEventArray", menuName = "Audio/FmodEventArray", order = 1)]

public class FmodEventArrays_SO : ScriptableObject
{
    [SerializeField]
    public List<FmodEventGroup> FmodEventGroups;

    public FmodEventGroup AddToFmodEventGroups(string fmodEventGroupName)
    {
        if (string.IsNullOrEmpty(fmodEventGroupName))
        {
            return default;
        }

        if (FmodEventGroups == null)
        {
            FmodEventGroups = new List<FmodEventGroup>();
        }

        FmodEventGroup currentGroup = GetFmodEventGroupByName(fmodEventGroupName);

        if (!currentGroup.Equals(default))
        {
            return currentGroup;
        }

        currentGroup = new FmodEventGroup(fmodEventGroupName);
        FmodEventGroups.Add(currentGroup);

        return currentGroup;
    }

    public void AddEventToGroup(string fmodEventGroupName, string fmodEventName)
    {
        FmodEventGroup currentGroup = AddToFmodEventGroups(fmodEventGroupName);

        if (!currentGroup.Equals(default))
        {
            return;
        }

        currentGroup.AddToFmodEventGroup(fmodEventName);
    }

    public FmodEventGroup GetFmodEventGroupByName(string name)
    {
        foreach (FmodEventGroup group in FmodEventGroups)
        {
            if (group.FmodEventGroupName == name)
            {
                return group;
            }
        }
        return default;
    }

    public List<string> GetAllFmodEventGroupNames()
    {
        List<string> allFmodEventGroupNames = new List<string>();
        foreach (FmodEventGroup group in FmodEventGroups)
        {
            allFmodEventGroupNames.Add(group.FmodEventGroupName);
        }
        return allFmodEventGroupNames;
    }

    public EventReference GetFmodEventReference(string groupName, string eventName)
    {
        return GetFmodEventGroupByName(groupName).GetFmodEventReferenceByName(eventName);
    }
}