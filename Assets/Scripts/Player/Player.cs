using SOS;
using UnityEngine;
using UnityEngine.InputSystem;

/*
    ------------------- Review -------------------
    Revivewed by: CarGAnd 14/11/2023 [Accepted]

    ----------------- Player -----------------

    This class is responsible for handling player input. It is responsible for firing the following events:
    - ToggleBuildMode
    - PlaceTurnModule
    - PlaceElseGateModule
    - PlaceMergeModule
    - SelectModule

    We need to consider if we can expose the events in Modules or the action maps.
    This would allow us to attach to the events from different scripts rather than filling up this one.
*/
public class Player : MonoBehaviour
{
    [SerializeField] private LevelStateRef currentLevelStateRef;
    bool buildModeEnabled = false;
    PlayerControls playerControls;

    private void OnEnable()
    {
        playerControls.Modules.Enable();
    }

    private void OnDisable()
    {
        playerControls.Modules.Disable();
    }

    void Awake()
    {
        playerControls = new PlayerControls();
    }

    private void OnToggleBuildMode(InputAction.CallbackContext context)
    {
        buildModeEnabled = !buildModeEnabled;
    }

    private void OnSelectModule(InputAction.CallbackContext context)
    {
        if(currentLevelStateRef.Value != LevelState.BuildPhase)
        {
            return;
        }
    }
}
