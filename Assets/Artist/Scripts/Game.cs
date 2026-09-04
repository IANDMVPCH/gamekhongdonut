using UnityEngine;

public static class GameData
{
    public static int currentLevel = 0; // Index of current level (0 = Level 1)
    public static int backupModules = 0;

    public static int currentMoney = 100;

    // Array of scene names in level order
    public static string[] levelScenes = new string[]
    {
        "Water",
        "Rock",
        "Ice",
        "Lightning",
        "Toxic",
        "Lava",
        "Bossmap"
    };

    public static void ResetData()
    {
        currentLevel = 0;
        backupModules = 0;
    }
}