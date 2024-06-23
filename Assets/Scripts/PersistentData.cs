using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour
{
    [SerializeField] string playerName;
    [SerializeField] int playerLevel;
    [SerializeField] int playerScore;
    [SerializeField] float elapsedTime;
    [SerializeField] int crystalCount;

    public static PersistentData Instance;

    void Awake()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(this);
            Debug.Log("PersistentData.sc Don't Destory On Load");
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            Debug.Log("Destroy(gameObject)");
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        playerLevel = 1;
        // if (playerLevel == null)
        //      playerLevel = 0;
        // if (playerLevel <= 0)
        //     playerLevel = 0;
        // if (playerLevel.HasValue)
        //     playerLevel = 0;
        
        // if (playerName == null)
        //     playerName = "unknown";
        if (playerScore <= 0)
            playerScore = 0;
        // if (playerScore.HasValue)
        //     playerScore = 0;

        elapsedTime = 0;
        crystalCount = 0;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetPlayer()
    {
        playerName = null;
        playerLevel = 1;
        playerScore = 0;
        elapsedTime = 0;
    }
    public void SetName(string n)
    {
        playerName = n;
    }

    public void SetLevel(int l)
    {
        playerLevel = l;
    }

    public void SetScore(int s)
    {
        playerScore = s;
    }

    public void SetElapsedTime(float r)
    {
        elapsedTime = r;
    }

    public string GetName()
    {
        return playerName;
    }

    public int GetLevel()
    {
        return playerLevel;
    }

    public int GetScore()
    {
        return playerScore;
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    public int GetCrystalCount()
    {
        return crystalCount; // crystalCollected
    }

    public void SetCrystalCount(int i)
    {
        crystalCount = i; // crystalCollected
    }

    public void IncreaseCrystalCount(int i)
    {
        SetCrystalCount(GetCrystalCount()+i);
    }
}
