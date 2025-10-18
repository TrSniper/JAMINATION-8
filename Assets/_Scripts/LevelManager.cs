// In LevelManager.cs
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance; // Singleton

    public event Action OnLevelWin;
    public event Action OnLevelLose;

    // Keep track of all blocks that can be "awakened"
    [SerializeField] private List<Rigidbody> _levelPhysicsBlocks = new List<Rigidbody>();

    private event Action OnLoseCondition;
    private bool _lostYet = false;
    private bool _hasWon = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // At level start, you might find all pre-placed blocks
        // and add their Rigidbodies to the list, setting them to kinematic.
    }

    public void ResetLevel() // Add other level stuff here? Do we simply reload scene or????
    {
        _lostYet = false;
        _hasWon = false;
    }

    private void OnEnable()
    {
        OnLoseCondition += LevelManager_OnLoseCondition;
    }

    private void LevelManager_OnLoseCondition()
    {
        if(!_hasWon)
        {
            _lostYet = true;
            OnLevelLose.Invoke();
            ResetLevel();
            //TODO: Lose
        }    
    }

    // Called by PlayerBlockController.DropBlock() 
    public void SpawnPhysicsBlock(GameObject prefab, Vector3 position)
    {
        GameObject newBlock = Instantiate(prefab, position, Quaternion.identity);
        Rigidbody rb = newBlock.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // Spawn it "asleep" so it doesn't fall while dragging
            rb.isKinematic = true;
            _levelPhysicsBlocks.Add(rb);
        }
    }

    // Called by PlayerBlockController.DropBlock()
    public void ActivateLevelPhysics()
    {
        // "Awaken" all blocks!
        foreach (Rigidbody rb in _levelPhysicsBlocks)
        {
            if (rb != null) // Good to check
                rb.isKinematic = false;
        }

        // Now, start checking for your win/lose conditions
        StartWinCheckTimer();
    }

    private void StartWinCheckTimer()
    {
        if(!_lostYet)
       StartCoroutine(CheckStabilityCoroutine(5.0f));
    }

    IEnumerator CheckStabilityCoroutine(float time)
    {
        while(time>=0)
        {
            time -= Time.deltaTime;
            if (_lostYet) yield return null;

        }
        OnLevelWin.Invoke(); //wont get called if lose?
        yield return null;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out DynamicBlock d))
            OnLoseCondition?.Invoke();
    }
}

