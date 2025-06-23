using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
public class Level : MonoBehaviour
{

    public static Dictionary<int, Level> Levels = new Dictionary<int, Level>();
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
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        Levels.Add(LevelNumber,this);
        collider = GetComponent<BoxCollider2D>();

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

    public LevelSaveData(Level level)
    {
        foreach (var d in SaveData)
        {
            if (d.levelNumber == level.LevelNumber)
            {
                d.levelNumber = level.LevelNumber;
                d.completed = level.completed;
                d.hasCollectedBlueprint = level.hasCollectedBlueprint;
                return;
            }

        }
        levelNumber = level.LevelNumber;
        completed = level.completed;
        hasCollectedBlueprint = level.hasCollectedBlueprint;
        SaveData.Add(this);

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
                
                foreach (var levelData in SaveData)
                {
                    if (Level.Levels.TryGetValue(levelData.levelNumber, out Level levelToLoad))
                    {
                        levelToLoad.hasCollectedBlueprint = levelData.hasCollectedBlueprint;
                        levelToLoad.completed = levelData.completed;
                    }
                }
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