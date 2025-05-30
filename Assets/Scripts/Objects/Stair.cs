

using System.Collections.Generic;

using UnityEngine;


public class Stair : MonoBehaviour
{
    [SerializeField] private float inclineUpwardForce = 0.5f; // How much upward push is applied
    [SerializeField] private float speedReductionFactor = 0.7f; // Reduces speed (e.g., 0.7 = 70% speed)

    public static List<Stair> ActiveStairs = new List<Stair>();

    private bool isPlayerOnStair = false;

    public static bool IsPlayerOnAnyStair()
    {
        foreach (var stair in ActiveStairs)
        {
            if (stair.isPlayerOnStair) return true;
        }
        return false;
    }

    public static float GetSpeedModifier()
    {
        return IsPlayerOnAnyStair() ? ActiveStairs[0].speedReductionFactor : 1f;
    }

    public static float GetInclineForce()
    {
        return IsPlayerOnAnyStair() ? ActiveStairs[0].inclineUpwardForce : 0f;
    }

    private void OnEnable() => ActiveStairs.Add(this);
    private void OnDisable() => ActiveStairs.Remove(this);

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 13) // Assuming layer 13 is the player
            isPlayerOnStair = true;
    }

    private void OnTriggerExit2D( Collider2D other)
    {
        if (other.gameObject.layer == 13)
            isPlayerOnStair = false;
    }
}