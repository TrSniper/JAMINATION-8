// In InputManager.cs
using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    // Mouse Events
    public static event Action<Vector2> OnPrimaryPress;    // Mouse Down
    public static event Action<Vector2> OnPrimaryHold;     // Mouse Held
    public static event Action<Vector2> OnPrimaryRelease;  // Mouse Up

    // Precision Mode Events
    public static event Action OnPrecisionModeStarted;
    public static event Action OnPrecisionModeEnded;

    // Snap Adjustment Events
    public static event Action OnSnapIncrement;
    public static event Action OnSnapDecrement;

    private bool _isPrecisionMode = false;

    void Update()
    {
        // 1. Mouse Input
        if (Input.GetMouseButtonDown(0))
            OnPrimaryPress?.Invoke(Input.mousePosition);

        if (Input.GetMouseButton(0))
            OnPrimaryHold?.Invoke(Input.mousePosition);

        if (Input.GetMouseButtonUp(0))
            OnPrimaryRelease?.Invoke(Input.mousePosition);

        // 2. Precision Mode Input
        bool ctrlHeld = Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
        if (ctrlHeld && !_isPrecisionMode)
        {
            _isPrecisionMode = true;
            OnPrecisionModeStarted?.Invoke();
        }
        else if (!ctrlHeld && _isPrecisionMode)
        {
            _isPrecisionMode = false;
            OnPrecisionModeEnded?.Invoke();
        }

        // 3. Snap Adjustment Input
        // (W/S or Up/Down)
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            OnSnapIncrement?.Invoke();

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            OnSnapDecrement?.Invoke();
    }
}
