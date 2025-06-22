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

    private Player player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("player").GetComponent<Player>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 13)
        {
            if (player.lastLevelCompleted < level)
            {
                player.lastLevelCompleted = level;
                
            }
            Animator ani = GetComponent<Animator>();
            if (ani != null)
            {
                ani.enabled = true;
            }
            

            StartCoroutine(sendPlayer());

        }
    }

    private IEnumerator sendPlayer()
    {
        yield return new WaitForSeconds(0.4f);
        yield return StartCoroutine(player.RunCircleWipe());
        

        player.transform.position = player.StartPosition;
        player.resetPlayer();
        yield return new WaitForSeconds(0.75f);
        yield return StartCoroutine(player.ReverseCircleWipe());
        player.resetGame();
        player.time = 0;
        
        
    }
}
