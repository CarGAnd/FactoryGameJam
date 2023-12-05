using UnityEngine;

//This class is not fully developed yet, but serves as a placeholder to implement the camera system. will definitely be revised
public class InputManager : MonoBehaviour
{
    public Vector3 GetMovementInput()
    {
        //WASD interaction
        //Cursor location? if pushing against the bounds of the screen.
        return new Vector3();
    }

    public float GetZoomInput()
    {
        //Mousewheel interaction
        return 0f;
    }
}
