using System;
using UnityEngine;

public class DoorButton : MonoBehaviour
{
    public bool isPressed = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == 12 || other.gameObject.layer == 13)
        {
            isPressed = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 12|| other.gameObject.layer == 13)
        {
            isPressed = false;
        }
    }
}
