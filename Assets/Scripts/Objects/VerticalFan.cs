using System;
using Unity.Mathematics;
using UnityEngine;

public class VerticalFan : MonoBehaviour
{

    [SerializeField] private int startDirection = 1;

    private Player_Controller player;

    private bool isOn;

    private bool isOverFan;

    private Collider2D collider;

    [SerializeField] private float liftSpeed = 10;
    [SerializeField] private float distanceDropOff = 2f;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("player").GetComponent<Player_Controller>();
        collider = GetComponent<PolygonCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        int currentDirection = player.timeEngine.direction;
        bool flowDirection = currentDirection - startDirection == 0;
        if (isOverFan && flowDirection)
        {
            float distance = 1-Vector3.Distance(player.transform.position, transform.position);
            print(distance);
            player.transform.position += -Vector3.down * liftSpeed * Time.deltaTime*(distance/distanceDropOff);
            collider.enabled = false;
        }
        else
        {
            collider.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 13)
        {
            isOverFan = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 13)
        {
            isOverFan = false;
        }
    }
}
