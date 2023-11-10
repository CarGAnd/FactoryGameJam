using FMOD;
using FMOD.Studio;
using FMODUnity;
using System;
using System.Collections.Generic;
using UnityEngine;

public class AudioMixerManager : MonoBehaviour
{
    public static AudioMixerManager instance { get; private set; }

    private Dictionary<string, GUID> vcaIDCache = new Dictionary<string, GUID>();
    private bool vcaWarningDisplayed = false;

    private Dictionary<string, GUID> busIDCache = new Dictionary<string, GUID>();
    private bool busWarningDisplayed = false;

    Bus masterBus;

    [SerializeField]
    [Range(-80f, 10f)]
    private float masterBusVolume;

    private float linearValue;

    public float MasterBusVolume
    {
        get => masterBusVolume;
        private set
        {
            masterBusVolume = value;
        }
    }

    #region Unity Methods
    private void Awake()
    {
        if (instance != null)
        {
            UnityEngine.Debug.Log("Found more than one Audio Mixer Manager in the same scene");
        }
        instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        masterBus = FMODUnity.RuntimeManager.GetBus("bus:/");
    }

    // Update is called once per frame
    private void Update()
    {
        linearValue = ConvertDbScaleToLinear(masterBusVolume);
        masterBus.setVolume(linearValue);
    }
    #endregion

    #region Logic for Bus system
    public bool CacheBusID(string busName)
    {
        if(!busIDCache.ContainsKey(busName))
        {
            Bus bus;
            if (RuntimeManager.StudioSystem.getBus("bus:/" + busName, out bus) == RESULT.OK)
            {
                GUID busID;
                bus.getID(out busID);
                busIDCache[busName] = busID;
            }
            else if (!busWarningDisplayed)
            {
                UnityEngine.Debug.LogWarning("Bus with the name '" + busName + "' does not exist");
                busWarningDisplayed = true;
                return false;
            }
            return true;
        }
        return true;
    }

    public void GetBusSetVolume(string busName, float busVolumeValue)
    {
        if (busIDCache.ContainsKey(busName))
        {
            RuntimeManager.StudioSystem.getBusByID(busIDCache[busName], out Bus cachedBus);
            if (cachedBus.isValid())
            {
                cachedBus.setVolume(busVolumeValue);
            }
            else
            {
                UnityEngine.Debug.LogWarning("cached Bus bool 'isValid' returns false");
            }
        }
        else return;
    }
    #endregion

    #region Logic for VCA system
    public bool CacheVcaID(string vcaName)
    {
        if (!vcaIDCache.ContainsKey(vcaName))
        {
            VCA vca;
            if (RuntimeManager.StudioSystem.getVCA("vca:/" + vcaName, out vca) == RESULT.OK)
            {
                FMOD.GUID vcaID;
                vca.getID(out vcaID);
                vcaIDCache[vcaName] = vcaID;
            }
            else if (!vcaWarningDisplayed)
            {
                UnityEngine.Debug.LogWarning("VCA with the name '" + vcaName + "' does not exist");
                vcaWarningDisplayed = true;
                return false;
            }
            return true;
        }
        return true;
    }

    public void GetVCASetVolume(string vcaName, float vcaVolumeValue)
    {
        if (vcaIDCache.ContainsKey(vcaName))
        {
            RuntimeManager.StudioSystem.getVCAByID(vcaIDCache[vcaName], out VCA cachedVCA);
            if (cachedVCA.isValid())
            {
                cachedVCA.setVolume(vcaVolumeValue);
            }
            else
            {
                UnityEngine.Debug.LogWarning("cached VCA bool 'isValid' returns false");
            }
        }
        else return;
    }
    #endregion

    public float ConvertDbScaleToLinear(float dbVolume)
    {
        float floatValue = Mathf.Pow(10.0f, dbVolume / 20f);
        return floatValue;
    }
}