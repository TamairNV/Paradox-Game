using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    
    private TMP_Text text;
    private Player player;
    [SerializeField] private Animator arrow;
    private int currentDirection = 1;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Time.timeScale = 1f;
        text = GetComponent<TMP_Text>();
        player = GameObject.Find("player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    
    {
        text.text = Math.Round(player.time, 1).ToString("0.0");
        if (player.Book.bookOpen)
        {
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

        if (player.timeEngine.direction != currentDirection)
        {
            if (player.timeEngine.direction == 1)
            {
                arrow.Play("SpinToLeft");
            }
            else
            {
                arrow.Play("SpinToRight");
            }
        }

        currentDirection = player.timeEngine.direction;


    }
}
