using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEditor;

public class SimpleGetSetGlobalParameter : MonoBehaviour
{

    [SerializeField]
    [OnCollectionChanged("SubscribeOnCollectionChange")]
    private List<GlobalParamContainer> globalParamContainer = new List<GlobalParamContainer>();

    private void OnEnable()
    {
        if(CheckIfGlobalParamListIsValid(globalParamContainer))
        {
            foreach (GlobalParamContainer globalParam in globalParamContainer)
            {
                globalParam.ValueChanged += OnGlobalParamValueChange;
            }
        }
    }

    private void OnDisable()
    {
        if (CheckIfGlobalParamListIsValid(globalParamContainer))
        {
            foreach (GlobalParamContainer globalParam in globalParamContainer)
            {
                globalParam.ValueChanged -= OnGlobalParamValueChange;
            }
        }
    }

    private void OnGlobalParamValueChange(string _name, float _value)
    {
        AudioManager.instance.CacheGlobalParamIDByName(_name);
        SetGlobalParamValue(_name, _value);
    }

    private void SetGlobalParamValue(string globalParameterName, float value)
    {
        AudioManager.instance.SetGlobalParameter(globalParameterName, value);
    }

    public bool CheckIfGlobalParamListIsValid(List<GlobalParamContainer> globalParamNamesList)
    {
        if (globalParamNamesList == null && globalParamNamesList.Count <= 0)
            return false;
        return true;
    }

    protected void SubscribeOnCollectionChange(CollectionChangeInfo info, object value)
    {
        if (!EditorApplication.isPlaying)
            return;

        if (info.ChangeType == CollectionChangeType.Add || info.ChangeType == CollectionChangeType.Insert)
        {
            if (info.Value is not GlobalParamContainer)
                return;

            if (info.Value is GlobalParamContainer)
            {
                (info.Value as GlobalParamContainer).ValueChanged += OnGlobalParamValueChange;
            }
        }
    }
}
