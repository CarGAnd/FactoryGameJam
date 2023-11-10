using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

[CreateAssetMenu(fileName = "FmodSnapshotArray", menuName = "Audio/FmodSnapshotArray", order = 1)]

public class FmodSnapshotArrays_SO : ScriptableObject
{
    [SerializeField]
    public List<FmodSnapshotGroup> FmodSnapshotGroups;

    public FmodSnapshotGroup AddToFmodSnapshotGroups(string fmodSnapshotGroupName)
    {
        if (string.IsNullOrEmpty(fmodSnapshotGroupName))
        {
            return default;
        }

        if (FmodSnapshotGroups == null)
        {
            FmodSnapshotGroups = new List<FmodSnapshotGroup>();
        }

        FmodSnapshotGroup currentGroup = GetFmodSnapshotGroupByName(fmodSnapshotGroupName);

        if (!currentGroup.Equals(default))
        {
            return currentGroup;
        }

        currentGroup = new FmodSnapshotGroup(fmodSnapshotGroupName);
        FmodSnapshotGroups.Add(currentGroup);

        return currentGroup;
    }

    public void AddSnapshotToGroup(string fmodSnapshotGroupName, string fmodSnapshotName)
    {
        FmodSnapshotGroup currentGroup = AddToFmodSnapshotGroups(fmodSnapshotGroupName);

        if (!currentGroup.Equals(default))
        {
            return;
        }

        currentGroup.AddToFmodSnapshotGroup(fmodSnapshotName);
    }

    public FmodSnapshotGroup GetFmodSnapshotGroupByName(string name)
    {
        foreach (FmodSnapshotGroup group in FmodSnapshotGroups)
        {
            if (group.FmodSnapshotGroupName == name)
            {
                return group;
            }
        }
        return default;
    }

    public List<string> GetAllFmodSnapshotGroupNames()
    {
        List<string> allFmodSnapshotGroupNames = new List<string>();
        foreach (FmodSnapshotGroup group in FmodSnapshotGroups)
        {
            allFmodSnapshotGroupNames.Add(group.FmodSnapshotGroupName);
        }
        return allFmodSnapshotGroupNames;
    }

    public EventReference GetFmodSnapshotReference(string groupName, string snapshotName)
    {
        return GetFmodSnapshotGroupByName(groupName).GetFmodSnapshotReferenceByName(snapshotName);
    }
}