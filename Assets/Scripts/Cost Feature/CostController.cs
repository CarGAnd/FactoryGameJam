using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SOS;
using Unity.VisualScripting;
using System;

public class CostController : MonoBehaviour
{
    [SerializeField]
    private IntRef runningCost;
    //private LevelStateRef levelStateRef;
    private TextMeshProUGUI costText;

    void Start () {
        costText = GetComponentInChildren<TextMeshProUGUI>();
        ResetRunningCost();
    }

    void OnEnable () {
        SubscribeToRunningCost();
    }

    void OnDisable () {
        UnsubscribeToRunningCost();
    }

    private void ResetRunningCost() {
        runningCost.Value = 0;
    }

    private void SubscribeToRunningCost() {
        runningCost.ValueChanged += UpdateCost;
    }

    private void UnsubscribeToRunningCost() {
        runningCost.ValueChanged -= UpdateCost;
    }

    private void UpdateCost(int arg1, ReferenceType type)
    {
        costText.text = $"Cost: {arg1}";
    }
}
