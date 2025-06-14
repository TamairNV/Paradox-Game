using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private Animator ani;

    [SerializeField] private float countDownTime = 3f;

    private Player player;

    private int direction;

    private string currentAnimation = "";
    
    
    private Dictionary<int,string> explosionList = new Dictionary<int, string>();
    private bool isVisable = true;
    private SpriteRenderer sr;
    
    public static List<Bomb> Bombs = new List<Bomb>();

    private bool isExploding = false;

    [SerializeField] private float animationLength = 0.8f;



    public static void DestroyALlBombs()
    {
        foreach (var bomb in Bombs)
        {
            Destroy(bomb);
        }
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Bombs.Add(this);
        player = GameObject.Find("player").GetComponent<Player>();
        ani = transform.GetChild(0).GetComponent<Animator>();
        direction = player.timeEngine.direction;
        sr = transform.GetComponent<SpriteRenderer>();

        if (direction == 1)
        {
            explosionList.Add(player.timeEngine.CurrentTime,"explosion_start");
        }
        else
        {
            explosionList.Add(player.timeEngine.CurrentTime,"inverse_explosion_start");
        }
        StartCoroutine( TriggerExplosion());
  
    }

    // Update is called once per frame
    void Update()
    {
        direction = player.timeEngine.direction;
        if (explosionList.ContainsKey(player.timeEngine.CurrentTime ))
        {
            if (explosionList[player.timeEngine.CurrentTime] == "explosion_start" && direction == 1)
            {
                StartCoroutine(TriggerExplosion());
            }
            if(explosionList[player.timeEngine.CurrentTime] == "inverse_explosion_start" && direction ==0)
            {
                StartCoroutine(TriggerExplosion());
            }
            if (explosionList[player.timeEngine.CurrentTime] == "reverse_explosion" && direction == 0)
            {
                StartCoroutine(TriggerReverseExplosion());
            }
            if(explosionList[player.timeEngine.CurrentTime] == "reverse_inverse_explosion" && direction == 1)
            {
                StartCoroutine(TriggerReverseExplosion());
            }
        }


        
        
    }



    private IEnumerator TriggerExplosion()
    {
        sr.enabled = true;
        isExploding = true;
        bool cancelCountDown = false;
        int startDirection = direction;
        float timer = 0;
        while (timer <= countDownTime)
        {
            yield return null;
            if (direction != startDirection)
            {
                cancelCountDown = true;
                break;
            }
            timer += Time.deltaTime;
            
        }
        
        if (!cancelCountDown)
        {
            
            sr.enabled = false;
            if (direction == 1)
            {
                changeAnimation("explosion");
            }
            else
            {
                changeAnimation("explosion_inverse");
            }
    
            bool cancelExplosion= false;
            float exTimer = 0;
            while (exTimer <= animationLength)
            {
                yield return null;
                if (direction != startDirection)
                {
                    cancelExplosion = true;
                    break;
                }
                exTimer += Time.deltaTime;
            
            }

            if (!cancelExplosion)
            {
                            
                if (!explosionList.ContainsKey(player.timeEngine.CurrentTime))
                {
                    if (direction == 1)
                    {
                        explosionList.Add(player.timeEngine.CurrentTime, "reverse_explosion");
                    }
                    else
                    {
                        explosionList.Add(player.timeEngine.CurrentTime, "reverse_inverse_explosion");
                    }
                }
            }
            else
            {
                if (direction == 0)
                {
                    changeAnimation("explosion_reverse");
                }
                else
                {
                    changeAnimation("explosion_inverse_reverse");
                }
                
                while (exTimer > 0)
                {
                    exTimer -= Time.deltaTime;
                    yield return null;
                }
                sr.enabled = true;
                yield return new WaitForSeconds(countDownTime);
                sr.enabled = false;
                isExploding = false;
            }
            
        }
        else
        {
            while (timer > 0)
            {
                timer -= Time.deltaTime;
                yield return null;
            }
            sr.enabled = false;
        }

        isExploding = false;
    }

    private IEnumerator TriggerReverseExplosion()
    {
        isExploding = true;
        sr.enabled = false;
        if (direction == 0)
        {
            changeAnimation("explosion_reverse");
        }
        else
        {
            changeAnimation("explosion_inverse_reverse");
        }
        yield return new WaitForSeconds(animationLength);
        sr.enabled = true;
        yield return new WaitForSeconds(countDownTime);
        sr.enabled = false;
        isExploding = false;
    }
    


    private void changeAnimation(string animation)
    {
        if (currentAnimation != animation)
        {
            ani.Play(animation);
        }
    }
}
