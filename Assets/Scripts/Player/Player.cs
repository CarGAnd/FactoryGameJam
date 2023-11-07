using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector.Editor.Modules;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    bool buildModeEnabled = false;

    PlayerControls playerControls;

    void Awake()
    {
        playerControls = new PlayerControls();
        playerControls.Modules.ToggleBuildMode.performed += OnToggleBuildMode;
        playerControls.Modules.PlaceTurnGate.performed += OnPlaceTurnModule;
        playerControls.Modules.PlaceElseGate.performed += OnPlaceElseGateModule;
        playerControls.Modules.SelectModule.performed += OnSelectModule;
    }
    private void PlaceModule(ModuleTypes moduleTypes)
    {
        ModulesManager.Instance.PlaceModule(moduleTypes);
    }

    private void OnEnable()
    {
        playerControls.Modules.Enable();
    }

    private void OnDisable()
    {
        playerControls.Modules.Disable();
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


    private void OnSelectModule(InputAction.CallbackContext context)
    {
        if(LevelManager.Instance.CurrentLevelState != LevelState.BuildPhase)
        {
            return;
        }
        ModulesManager.Instance.SelectModule();
    }
}
