using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level Data", menuName = "ScriptableObjects/Level Data")]
public class LevelDataSO : ScriptableObject
{
    public string levelName;
    public int levelIndex;
    public bool isUnlocked;
    public int starsEarned;
    public string sceneName;


    // Additional properties and methods can be added as needed
}

