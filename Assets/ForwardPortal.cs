using System;
using UnityEngine;

public class ForwardPortal : MonoBehaviour
{
    [SerializeField] private Player_Controller player;

    public bool isSingleUse = false;

    private bool isUsed = false;

    public float cooldown = 5f;

    private float timer = 0;

    private bool isAllowed = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        timer = cooldown;
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
        if (other.gameObject.layer == 13 && isAllowed && (!isAllowed || !isSingleUse))
        {
            if (player.timeEngine.direction == 1)
            {
                DimentionalPlayer dimPlayer = player.ReverseDirection();
                dimPlayer.transform.position = player.direction * 15;
            }
            else
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
