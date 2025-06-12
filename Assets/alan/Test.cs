using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test : MonoBehaviour
{

    [SerializeField] public float speed = 10;
    
    
    private float afloat = 2;
    private Vector3 playerPosition;
 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.dKey.isPressed)
        {
            transform.position = new Vector3(transform.position.x + speed * Time.deltaTime, transform.position.y, 0);
        }
        if (Keyboard.current.aKey.isPressed)
        {
            transform.position = new Vector3(transform.position.x - speed * Time.deltaTime, transform.position.y, 0);
        }
        print("Hello");

    }
    

}
