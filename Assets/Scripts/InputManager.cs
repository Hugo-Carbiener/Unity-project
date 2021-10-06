using UnityEngine;

public class InputManager : InputManager
{
    // Events
    public static event moveInputHandler onMoveInput;
    public static event zoomInputHandler onZoomInput;
    public static event selectionInputHandler onSelectInput;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Z))
        {
            onMoveInput?.Invoke(Vector2.up);
        }
        if (Input.GetKey(KeyCode.S))
        {
            onMoveInput?.Invoke(Vector2.down);
        }
        if (Input.GetKey(KeyCode.D))
        {
            onMoveInput?.Invoke(Vector2.right);
        }
        if (Input.GetKey(KeyCode.Q))
        {
            onMoveInput?.Invoke(Vector2.left);
        }

        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            // mouse wheel goes up
            onZoomInput?.Invoke(-1);
        }
        if (Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            // mouse wheel goes down
            onZoomInput?.Invoke(1);
        }

        if (Input.GetMouseButtonDown(0))
        {
            onSelectInput?.invoke(Vector3 Input.mousePosition);
        }
    }
}
