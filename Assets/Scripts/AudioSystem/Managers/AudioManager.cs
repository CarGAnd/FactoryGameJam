using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class AudioManager : MonoBehaviour
{
    private List<EventInstance> eventInstances;
    private List<EventInstance> snapshotInstances;

    private Dictionary<string, PARAMETER_ID> globalParamIDCache = new Dictionary<string, PARAMETER_ID>();

    public static AudioManager instance { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Debug.Log("Found more than one Audio Manager in the same scene");
        }
        instance = this;

        eventInstances = new List<EventInstance>();
        snapshotInstances = new List<EventInstance>();
    }

    private void OnDestroy()
    {
        CleanUp();
    }
    
    public EventInstance CreateEventInstance(EventReference eventReference, Vector3 worldPos, out EventDescription eventDescription)
    {
        EventInstance eventInstance = RuntimeManager.CreateInstance(eventReference);
        eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(worldPos));
        eventInstance.getDescription(out eventDescription);
        eventInstances.Add(eventInstance);
        return eventInstance;
    }

    public EventInstance CreateSnapshotInstance(EventReference snapshotReference)
    {
        EventInstance snapshotInstance = RuntimeManager.CreateInstance(snapshotReference);
        snapshotInstances.Add(snapshotInstance);
        return snapshotInstance;
    }

    #region Local Modulators
    public PARAMETER_DESCRIPTION GetLocalParameterDescription(EventDescription eventDescription, string localParameterName)
    {
        PARAMETER_DESCRIPTION localParameterDescription;
        if (eventDescription.getParameterDescriptionByName(localParameterName, out localParameterDescription) == FMOD.RESULT.OK)
        {
            return localParameterDescription;
        }
        UnityEngine.Debug.LogWarning("Local parameter with the name '" + localParameterName + "' does not exist");
        return default;
    }
    public PARAMETER_ID GetLocalParameterID(PARAMETER_DESCRIPTION localParameterDescription)
    {
        PARAMETER_ID localParameterID;
        localParameterID = localParameterDescription.id;
        return localParameterID;
    }
    #endregion

    #region Global Modulators

    public void CacheGlobalParamIDByName(string globalParamName)
    {
        if (!globalParamIDCache.ContainsKey(globalParamName))
        {
            PARAMETER_DESCRIPTION globalParamDescription;

            if (RuntimeManager.StudioSystem.getParameterDescriptionByName(globalParamName, out globalParamDescription) == FMOD.RESULT.OK)
            {
                PARAMETER_ID globalParamID = globalParamDescription.id;
                globalParamIDCache[globalParamName] = globalParamID;
                return;
            }
            Debug.LogWarning("Global Parameter with the name '" + globalParamName + "' does not exist");
        }
    }

    public void SetGlobalParameter(string globalParamName, float globalParamValue)
    {
        if (!globalParamIDCache.TryGetValue(globalParamName, out PARAMETER_ID cachedGlobalParamID))
        {
            return;
        }
        RuntimeManager.StudioSystem.setParameterByID(cachedGlobalParamID, globalParamValue);
    }

    #endregion

    #region Pause and Resume playback
    // Pause all FmodEvents and resume from paused place in timeline
    public void PauseAllFmodEvents()
    {
        // Pause all EventInstances
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.setPaused(true);
        }
    }

    public void ResumeAllFmodEvents()
    {
        // Resume all EventInstances
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.setPaused(false);
        }
    }
    #endregion

    private void CleanUp()
    {
        // stop and release any created instances
        foreach (EventInstance eventInstance in eventInstances)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
        foreach (EventInstance snapshotInstance in snapshotInstances)
        {
            snapshotInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            snapshotInstance.release();
        }
    }
}