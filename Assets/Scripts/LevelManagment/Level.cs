
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using System.IO;

public class Level : MonoBehaviour
{

    public Player player;
    public static Level CurrentLevel;
    [SerializeField] public int LevelNumber;
    [SerializeField] public Transform startLocation;
    [SerializeField] public float maxZoom = 2f;
    public bool hasCollectedBlueprint = false;
    public Sprite FinishItemSprite;
    public Sprite BlueprintSprite;
    public bool completed = false;
    [HideInInspector]
    public Collider2D collider;
    public bool addedToBook = false;
    public bool starAddedToBook = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        CurrentLevel = this;
        collider = GetComponent<BoxCollider2D>();

    }

    private void Start()
    {
        player = GameObject.Find("player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {



    }

    public bool isMouseOverLevel()
    {
        Vector2  mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
        return collider.OverlapPoint(mouseWorldPos);

    }
}
[System.Serializable]
public class LevelSaveData
{   
    public static string path = Path.Combine(Application.persistentDataPath, "crazy.json");
    
    // Static list containing all save data
    public static List<LevelSaveData> SaveData = new List<LevelSaveData>();
    
    public int levelNumber;
    public bool completed;
    public bool hasCollectedBlueprint;
    public float EntropyAtEnd;
    public bool addedToBook;
    public bool starAddedToBook;

    public LevelSaveData(Level level)
    {
        foreach (var d in SaveData)
        {
            if (d.levelNumber == level.LevelNumber)
            {
                d.levelNumber = level.LevelNumber;
                d.completed = level.completed;
                d.hasCollectedBlueprint = level.hasCollectedBlueprint;
                d.EntropyAtEnd = level.player.Entropy;
                d.addedToBook = level.addedToBook;
                d.starAddedToBook = level.starAddedToBook;
                return;
            }

        }
        levelNumber = level.LevelNumber;
        completed = level.completed;
        hasCollectedBlueprint = level.hasCollectedBlueprint;
        EntropyAtEnd = level.player.Entropy;
        addedToBook = level.addedToBook;
        starAddedToBook = level.starAddedToBook;
        SaveData.Add(this);

    }

    public static void deleteAllData()
    {
        SaveData = new List<LevelSaveData>();
        string json = JsonUtility.ToJson("", true);
        File.WriteAllText(path, json);
    }

    public static void SaveAllData()
    {
        // Create a wrapper for proper JSON serialization
        var wrapper = new LevelSaveDataWrapper();
        wrapper.data = SaveData;
        
        string json = JsonUtility.ToJson(wrapper, true);
        File.WriteAllText(path, json);
    }



    public static void LoadAllData()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            var wrapper = JsonUtility.FromJson<LevelSaveDataWrapper>(json);
            
            // Clear existing data before loading
            SaveData.Clear();
            
            if (wrapper != null && wrapper.data != null)
            {
                SaveData.AddRange(wrapper.data);
                
            }
        }
    }
    
    // Wrapper class for proper JSON serialization
    [System.Serializable]
    private class LevelSaveDataWrapper
    {
        public List<LevelSaveData> data;
    }

    // Helper method to get data for a specific level
    public static LevelSaveData GetDataForLevel(int levelNumber)
    {
        return SaveData.Find(data => data.levelNumber == levelNumber);
    }
}