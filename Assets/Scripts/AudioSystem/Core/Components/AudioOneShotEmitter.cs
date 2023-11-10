using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using System;

[AddComponentMenu("Audio System Custom Components/Audio OneShot Emitter")]
public class AudioOneShotEmitter : FMODUnity.EventHandler
{
    private Action<Vector3> TransformPositionChanged;

    private Dictionary<string, PARAMETER_ID> localParamIDCache = new Dictionary<string, PARAMETER_ID>();

    [SerializeField]
    [OnCollectionChanged("SubscribeOnCollectionChange")]
    private List<ParamContainer> localParamContainer = new List<ParamContainer>();

    private List<EventInstance> componentSpecificOneShotList = new List<EventInstance>();

    [Required]
    [ValueDropdown("@GetAllFmodEventGroupNames()")]
    public string fmodGroup = null;
    [Required]
    [ValueDropdown("@GetAllFmodEventNames()")]
    public string fmodEvent = null;

    private EventInstance oneShotInstance;
    private EventDescription oneShotDescription;

    private Vector3 thisTransformPosition;
    public Vector3 ThisTransformPosition
    {
        get { return thisTransformPosition; }
        set { thisTransformPosition = value; TransformPositionChanged?.Invoke(thisTransformPosition); }
    }

    public EmitterGameEvent PlayOneshot = EmitterGameEvent.None;
    public EmitterGameEvent StopAllOneshots = EmitterGameEvent.None;

    protected override void HandleGameEvent(EmitterGameEvent gameEvent)
    {
        if (PlayOneshot == gameEvent)
        {
            thisTransformPosition = transform.position;
            PlayOneShotEvent();
        }
        if (StopAllOneshots == gameEvent)
        {
            StopAllOneShotEvents();
        }
    }

    #region Unity Methods
    private void Update()
    {
        if (transform.hasChanged)
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

        if (CheckIfLocalParamListIsValid(localParamContainer))
        {
            foreach (ParamContainer param in localParamContainer)
            {
                param.ValueChanged -= OnLocalParamValueChange;
            }
        }

        TransformPositionChanged -= OnThisTransformPositionChange;
        StopAllOneShotEvents();
    }
    #endregion

    #region Sound Event Methods
    [Button]
    public void PlayOneShotEvent()
    {
        if (fmodGroup != null && fmodEvent != null)
        {
            Vector3 worldPos = transform.position;
            oneShotInstance = AudioManager.instance.CreateEventInstance(GetAudioSO.GetFmodEventReferences().GetFmodEventReference(fmodGroup, fmodEvent), worldPos, out oneShotDescription);
            componentSpecificOneShotList.Add(oneShotInstance);

            foreach (ParamContainer param in localParamContainer)
            {
                PARAMETER_DESCRIPTION localParameterDescription;
                PARAMETER_ID localParameterID;
                localParameterDescription = AudioManager.instance.GetLocalParameterDescription(oneShotDescription, param.LocalParamName);
                localParameterID = AudioManager.instance.GetLocalParameterID(localParameterDescription);
                CacheLocalParamID(param.LocalParamName, localParameterID);
                SetLocalParamValue(param.LocalParamName, param.LocalParamValue);
            }

            oneShotInstance.start();
            oneShotInstance.release();
        }
        else
        {
            Debug.LogError("FmodEventManager instance not found");
        }
    }

    [Button]
    public void StopAllOneShotEvents()
    {
        foreach (EventInstance eventInstance in componentSpecificOneShotList)
        {
            eventInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            eventInstance.release();
        }
    }
    #endregion


    private void OnThisTransformPositionChange(Vector3 thisTransform)
    {
        if (!CheckIfComponentSpecificOneShotListIsValid(componentSpecificOneShotList))
            return;

        SetNewSoundPosition(thisTransform);
    }

    private void SetNewSoundPosition(Vector3 thisTransform)
    {
        foreach (EventInstance oneShotInstance in componentSpecificOneShotList)
        {
            oneShotInstance.set3DAttributes(RuntimeUtils.To3DAttributes(thisTransform));
        }
    }
    private bool CheckIfComponentSpecificOneShotListIsValid(List<EventInstance> componentSpecificOneShotList)
    {
        if (componentSpecificOneShotList == null || componentSpecificOneShotList.Count <= 0)
        {

            return false;

        }
        //Debug.Log("CheckIfComponentSpecificOneShotListIsValid returns true");
        return true;
    }

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
        if (!CheckIfComponentSpecificOneShotListIsValid(componentSpecificOneShotList))
        {
            Debug.Log("CheckIfComponentSpecificOneShotListIsValid is called");
            return;
        }
        SetLocalParamValue(_name, _float);
    }

    public void SetLocalParamValue(string localParameterName, float _float)
    {
        if (!localParamIDCache.TryGetValue(localParameterName, out PARAMETER_ID cachedLocalParameterID) && !oneShotInstance.isValid())
        {
            return;
        }
        oneShotInstance.setParameterByID(cachedLocalParameterID, _float);
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
