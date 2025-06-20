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

            StartCoroutine(sendPlayer());

        }
    }

    private IEnumerator sendPlayer()
    {
        Vignette vignette= null;
        player.volume.profile.TryGet(out vignette);
        float timer = 0;
        while (timer < 1)
        {

            float t = timer / 1f;

            vignette.intensity.Override(t*10f);

            
            timer += Time.deltaTime;
            yield return new WaitForSeconds(Time.deltaTime);
        }

        player.transform.position = player.StartPosition;
        player.resetPlayer();
        timer = 0;
        while (timer < 1)
        {

            float t = timer / 1f;
            timer += Time.deltaTime;
            
            vignette.intensity.Override((1-t)*10f);
            
            yield return new WaitForSeconds(Time.deltaTime);
        }
        player.resetGame();
        player.time = 0;
        
        
    }
}
