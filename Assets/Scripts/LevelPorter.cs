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
    private SceneLoader sceneLoader;
    [SerializeField] public int LevelNumber;
    
    private Player player;

    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("player").GetComponent<Player>();
        sceneLoader = GameObject.Find("_SceneLoader").GetComponent<SceneLoader>();
        
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
        
        sceneLoader.LoadLevel("Level_" + LevelNumber);


        player.allowedToWalk = false;
        float duration = 2f;
        float elapsed = 0f;
        yield return StartCoroutine(player.RunCircleWipe());



        player.resetGame();
        
        player.transform.position = Level.Levels[LevelNumber].startLocation.position;
        Level.CurrentLevel = Level.Levels[LevelNumber];

        yield return new WaitForSeconds(0.5f);
        yield return StartCoroutine(player.ReverseCircleWipe());
        player.allowedToWalk = true;
        player.time = 0;
        yield return new WaitForSeconds(2f);
    }   
}
