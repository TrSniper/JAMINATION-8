// In PlayerBlockController.cs
using UnityEngine;

public class PlayerBlockController : MonoBehaviour
{
    [Header("Snapping")]
    [SerializeField] private float _snapIncrement = 1.0f;
    [SerializeField] private float _snapAdjustStep = 0.1f;
    [SerializeField] private float _minSnap = 0.1f;
    [SerializeField] private float _maxSnap = 5.0f;
    [SerializeField] private LayerMask _dropPlaneLayer; // A flat, invisible plane for raycasting

    private bool _isDragging = false;
    private bool _isPrecisionMode = false;

    private BlockStock _currentBlockStock;
    private GameObject _ghostBlockInstance;
    private Camera _mainCam;

    void Start()
    {
        _mainCam = Camera.main;
    }

    void OnEnable()
    {
        // Listen to UI
        BlockPalette.OnBlockDragStarted += HandleDragStarted;

        // Listen to InputManager
        InputManager.OnPrimaryHold += HandleDragUpdate;
        InputManager.OnPrimaryRelease += HandleDragRelease;
        InputManager.OnPrecisionModeStarted += () => _isPrecisionMode = true;
        InputManager.OnPrecisionModeEnded += () => _isPrecisionMode = false;
        InputManager.OnSnapIncrement += HandleSnapIncrement;
        InputManager.OnSnapDecrement += HandleSnapDecrement;

        // Listen to GameManager
        GameManager.OnDropTimerExpired += HandleTimerExpired;
    }

    void OnDisable()
    {
        // Unsubscribe from all events...
    }

    private void HandleDragStarted(BlockStock blockToDrag)
    {
        _isDragging = true;
        _currentBlockStock = blockToDrag;

        // Spawn the hollow "ghost"
        _ghostBlockInstance = Instantiate(blockToDrag.ghostPrefab);

        // Tell GameManager to change state and start the timer
        GameManager.Instance.SetState(GameState.DraggingBlock);
        GameManager.Instance.StartDropTimer(5.0f);
    }

    private void HandleDragUpdate(Vector2 mousePos)
    {
        if (!_isDragging) return;

        // Move the ghost block
        Vector3 worldPos = GetWorldPosition(mousePos);
        _ghostBlockInstance.transform.position = worldPos;
    }

    private void HandleDragRelease(Vector2 mousePos)
    {
        if (!_isDragging) return;
        DropBlock();
    }

    private void HandleTimerExpired()
    {
        if (!_isDragging) return;
        DropBlock();
    }

    private void DropBlock()
    {
        _isDragging = false;
        GameManager.Instance.StopDropTimer();

        // Get final drop position
        Vector3 dropPosition = _ghostBlockInstance.transform.position;

        // Destroy the ghost
        Destroy(_ghostBlockInstance);
        _ghostBlockInstance = null;

        // --- THIS IS THE KEY PART ---

        // 1. Tell LevelManager to spawn the *real* physics block
        LevelManager.Instance.SpawnPhysicsBlock(_currentBlockStock.realPrefab, dropPosition);

        // 2. Tell LevelManager to "awaken" physics
        LevelManager.Instance.ActivateLevelPhysics();

        // 3. Set game state back to 'Playing'
        GameManager.Instance.SetState(GameState.Playing);
    }

    // This function converts mouse position to a world point
    private Vector3 GetWorldPosition(Vector2 mousePos)
    {
        Ray ray = _mainCam.ScreenPointToRay(mousePos);
        if (Physics.Raycast(ray, out RaycastHit hit, 100f, _dropPlaneLayer))
        {
            Vector3 worldPos = hit.point;

            // Apply snapping if in precision mode
            if (_isPrecisionMode)
            {
                worldPos.x = Mathf.Round(worldPos.x / _snapIncrement) * _snapIncrement;
                worldPos.y = Mathf.Round(worldPos.y / _snapIncrement) * _snapIncrement;
                worldPos.z = Mathf.Round(worldPos.z / _snapIncrement) * _snapIncrement;
            }
            return worldPos;
        }
        return Vector3.zero; // Fallback
    }

    // --- Snap Adjustment Handlers ---
    private void HandleSnapIncrement()
    {
        _snapIncrement = Mathf.Clamp(_snapIncrement + _snapAdjustStep, _minSnap, _maxSnap);
        // You could tell the UIManager to show this new value
        // UIManager.Instance.ShowSnapValue(_snapIncrement);
    }

    private void HandleSnapDecrement()
    {
        _snapIncrement = Mathf.Clamp(_snapIncrement - _snapAdjustStep, _minSnap, _maxSnap);
        // UIManager.Instance.ShowSnapValue(_snapIncrement);
    }
}
