using System;
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
    public bool isExploding = false;
    
    private Dictionary<int,string> explosionList = new Dictionary<int, string>();
    private SpriteRenderer sr;
    
    public static List<Bomb> Bombs = new List<Bomb>();

   

    [SerializeField] private float animationLength = 0.8f;
    public bool isHot = true;

    private CircleCollider2D collider;



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
            isHot = true;
        }
        else
        {
            explosionList.Add(player.timeEngine.CurrentTime,"inverse_explosion_start");
            isHot = false;
        }

        collider = transform.GetChild(0).GetComponent<CircleCollider2D>();
        StartCoroutine(TriggerExplosion());
        currentMoment = player.timeEngine.CurrentTime;
    }

    private int currentMoment;
    // Update is called once per frame
    void Update()
    {


        if (currentMoment == player.timeEngine.CurrentTime)
        {
            return;
        }
        direction = player.timeEngine.direction;
        if (explosionList.ContainsKey(player.timeEngine.CurrentTime ))
        {
            if (explosionList[player.timeEngine.CurrentTime] == "explosion_start" && direction == 1)
            {
                isHot = true;
                StartCoroutine(TriggerExplosion());
            }
            else if(explosionList[player.timeEngine.CurrentTime] == "inverse_explosion_start" && direction ==0)
            {
                isHot = false;
                StartCoroutine(TriggerExplosion());
            }
            else if (explosionList[player.timeEngine.CurrentTime] == "reverse_explosion" && direction == 0)
            {
                isHot = false;
                StartCoroutine(TriggerReverseExplosion());
            }
            else if(explosionList[player.timeEngine.CurrentTime] == "reverse_inverse_explosion" && direction == 1)
            {
                isHot = true;
                StartCoroutine(TriggerReverseExplosion());
            }
        }

        currentMoment = player.timeEngine.CurrentTime;




    }



    private IEnumerator TriggerExplosion()
    {
        sr.enabled = true;
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
            isExploding = true;
            sr.enabled = false;
            if (direction == 1)
            {
                changeAnimation("explosion");
            }
            else
            {
                changeAnimation("explosion_inverse");
            }

            List<DimentionalObjects> hitBoxes = new List<DimentionalObjects>();
            bool cancelExplosion= false;
            float exTimer = 0;
            while (exTimer <= animationLength)
            {
                yield return null;
                if (direction != startDirection)
                {
                    isExploding = false;
                    cancelExplosion = true;
                    break;
                }
                exTimer += Time.deltaTime;
                
                Collider2D[] boxes = Physics2D.OverlapCircleAll(transform.position, collider.radius*transform.GetChild(0).lossyScale.x,1 << 15);
                foreach (var box in boxes)
                {
                    DimentionalObjects b = box.transform.GetComponent<DimentionalObjects>();
                   
                    if (!hitBoxes.Contains(b))
                    {
                        hitBoxes.Add(b);
                        if (isHot)
                        {
                            b.Temp++;
                        }
                        else
                        {
                            b.Temp--;
                            
                        }
                        b.changeBoxType();
                        
                    }
                }

                if (exTimer > animationLength / 2)
                {
                    isExploding = false;
                }

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
              
                foreach (var box in hitBoxes)
                {
                    DimentionalObjects b = box.transform.GetComponent<DimentionalObjects>();
                    if (isHot)
                    {
                        b.Temp--;
                    }
                    else
                    {
                        b.Temp++;
                    }
                    b.changeBoxType();
                        
                    
                }

                isExploding = false;

                
                while (exTimer > 0)
                {
                    exTimer -= Time.deltaTime;
                    yield return null;
                }
                sr.enabled = true;
                yield return new WaitForSeconds(countDownTime);
                sr.enabled = false;
             
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
    }

    private IEnumerator TriggerReverseExplosion()
    {
        sr.enabled = false;
        if (direction == 0)
        {
            changeAnimation("explosion_reverse");
        }
        else
        {
            changeAnimation("explosion_inverse_reverse");
        }

        isExploding = false;
        List<DimentionalObjects> hitBoxes = new List<DimentionalObjects>();
        float exTimer = 0;
        while (exTimer <= animationLength)
        {
            yield return null;
            exTimer += Time.deltaTime;
                
            Collider2D[] boxes = Physics2D.OverlapCircleAll(transform.position, collider.radius*transform.GetChild(0).lossyScale.x,1 << 15);
            foreach (var box in boxes)
            {
                DimentionalObjects b = box.transform.GetComponent<DimentionalObjects>();
                
                if (!hitBoxes.Contains(b))
                {
                    
                    hitBoxes.Add(b);
                    if (isHot)
                    {
                        b.Temp++;
                    }
                    else
                    {
                        b.Temp--;
                    }
                    b.changeBoxType();
                        
                }
            }

            if (exTimer > animationLength / 2)
            {
                isExploding = true;
            }
            

        }
        
        isExploding = false;
        
        sr.enabled = true;
        yield return new WaitForSeconds(countDownTime);
        sr.enabled = false;
    }
    


    private void changeAnimation(string animation)
    {
        if (currentAnimation != animation)
        {
            ani.Play(animation);
        }
    }

    private void OnDrawGizmos()
    {
        if (collider != null)
        {
            Gizmos.DrawWireSphere(transform.position, collider.radius*transform.GetChild(0).lossyScale.x);
        }
        
    }
}
