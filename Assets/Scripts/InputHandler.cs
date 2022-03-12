using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InputHandler : MonoBehaviour
{
    public delegate void moveInputHandler(Vector2 moveVector);
    public delegate void zoomInputHandler(float zoomAmount);
    public delegate void selectionInputHandler(Vector2 mousePosition);
    public delegate void menuInputHandler();
}
