using UnityEngine;

[CreateAssetMenu(fileName = "NewGameData", menuName = "Game/Game Data")]
public class GameDataSO : ScriptableObject
{
    public int playerScore;
    public bool isGamePaused;
    public string playerName;
    public int crystalCollected;
    // Add other fields as needed
}
