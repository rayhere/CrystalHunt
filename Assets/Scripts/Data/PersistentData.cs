using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersistentData : MonoBehaviour
{
    [SerializeField] float elapsedTime;
    [SerializeField] float lastElapsedTime;
    [SerializeField] int crystalCount;

    // Keys for PlayerPrefs (adjust these as needed)
    private const string ElapsedTimeKey = "ElapsedTime";
    private const string LastElapsedTimeKey = "LastElapsedTime";
    private const string TotalElapsedTimeKey = "TotalElapsedTime";
    private const string LastCrystalCountKey = "LastCrystalCount";
    private const string TopCrystalCountKey = "TopCrystalCount";
    private const string TotalCrystalsCollectedKey = "TotalCrystalsCollected";
    private const string LongestElapsedtimeKey = "LongestElapsedtime";
    private const string RecordPlayDurationKey = "RecordPlayDuration";

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
        elapsedTime = 0;
        crystalCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Method to save data to PlayerPrefs
    public void SaveData()
    {
        if (PlayerPrefs.HasKey(TopCrystalCountKey))
        {
            int topCrystalCount = PlayerPrefs.GetInt(TopCrystalCountKey);
            if (crystalCount > topCrystalCount)
            {
                 PlayerPrefs.SetInt(TopCrystalCountKey, crystalCount);
            }
        }
        else 
        {
            PlayerPrefs.SetInt(TopCrystalCountKey, crystalCount);
        }

        if (PlayerPrefs.HasKey(TopCrystalCountKey))
        {
            PlayerPrefs.SetInt(TotalCrystalsCollectedKey, PlayerPrefs.GetInt(TotalCrystalsCollectedKey) + crystalCount);

        }
        else
        {
            PlayerPrefs.SetInt(TotalCrystalsCollectedKey, crystalCount);
        }

        if (PlayerPrefs.HasKey(LongestElapsedtimeKey))
        {
            float longestElapsedTime = PlayerPrefs.GetFloat(LongestElapsedtimeKey);
            if (elapsedTime > longestElapsedTime)
            {
                 PlayerPrefs.SetFloat(LongestElapsedtimeKey, elapsedTime);
            }
        }
        else 
        {
            PlayerPrefs.SetFloat(LongestElapsedtimeKey, elapsedTime);
        }
        
        if (PlayerPrefs.HasKey(RecordPlayDurationKey))
        {
            PlayerPrefs.SetFloat(RecordPlayDurationKey, PlayerPrefs.GetFloat(RecordPlayDurationKey) + elapsedTime);
        }
        else 
        {
            PlayerPrefs.SetFloat(RecordPlayDurationKey, elapsedTime);
        }

        PlayerPrefs.SetFloat(ElapsedTimeKey, elapsedTime);

        PlayerPrefs.Save(); // Always remember to save changes
    }

    // Method to load data from PlayerPrefs
    public void LoadData()
    {
        if (PlayerPrefs.HasKey(LastElapsedTimeKey))
        {
            lastElapsedTime = PlayerPrefs.GetFloat(LastElapsedTimeKey);
        }
    }

    public void ResetPlayer()
    {
        elapsedTime = 0;
        crystalCount = 0;
    }

    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    public void SetElapsedTime(float f)
    {
        elapsedTime = f;
    }

    public float GetLastElapsedTime()
    {
        return lastElapsedTime;
    }

    public void SetLastElapsedTime(float f)
    {
        lastElapsedTime = f;
    }

    public int GetCrystalCount()
    {
        return crystalCount;
    }

        public void SetCrystalCount(int i)
    {
        crystalCount = i;
    }

    public void IncreaseCrystalCount(int i)
    {
        SetCrystalCount(GetCrystalCount()+i);
    }
}
