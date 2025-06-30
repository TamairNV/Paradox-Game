
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using System.IO;
public class SettingsManager : MonoBehaviour
{
    public float MasterVolume;
    public float SFXVolume;
    public float MusicVolume;

    [SerializeField] private Rocker MasterRocker;
    [SerializeField] private Rocker SFXRocker;
    [SerializeField] private Rocker MusicRocker;                       
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}



[System.Serializable]
public class SettingsSaver
{

    public static string path = Path.Combine(Application.persistentDataPath, "crazySettings.json");
    public float MasterVolume;
    public float SFXVolume;
    public float MusicVolume;

    public SettingsSaver(float masterVolume, float sfxVolume, float musicVolume)
    {
        MasterVolume = masterVolume;
        SFXVolume = sfxVolume;
        MusicVolume = musicVolume;
    }

    public void SaveData()
    {
        System.String json = JsonUtility.ToJson(this,true);
        File.WriteAllText(path, json);
    }

    public void LoadData()
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            var data = JsonUtility.FromJson<SettingsSaver>(json);
            this.MasterVolume  = data.MasterVolume;
            this.SFXVolume  = data.SFXVolume;
            this.MusicVolume  = data.MusicVolume;
        }
    }
}
