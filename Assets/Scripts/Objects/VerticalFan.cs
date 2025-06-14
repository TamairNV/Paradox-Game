using System;
using Unity.Mathematics;
using UnityEngine;

public class VerticalFan : MonoBehaviour
{

    [SerializeField] private int startDirection = 1;

    private Player player;

    private bool isOn;

    private bool isOverFan;

    private Collider2D collider;

    [SerializeField] private float liftSpeed = 10;
    [SerializeField] private float distanceDropOff = 2f;

    [SerializeField] private float particleHeight = 0.5f;
    [SerializeField] private ParticleSystem particleSystem;
    private Vector3 startLocation;
    private Vector3 startScale;
    [SerializeField] private Transform jumpPoint;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startLocation = particleSystem.transform.position;
        startScale = particleSystem.transform.localScale;
        player = GameObject.Find("player").GetComponent<Player>();
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
            player.transform.position += -Vector3.down * liftSpeed * Time.deltaTime*(distance/distanceDropOff);
            collider.enabled = false;
        }
        else
        {
            collider.enabled = true;
        }

        if (flowDirection)
        {
            particleSystem.transform.position = startLocation;
            particleSystem.transform.localScale = new Vector3(startScale.x, startScale.y, startScale.z);
            if(jumpPoint != null){jumpPoint.gameObject.SetActive(false);}
        }
        else
        {
            particleSystem.transform.position = startLocation + new Vector3(0, particleHeight, 0);
            particleSystem.transform.localScale = new Vector3(startScale.x, -startScale.y, startScale.z);
            if(jumpPoint != null){jumpPoint.gameObject.SetActive(true);}
            
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
