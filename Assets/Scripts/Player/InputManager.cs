using UnityEngine;

//This class is not fully developed yet, but serves as a placeholder to implement the camera system. will definitely be revised
public class InputManager : MonoBehaviour
{
    public Vector3 GetMovementInput()
    {
        Vector3 moveDir = new Vector3();
        //For now use the old inputSystem
        //WASD interaction
        moveDir.x = Input.GetAxisRaw("Horizontal");
        moveDir.z = Input.GetAxisRaw("Vertical");
        moveDir.Normalize();

        return moveDir;
    }

    public float GetZoomInput()
    {
        float delta = Input.mouseScrollDelta.y;
        return delta;
    }

    public Vector3 GetMouseMovement() {
        if (!MouseIsInsideGame()) {
            return Vector3.zero;
        }

        //Cursor location if pushing against the bounds of the screen.
        float boundsX = 0.05f;
        float boundsY = 0.05f;

        Vector3 mousePos = Input.mousePosition;

        Vector3 mouseDir = new Vector3();
        if(mousePos.x < Screen.width * boundsX || 
           mousePos.x > Screen.width - boundsX * Screen.width ||
           mousePos.y < Screen.height * boundsY || 
           mousePos.y > Screen.height - boundsY * Screen.height) 
        {
            mouseDir.x = mousePos.x - Screen.width / 2.0f;
            mouseDir.z = mousePos.y - Screen.height / 2.0f;
        }

        mouseDir.Normalize();
        return mouseDir;
    }

    private bool MouseIsInsideGame() {
        Vector3 mousePos = Input.mousePosition;
        return mousePos.x < Screen.width && mousePos.x > 0 && mousePos.y > 0 && mousePos.y < Screen.height;
    }
}
