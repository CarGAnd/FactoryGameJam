using UnityEngine;
using UnityEngine.InputSystem;


public class Player : MonoBehaviour
{
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
}
