using System.Collections.Generic;
using UnityEngine;
using System;
using System.Drawing;
using System.Linq;
using Color = UnityEngine.Color;

public class PressurePlate : Activator
{
    

    [SerializeField] private Sprite up;
    [SerializeField] private Sprite down;
    private SpriteRenderer sr;
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

   
    void Update()
    {
        if (isPressed)
        {
            sr.sprite = down;
        }
        else
        {
            sr.sprite = up;
        }
    }
    
    
}