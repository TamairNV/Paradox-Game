using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FinishPorter : MonoBehaviour
{


    [SerializeField] private int level = 0;
    private bool reseting = false;
    private Player player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("player").GetComponent<Player>();
        StartCoroutine(loadSprite());
    }

    private IEnumerator loadSprite()
    {
        while (Level.CurrentLevel == null ||  Level.CurrentLevel.FinishItemSprite == null)
        {
            yield return null;
        }
        GetComponent<SpriteRenderer>().sprite = Level.CurrentLevel.FinishItemSprite;
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 13 && !reseting)
        {
            if (player.lastLevelCompleted < level)
            {
                player.lastLevelCompleted = level;
                
            }

            Level.CurrentLevel.completed = true;
            
            Animator ani = GetComponent<Animator>();
            if (ani != null)
            {
                ani.enabled = true;
            }

            new LevelSaveData(Level.CurrentLevel);
            LevelSaveData.SaveAllData();
            LevelPorter.ReadSaveData();

            StartCoroutine(sendPlayer());

        }
    }

    private IEnumerator sendPlayer()
    {
        reseting = true;
        yield return StartCoroutine(player.RunCircleWipe());
        

        
        player.resetPlayer();
        player.transform.position = player.StartPosition;
        Camera.main.transform.position = new Vector3(player.transform.position.x,player.transform.position.y,Camera.main.transform.position.z);
        yield return new WaitForSeconds(0.75f);
        Level.CurrentLevel = null;
        player.resetGame();
        player.time = 0;
        yield return StartCoroutine(player.ReverseCircleWipe());
        reseting = false;


    }
}
