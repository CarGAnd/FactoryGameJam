using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using FMOD.Studio;
using System;
using FMODUnity;

[AddComponentMenu("Audio System Custom Components/Audio Snapshot Trigger")]
public class AudioSnapshotTrigger : FMODUnity.EventHandler
{
    [ValueDropdown("@GetAllFmodSnapshotGroupNames()")]
    [Required]
    public string snapshotGroup = null;

    [ValueDropdown("@GetAllFmodSnapshotNames()")]
    [Required]
    public string snapshot = null;

    private EventInstance snapshotInstance;

    [SerializeField]
    private float delayTime;

    private float currentTimerValue;

    private AudioTimer audioTimer;

    private bool timerIsAdded = false;

    //[SerializeField]
    //private bool initiateOnStart = false;

    private bool snapshotInitialized = false;

    public EmitterGameEvent StartSnapshot = EmitterGameEvent.None;
    public EmitterGameEvent EndSnapshot = EmitterGameEvent.None;

    //private void Start()
    //{
    //    if (!initiateOnStart)
    //    {
    //        return;
    //    }
    //    else
    //    {
    //        TriggerSnapshot();
    //    }
    //}

    protected override void HandleGameEvent(EmitterGameEvent gameEvent)
    {
        if (StartSnapshot == gameEvent)
        {
            TriggerSnapshot();
        }
        if (EndSnapshot == gameEvent)
        {
            StopSnapshot();
        }
    }

    private void Update()
    {
        bool delaySoundFlag = DelayTrig();

        if (delaySoundFlag && !snapshotInitialized)
        {
            CallSnapshot();
        }
    }

    #region Snapshot Trigger Methods

    [Button]
    private void TriggerSnapshot()
    {
        if (!snapshotInitialized)
        {
            CheckIfDelayTrigIsUsed();

            if (audioTimer == null)
            {
                CallSnapshot();
            }
            else return;
        }
    }

    private void CheckIfDelayTrigIsUsed()
    {
        if (delayTime != 0f)
        {
            audioTimer = gameObject.AddComponent<AudioTimer>();
            timerIsAdded = true;

            audioTimer.SetTimer(delayTime);
            audioTimer.StartTimer();
        }
        else return;
    }

    private bool DelayTrig()
    {
        if (!timerIsAdded)
        {
            return false;
        }
        else if (audioTimer.run)
        {
            currentTimerValue = audioTimer.timer;
            if (currentTimerValue <= 0f)
            {
                audioTimer.StopTimer();
                Destroy(audioTimer);
                return true;
            }
            return false;
        }
        return false;
    }

    private void CallSnapshot()
    {
        if (snapshotInitialized)
        {
            Debug.LogWarning("Snapshot is already initiated. Multiple snapshots cannot run in the same component.");
            return;
        }
        else if (snapshotGroup != null && snapshot != null)
        {
            snapshotInstance = AudioManager.instance.CreateSnapshotInstance(GetAudioSO.GetFmodSnapshotReferences().GetFmodSnapshotReference(snapshotGroup, snapshot));
            snapshotInstance.start();

            snapshotInitialized = true;
        }
    }

    [Button]
    public void StopSnapshot()
    {
        if (!snapshotInitialized && !snapshotInstance.isValid())
        {
            //Debug.LogWarning("Cannot invoke method 'StopSnapshot'. No snapshot has been initiated.");
            return;
        }

        snapshotInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        snapshotInstance.release();
        snapshotInitialized = false;
    }

    #endregion

    #region Get Fmod Snapshot and Group methods
    private List<string> GetAllFmodSnapshotGroupNames()
    {
        List<string> result = new List<string>();
        result.Add(null);
        result.AddRange(GetAudioSO.GetFmodSnapshotReferences().GetAllFmodSnapshotGroupNames());
        return result;
    }

    private List<string> GetAllFmodSnapshotNames()
    {
        List<string> result = new List<string>();
        if (!string.IsNullOrEmpty(snapshotGroup))
        {
            result.AddRange(GetAudioSO.GetFmodSnapshotReferences().GetFmodSnapshotGroupByName(snapshotGroup).GetAllFmodSnapshotNames());
        }
        else
        {
            result.Add(null);
        }
        return result;
    }
    #endregion
}