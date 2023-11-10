using FMODUnity;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct FmodSnapshotGroup
{
    public FmodSnapshotGroup(string groupName)
    {
        FmodSnapshotGroupName = groupName;
        FmodSnapshotNames = new List<FmodSnapshot>();
    }

    public string FmodSnapshotGroupName;

    public List<FmodSnapshot> FmodSnapshotNames;

    public void AddToFmodSnapshotGroup(string fmodSnapshotName)
    {
        if (string.IsNullOrEmpty(fmodSnapshotName))
        {
            return;
        }

        if (FmodSnapshotNames == null)
        {
            FmodSnapshotNames = new List<FmodSnapshot>();
        }

        FmodSnapshotNames.Add(new FmodSnapshot(fmodSnapshotName));
    }

    public List<string> GetAllFmodSnapshotNames()
    {
        List<string> allFmodSnapshotNames = new List<string>();
        foreach (FmodSnapshot fmodSnapshot in FmodSnapshotNames)
        {
            allFmodSnapshotNames.Add(fmodSnapshot.FmodSnapshotName);
        }
        return allFmodSnapshotNames;
    }
    public EventReference GetFmodSnapshotReferenceByName(string name)
    {
        foreach (FmodSnapshot fmodSnapshot in FmodSnapshotNames)
        {
            if (fmodSnapshot.FmodSnapshotName == name)
            {
                return fmodSnapshot.snapshotReference;
            }
        }
        return default;
    }
}

[Serializable]
public struct FmodSnapshot
{
    public FmodSnapshot(string SnapshotName)
    {
        FmodSnapshotName = SnapshotName;
        snapshotReference = default;
    }

    public string FmodSnapshotName;
    public EventReference snapshotReference;
}


