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

    public static Dictionary<int, LevelPorter> levelPorters = new Dictionary<int,LevelPorter>();
    private SceneLoader sceneLoader;
    [SerializeField] public int LevelNumber;
    [SerializeField] public GameObject Blueprint;
    [SerializeField] public GameObject FinishItem;
    [SerializeField] public float TargetEntropy = 0.5f;
    private bool hasCollectedBlueprint = false;
    private bool completed = false;
    private Player player;
    

    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (LevelNumber > 0)
        {
            levelPorters[LevelNumber] = this;
        }
        
        player = GameObject.Find("player").GetComponent<Player>();
        sceneLoader = GameObject.Find("_SceneLoader").GetComponent<SceneLoader>();
        if (Blueprint != null)
        {
            Blueprint.SetActive(false);
        }
        
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
        yield return StartCoroutine(player.RunCircleWipe());
        StartCoroutine(player.Book.GetComponent<Book>().openBook("levelStart"));


        
        player.transform.position = Level.CurrentLevel.startLocation.position;
        Level.CurrentLevel.hasCollectedBlueprint = hasCollectedBlueprint;
        
        Level.CurrentLevel.FinishItemSprite = FinishItem.GetComponent<SpriteRenderer>().sprite;
        if (Blueprint != null)
        {
            Level.CurrentLevel.BlueprintSprite = Blueprint.GetComponent<SpriteRenderer>().sprite;
            player.Book.GetComponent<Book>().BlueprintImage.sprite= Blueprint.GetComponent<SpriteRenderer>().sprite;
        }
        else
        {
            player.Book.GetComponent<Book>().BlueprintImage.sprite = null;
        }

        double num = Math.Ceiling((TargetEntropy / player.MaxEntropy)*10);
        player.Book.TargetEntropy.text = (num/10f).ToString() ;
        player.Book.GetComponent<Book>().CollecableImage.sprite = FinishItem.GetComponent<SpriteRenderer>().sprite;
        yield return new WaitForSeconds(0.75f);
        yield return StartCoroutine(player.ReverseCircleWipe());
        player.resetGame();
        player.allowedToWalk = true;
        player.time = 0;
        
        
    }   
}
