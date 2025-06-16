using System;
using UnityEngine;

public class sideFan : MonoBehaviour
{
    private Transform player;
    [SerializeField] private float speed = 10;
    [SerializeField] private Vector3 direction = new Vector3(1, 0,0);
    [SerializeField] private int startDirection = 1;
    private bool isOverFan;
    private Player playerController;
    [SerializeField] private float particleWidth = 1.5f;
    private Vector3 startLocation;
    [SerializeField] private ParticleSystem particleSystem;
    private Vector3 startScale;

    [SerializeField] private Material rightArrow;

    [SerializeField] private Material leftArrow;

 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startLocation = particleSystem.transform.position;
        startScale = particleSystem.transform.localScale;
        player = GameObject.Find("player").transform;

        playerController = player.GetComponent<Player>();
        
    }

    // Update is called once per frame
    void Update()
    {
        int currentDirection = playerController.timeEngine.direction;
        bool flowDirection = currentDirection - startDirection == 0;
        if (isOverFan && flowDirection)
        {
            player.position += direction * speed * Time.deltaTime;
        }
        if (isOverFan && !flowDirection)
        {
            player.position -= direction * speed/1.25f * Time.deltaTime;
        }
        
        if (flowDirection)
        {
            particleSystem.transform.position = startLocation;
            particleSystem.transform.localScale = new Vector3(startScale.x, startScale.y, startScale.z);
            particleSystem.GetComponent<Renderer>().material = leftArrow;
        }
        else
        {
            particleSystem.transform.position = startLocation + new Vector3(particleWidth, 0, 0);
            particleSystem.transform.localScale = new Vector3(startScale.x, -startScale.y, startScale.z);
            particleSystem.GetComponent<Renderer>().material = rightArrow;
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
