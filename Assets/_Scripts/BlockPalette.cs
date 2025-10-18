// In BlockPalette.cs (example)
using System;
using UnityEngine;

// A simple data structure to hold your block info
[System.Serializable]
public class BlockStock
{
    public BlockType type; // An enum (e.g., Square, Triangle)
    public GameObject realPrefab;
    public GameObject ghostPrefab; // The "hollow cursor"
    public int amountAvailable;
}

public enum BlockType
{
    None,
    Box,
    Normal,
    Long,
    Big,
    Gigantic
}

public class BlockPalette : MonoBehaviour
{
    // This event tells the rest of the game a drag has *successfully* started
    public static event Action<BlockStock> OnBlockDragStarted;

    // You would set this up in the inspector
    public BlockStock[] availableBlocks;

    void OnEnable()
    {
        InputManager.OnPrimaryPress += HandleClick;
    }
    void OnDisable()
    {
        InputManager.OnPrimaryPress -= HandleClick;
    }

    private void HandleClick(Vector2 mousePos)
    {
        // TODO: Your UI logic here
        // 1. Do a UI raycast (EventSystem.current.Raycast...)
        // 2. Check if the raycast hit a BlockButton
        // 3. Get the 'BlockType' from that button

        BlockStock blockToDrag = FindBlockToDrag(mousePos); // Your imaginary function

        if (blockToDrag != null && blockToDrag.amountAvailable > 0)
        {
            blockToDrag.amountAvailable--;
            // TODO: Update your UI text (e.g., "2/5")

            // Tell the rest of the game to start!
            OnBlockDragStarted?.Invoke(blockToDrag);
        }
    }

    private BlockStock FindBlockToDrag(Vector2 pos) { /* ... */ return null; }
}
