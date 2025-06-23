using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class LevelPorter : MonoBehaviour
{

    private static Dictionary<int, LevelPorter> levelPorters = new Dictionary<int,LevelPorter>();
    private SceneLoader sceneLoader;
    [SerializeField] public int LevelNumber;
    [SerializeField] private GameObject Blueprint;
    [SerializeField] private GameObject FinishItem;
    private bool hasCollectedBlueprint = false;
    private bool completed = false;
    private Player player;

    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        levelPorters[LevelNumber] = this;
        player = GameObject.Find("player").GetComponent<Player>();
        sceneLoader = GameObject.Find("_SceneLoader").GetComponent<SceneLoader>();
        Blueprint.SetActive(false);
        FinishItem.SetActive(false);
        
    }

    public static void ReadSaveData()
    {
        Player player = GameObject.Find("player").GetComponent<Player>();
        foreach (var data in LevelSaveData.SaveData)
        {
            LevelPorter lp = levelPorters[data.levelNumber];
            lp.hasCollectedBlueprint = data.hasCollectedBlueprint;
            lp.completed = data.completed;
            if (data.completed && data.levelNumber > player.lastLevelCompleted)
            {
                player.lastLevelCompleted = data.levelNumber;
            }
            if (lp.completed)
            {
                lp.FinishItem.SetActive(true);
            }
            else
            {
                lp.FinishItem.SetActive(false);
            }
            if ( lp.hasCollectedBlueprint)
            {
                lp.Blueprint.SetActive(true);
            }
            else
            {
                lp.Blueprint.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 13)
        {

            StartCoroutine(SendPlayerToLevel());
        }
    }


    public IEnumerator SendPlayerToLevel()
    {
        
        
        Level.CurrentLevel = null;
        sceneLoader.LoadLevel("Level_" + LevelNumber);


        player.allowedToWalk = false;
        float duration = 2f;
        float elapsed = 0f;
        yield return StartCoroutine(player.RunCircleWipe());



        
        player.transform.position = Level.Levels[LevelNumber].startLocation.position;
        Level.CurrentLevel = Level.Levels[LevelNumber];
        Level.CurrentLevel.hasCollectedBlueprint = hasCollectedBlueprint;
        Level.CurrentLevel.FinishItemSprite = FinishItem.GetComponent<SpriteRenderer>().sprite;
        Level.CurrentLevel.BlueprintSprite = Blueprint.GetComponent<SpriteRenderer>().sprite;
        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(player.ReverseCircleWipe());
        player.resetGame();
        player.allowedToWalk = true;
        player.time = 0;
        StartCoroutine(player.GetComponent<Player_Controller>().openBook());
        
    }   
}
