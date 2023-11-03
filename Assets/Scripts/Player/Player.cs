using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    bool buildModeEnabled = false;

    PlayerControls playerControls;

    void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.ModulePlacing.ToggleBuildMode.performed += OnToggleBuildMode;
        playerControls.ModulePlacing.PlaceTurnGate.performed += OnPlaceTurnModule;
        playerControls.ModulePlacing.PlaceElseGate.performed += OnPlaceElseGateModule;
    }
    private void PlaceModule(ModuleTypes moduleTypes)
    {
        ModulesManager.Instance.PlaceModule(moduleTypes);
    }

    private void OnEnable()
    {
        playerControls.ModulePlacing.Enable();
    }

    private void OnDisable()
    {
        playerControls.ModulePlacing.Disable();
    }

    private void OnToggleBuildMode(InputAction.CallbackContext context)
    {
        buildModeEnabled = !buildModeEnabled;
        ModulesManager.Instance.ToggleGizmos(buildModeEnabled);
    }
    private void OnPlaceTurnModule(InputAction.CallbackContext context)
    {
        if (buildModeEnabled)
        {
            PlaceModule(ModuleTypes.TurnModule);
        }
    }
    private void OnPlaceElseGateModule(InputAction.CallbackContext context)
    {
        if (buildModeEnabled)
        {
            PlaceModule(ModuleTypes.ElseGateModule);
        }
    }
}
