// In LevelStats.cs (example)
using UnityEngine;

[CreateAssetMenu(fileName = "Level_",menuName = "SCO/Level Stats Object")]
public class LevelStats : ScriptableObject
{
    [SerializeField] BlockType[] levelSolutionBlocks;
    [SerializeField] string levelName;
    [SerializeField] int levelNumber;
}