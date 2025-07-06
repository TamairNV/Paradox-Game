using UnityEngine;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using static System.Random;
using Random = System.Random;

public class Quotes : MonoBehaviour
{

    private System.Random ran;
    [SerializeField] private TMP_Text quoteText;
    private float quoteTimer = 0;
    public List<string> quotes = new List<string>
    {
        "\"People who can't new-think are thralled by old-think.\"",
        "\"There are no closed doors, only ones you haven't found the key for yet. And sometimes, the key is a paradox.\"",
        "\"You have to start looking at the world in a new way.\"",
        "\"This is a one-way system. Cause comes before effect.\"",
        "\"Don't try to understand it. Feel it.\"",
        "\"What's happened, happened. It's an expression of faith in the mechanics of the world.\"",
        "\"They are playing chess. We are playing the board, the pieces, and the time between moves.\"",
        "\"The lock isn't the problem, it's the key. You're trying to use it on the wrong 'when'.\"",
        "\"Some puzzles aren't meant to be solved, they're meant to be undone.\"",
        "\"The world doesn't change. Only the direction you're viewing it from.\"",
        "\"Every effect casts a shadow back in time. We call that shadow 'cause'.\"",
        "\"Your memory is a map of where you've been. Your instinct is a compass for where you must go.\"",
        "\"Stop trying to out-think it. Just act, and let time catch up to you.\""
    };
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ran = new Random();
        GenerateNewQuote();
    }

    // Update is called once per frame
    void Update()
    {
        quoteTimer += Time.deltaTime;

        if (quoteTimer > 125)
        {
            quoteTimer = 0;
            GenerateNewQuote();
        }
    }

    public void GenerateNewQuote()
    {
        quoteTimer = 0;
        quoteText.text = quotes[ran.Next(0, quotes.Count - 1)];
    }
}
