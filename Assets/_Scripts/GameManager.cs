// In GameManager.cs
using System;
using System.Collections;
using UnityEngine;

public enum GameState { MainMenu, Playing, DraggingBlock, Paused, LevelComplete }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance; // Singleton for easy access
    public static event Action OnDropTimerExpired;

    public GameState CurrentState { get; private set; }
    private Coroutine _dropTimerCoroutine;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public void SetState(GameState newState)
    {
        CurrentState = newState;
        // You can fire an event here if other systems need to react
        // OnGameStateChanged?.Invoke(newState);
    }

    public void StartDropTimer(float duration)
    {
        if (_dropTimerCoroutine != null)
            StopCoroutine(_dropTimerCoroutine);
        _dropTimerCoroutine = StartCoroutine(DropTimerCoroutine(duration));
    }

    public void StopDropTimer()
    {
        if (_dropTimerCoroutine != null)
        {
            StopCoroutine(_dropTimerCoroutine);
            _dropTimerCoroutine = null;
        }
    }

    private IEnumerator DropTimerCoroutine(float duration)
    {
        yield return new WaitForSeconds(duration);
        _dropTimerCoroutine = null;
        OnDropTimerExpired?.Invoke(); // Tell the world the timer ran out!
    }
}