// In LevelManager.cs
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance; // Singleton

    // Keep track of all blocks that can be "awakened"
    private List<Rigidbody> _levelPhysicsBlocks = new List<Rigidbody>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        // At level start, you might find all pre-placed blocks
        // and add their Rigidbodies to the list, setting them to kinematic.
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
        // e.g., StartCoroutine(CheckStabilityCoroutine(3.0f));
    }
}