using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.Experimental.Rendering.RayTracingAccelerationStructure;


public static class GetAudioSO
{
    public static FmodEventArrays_SO GetFmodEventReferences()
    {
        return AssetDatabase.LoadAssetAtPath<FmodEventArrays_SO>("Assets/Scriptable Objects/AudioSystem/FmodEventManager.asset");
    }

    public static MixerNameContainer_SO GetMixerNameReferences()
    {
        return AssetDatabase.LoadAssetAtPath<MixerNameContainer_SO>("Assets/Scriptable Objects/AudioSystem/MixerNameContainer.asset");
    }

    public static FmodSnapshotArrays_SO GetFmodSnapshotReferences()
    {
        return AssetDatabase.LoadAssetAtPath<FmodSnapshotArrays_SO>("Assets/Scriptable Objects/AudioSystem/FmodSnapshotManager.asset");
    }
}