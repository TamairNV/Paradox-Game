using System;
using UnityEngine;

public class LavaTile : MonoBehaviour
{
    [SerializeField] private Sprite lava;
    [SerializeField] private Sprite ice;
    public bool isLava = true;
    private SpriteRenderer sr;
    public static float LavaTileIncrease = 10;
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {;
        sr = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == 18)
        {
            
            Bomb bomb = other.gameObject.GetComponentInParent<Bomb>();
            
            if (bomb.isHot)
            {
                sr.sprite = lava;
                isLava = true;
            }
            else
            {
                sr.sprite = ice;
                isLava = false;
            }
        }

        if (other.gameObject.layer == 13 && isLava)
        {
            
            Player player = other.gameObject.GetComponent<Player>();
           
            if (player.FeetCollider.IsTouching(GetComponent<Collider2D>()))
            {
                StartCoroutine(player.CauseParadox());
                player.QuoteMaker.DisplayMessage("You Stepped into Lava");
            }
            

        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 18)
        {
            Bomb bomb = other.gameObject.GetComponentInParent<Bomb>();
            if (bomb.isHot)
            {
                sr.sprite = lava;
                isLava = true;
            }
            else
            {
                sr.sprite = ice;
                isLava = false;
            }
        }
    }
}
