using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    
    private Text text;
    private Player player;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1f;
        text = GetComponent<Text>();
        player = GameObject.Find("player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {       if (player.Book.activeSelf)
        {
            text.text = "0:00";
            return;
        }
        if (player.timeEngine.direction == 1)
        {
            player.time += Time.deltaTime;
        }
        else
        {
            player.time -= Time.deltaTime;
        }

        if (player.time <= 0)
        {
            player.time = 0;
        }
        
        text.text = Math.Round(player.time, 1).ToString("0.0");
    }
}
