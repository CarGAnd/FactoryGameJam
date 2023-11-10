using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using System;
using FMOD;

[AddComponentMenu("Audio System Custom Components/Audio Emitter")]
public class AudioEmitter : FMODUnity.EventHandler
{
    private Action<Vector3> TransformPositionChanged;

    private Dictionary<string, PARAMETER_ID> localParamIDCache = new Dictionary<string, PARAMETER_ID>();

    [SerializeField]
    [OnCollectionChanged("SubscribeOnCollectionChange")]
    private List<ParamContainer> localParamContainer = new List<ParamContainer>();

    [Required]
    [ValueDropdown("@GetAllFmodEventGroupNames()")]
    public string fmodGroup = null;
    [Required]
    [ValueDropdown("@GetAllFmodEventNames()")]
    public string fmodEvent = null;

    private EventInstance eventInstance;
    private EventDescription eventDescription;

    [SerializeField]
    private float delayTime;

    private float currentTimerValue;

    private AudioTimer audioTimer;

    private bool timerIsAdded = false;

    private Vector3 thisTransformPosition;

    public EmitterGameEvent PlayEvent = EmitterGameEvent.None;
    public EmitterGameEvent StopEvent = EmitterGameEvent.None;

    public Vector3 ThisTransformPosition
    {
        get { return thisTransformPosition; }
        set { thisTransformPosition = value; TransformPositionChanged?.Invoke(thisTransformPosition); }
    }
    protected override void HandleGameEvent(EmitterGameEvent gameEvent)
    {
        if (PlayEvent == gameEvent)
        {
            thisTransformPosition = transform.position;
            PlaySoundEvent();
        }
        if (StopEvent == gameEvent)
        {
            StopSoundEvent();
        }
    }

    #region Unity Methods

    private void Update()
    {
        bool delaySoundFlag = DelaySound();

        if (delaySoundFlag && GetPlaybackState(eventInstance) != PLAYBACK_STATE.PLAYING)
        {
            CallSoundEvent();
        }

        if (!eventInstance.isValid())
        {
            return;
        }
        else if (transform.hasChanged)
        {
            ThisTransformPosition = transform.position;
            transform.hasChanged = false;
        }
    }

    private void OnEnable()
    {
        HandleGameEvent(EmitterGameEvent.ObjectEnable);

        if (CheckIfLocalParamListIsValid(localParamContainer))
        {
            foreach (ParamContainer param in localParamContainer)
            {
                param.ValueChanged += OnLocalParamValueChange;
            }
        }

        TransformPositionChanged += OnThisTransformPositionChange;
    }

    private void OnDisable()
    {
        HandleGameEvent(EmitterGameEvent.ObjectDisable);

        //StopSoundEvent();
        if (CheckIfLocalParamListIsValid(localParamContainer))
        {
            foreach (ParamContainer param in localParamContainer)
            {
                param.ValueChanged -= OnLocalParamValueChange;
            }
        }

        TransformPositionChanged -= OnThisTransformPositionChange;
    }
    #endregion

    #region Sound Event Methods
    [Button]
    public void PlaySoundEvent()
    {
        if (GetPlaybackState(eventInstance) == PLAYBACK_STATE.PLAYING)
        {
            UnityEngine.Debug.LogWarning("Only one Event Instance can be active at a time in the same Audio Emitter component.");
            return;
        }
        else
        { 
            CheckIfDelayTimeIsUsed();

            if (audioTimer == null)
            {
                CallSoundEvent();
            }
            else
            {
                return;
            }
        }
    }

    public void CheckIfDelayTimeIsUsed()
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

    private bool DelaySound()
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

    private void CallSoundEvent()
    {
        if (fmodGroup != null && fmodEvent != null)
        {
            Vector3 worldPos = this.transform.position;
            eventInstance = AudioManager.instance.CreateEventInstance(GetAudioSO.GetFmodEventReferences().GetFmodEventReference(fmodGroup, fmodEvent), worldPos, out eventDescription);

            foreach (ParamContainer param in localParamContainer)
            {
                PARAMETER_DESCRIPTION localParameterDescription;
                PARAMETER_ID localParameterID;
                localParameterDescription = AudioManager.instance.GetLocalParameterDescription(eventDescription, param.LocalParamName);
                localParameterID = AudioManager.instance.GetLocalParameterID(localParameterDescription);
                CacheLocalParamID(param.LocalParamName, localParameterID);
                SetLocalParamValue(param.LocalParamName, param.LocalParamValue);
            }

            eventInstance.start();
        }
        else
        {
            UnityEngine.Debug.LogError("FmodEventManager instance not found");
        }
    }

    [Button]
    public void StopSoundEvent()
    {
        if (GetPlaybackState(eventInstance) != PLAYBACK_STATE.PLAYING && !eventInstance.isValid())
        {
            //Debug.LogWarning("Event Instance has been released.");
            return;
        }

        eventInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        eventInstance.release();
    }

    private PLAYBACK_STATE GetPlaybackState(EventInstance instance)
    {
        PLAYBACK_STATE playbackState;
        instance.getPlaybackState(out playbackState);
        return playbackState;
    }

    #endregion

    #region Spatial Audio Methods
    private void OnThisTransformPositionChange(Vector3 thisTransform)
    {
        SetNewSoundPosition(thisTransform);
    }

    private void SetNewSoundPosition(Vector3 thisTransform)
    {
        eventInstance.set3DAttributes(RuntimeUtils.To3DAttributes(thisTransform));
    }
    #endregion

    #region Local Parameter Methods
    public void CacheLocalParamID(string localParameterName, PARAMETER_ID localParameterID)
    {
        if (!localParamIDCache.ContainsValue(localParameterID))
        {
            localParamIDCache[localParameterName] = localParameterID;
        }
    }

    private void OnLocalParamValueChange(string _name, float _float)
    {
        SetLocalParamValue(_name, _float);
    }

    public void SetLocalParamValue(string localParameterName, float _float)
    {
        if (!localParamIDCache.TryGetValue(localParameterName, out PARAMETER_ID cachedLocalParameterID) && !eventInstance.isValid())
        {
            return;
        }
        eventInstance.setParameterByID(cachedLocalParameterID, _float);
    }

    public bool CheckIfLocalParamListIsValid(List<ParamContainer> localParamNamesList)
    {
        if (localParamNamesList == null || localParamNamesList.Count <= 0)
            return false;
        return true;
    }

    protected void SubscribeOnCollectionChange(CollectionChangeInfo info, object value)
    {
        if (!EditorApplication.isPlaying)
            return;

        if (info.ChangeType == CollectionChangeType.Add || info.ChangeType == CollectionChangeType.Insert)
        {
            if (info.Value is not ParamContainer)
                return;

            if (info.Value is ParamContainer)
            {
                (info.Value as ParamContainer).ValueChanged += OnLocalParamValueChange;
            }
        }
    }
    #endregion

    #region Playback Behaviour
    // methods for playback behaviour as looping, pausing, jump to marked frames in event, etc.

    [Button]
    public void SetNextKeyOff()
    {
        eventInstance.keyOff();
    }

    #endregion


    #region Get Fmod Event and Group methods
    private List<string> GetAllFmodEventGroupNames()
    {
        List<string> result = new List<string>();
        result.Add(null);
        result.AddRange(GetAudioSO.GetFmodEventReferences().GetAllFmodEventGroupNames());
        return result;
    }

    private List<string> GetAllFmodEventNames()
    {
        List<string> result = new List<string>();
        if (!string.IsNullOrEmpty(fmodGroup))
        {
            result.AddRange(GetAudioSO.GetFmodEventReferences().GetFmodEventGroupByName(fmodGroup).GetAllFmodEventNames());
        }
        else
        {
            result.Add(null);
        }
        return result;
    }
    #endregion

    #region For Testing
    //[Button]
    //public void IsEventInstanceValid()
    //{
    //    if(eventInstance.isValid())
    //        return;
    //    Debug.Log("No");
    //}
    #endregion
}