using System;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [SerializeField] private Player_Controller player;

    public bool isSingleUse = false;

    private bool isUsed = false;

    public float cooldown = 5f;

    private float timer = 0;

    private bool isAllowed = true;

    public int DirectionToGo = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = cooldown;
        player = GameObject.Find("player").transform.GetComponent<Player_Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer > cooldown)
        {
            isAllowed = true;
        }
        else
        {
            isAllowed = false;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 13 &&  isAllowed && !(isSingleUse && isUsed))
        {
            if (player.timeEngine.direction == 1 && DirectionToGo == 0)
            {
                DimentionalPlayer dimPlayer = player.ReverseDirection();
                dimPlayer.transform.position = player.direction * 15;
            }
            if(player.timeEngine.direction == 0 && DirectionToGo == 1)
            {
                DimentionalPlayer dimPlayer = player.ReverseDirection();
                dimPlayer.transform.position -= player.direction * 15;
            }

            StartCoroutine(player.BecomeImmune(2));
            isUsed = true;
            timer = 0;
        }
    }
}
