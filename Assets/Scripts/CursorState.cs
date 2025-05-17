using UnityEngine;

public class CursorState : MonoBehaviour
{
    [SerializeField] bool cursorState = true;
    private void Start()
    {
        ApplyCursorState(cursorState);
    }

    public void ApplyCursorState(bool state)
    {
        cursorState = state;

        Cursor.visible = cursorState;
        Cursor.lockState = cursorState ? CursorLockMode.None : CursorLockMode.Locked;
    }

    // Hopefully Used to pause the game and show the cursor
    // (Maybe set the cursor to the center of the screen)
    public void ToggleCursorState()
    {
        ApplyCursorState(!cursorState);
    }

}
