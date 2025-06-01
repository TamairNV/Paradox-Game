using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

public class MomentData
{
    public Vector2 Direction;
    public bool isMoving;
    public Vector3 Position;
    public bool Jumped = false;

    public MomentData(Vector2 direction, bool isMoving, Vector3 position)
    {
        Direction = direction;
        this.isMoving = isMoving;
        Position = position;
        


    }
    
}


