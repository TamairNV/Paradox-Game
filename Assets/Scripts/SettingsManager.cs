
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TMPro;
using UnityEngine;


using System.IO;


public class SettingsManager : MonoBehaviour
{
    public float MasterVolume;
    public float SFXVolume;
    public float MusicVolume;
    [SerializeField] public MusicManager musicManager;
    [SerializeField] public Rocker MasterRocker;
    [SerializeField] public Rocker SFXRocker;
    [SerializeField] public Rocker MusicRocker;
    private SettingsSaver saver;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        
        saver = new SettingsSaver();
        if (!saver.LoadData(this))
        {
            musicManager.updateMasterVolume(MasterRocker.Value);
            musicManager.updateMusicVolume(SFXRocker.Value);
            musicManager.updateSoundEffectVolume(MusicRocker.Value);
            saver.SaveData(MasterRocker.Value,SFXRocker.Value,MusicRocker.Value,false);
        }
        else
        {
            if (!saver.doneTutorial)
            {
                StartCoroutine(runTutorial());
            }
        }

        
    }

    IEnumerator runTutorial()
    {
        yield return null;
        Player player = GameObject.Find("player").GetComponent<Player>();
        StartCoroutine(player.SendToTutorial());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        saver.SaveData(MasterRocker.Value,SFXRocker.Value,MusicRocker.Value,false);
    }
}



[System.Serializable]
public class SettingsSaver
{

    
    public float MasterVolume;
    public float SFXVolume;
    public float MusicVolume;
    public bool doneTutorial;


    public void SaveData(float masterVolume, float sfxVolume, float musicVolume,bool doneTutorial)
    {
        this.doneTutorial = doneTutorial;
        MasterVolume = masterVolume;
        SFXVolume = sfxVolume;
        MusicVolume = musicVolume;
        System.String json = JsonUtility.ToJson(this,true);
        string path = Path.Combine(Application.persistentDataPath, "crazySettings.json");
        File.WriteAllText(path, json);
    }

    public bool LoadData(SettingsManager settings)
    {
        string path = Path.Combine(Application.persistentDataPath, "crazySettings.json");
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            var data = JsonUtility.FromJson<SettingsSaver>(json);
            this.MasterVolume  = data.MasterVolume;
            this.SFXVolume  = data.SFXVolume;
            this.MusicVolume  = data.MusicVolume;
            settings.musicManager.updateMasterVolume(MasterVolume);
            settings.musicManager.updateMusicVolume(MusicVolume);
            settings.musicManager.updateSoundEffectVolume(SFXVolume);
            settings.MasterRocker.Value = MasterVolume;
            settings.MusicRocker.Value = MusicVolume;
            settings.SFXRocker.Value = SFXVolume;
            return true;
        }

        return false;
    }
}
