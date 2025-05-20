using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

public class Turnstile : MonoBehaviour
{
    private bool working = false;

    [SerializeField] private Transform input;

    [SerializeField] private Transform output;

    private Vector3 goToPosition;

    private Player_Controller player;

    private bool inReverser = false;
    float startTime;
    float totalTime = 2.5f;
    private string currentAnimation = "";
    private Animator ani;
    private LinkedList<DimensionalNode> lastLink;

    private List<int> animationTimes = new List<int>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ani =transform.GetChild(0).GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (overTernstile)
        {
            checkForInteraction();
        }
        if (player !=null && animationTimes.Contains(player.timeEngine.CurrentTime))
        {
            if (player.timeEngine.direction == 1 || true)
            {
                changeAnimation("door");
            }
   
        }
        
        
        if (player != null)
        {
            lastLink = player.timeEngine.CurrentDimensionalNode.SameTimeNodes;
        }
        
        if (working && !inReverser)
        {
            float timeElapsed = Time.time - startTime;
            float t = Mathf.SmoothStep(0, 1, timeElapsed / totalTime);
            player.transform.position = Vector3.Lerp(player.transform.position, goToPosition, t);
        }

        if (working &&  Vector3.Distance(player.transform.position, goToPosition) < 0.01f)
        {
            inReverser = true;
        }
        
    }

    private bool overTernstile = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        GameObject obj = other.gameObject;
        if (obj.layer == 13)
        {
            overTernstile = true;
            player = obj.GetComponent<Player_Controller>();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        GameObject obj = other.gameObject;
        if (obj.layer == 13)
        {
            overTernstile = false;
        }
    }

    private void checkForInteraction()
    {

            print("player enter");
            if (!working && Keyboard.current.eKey.isPressed)
            {
                startTime = Time.time;
                working = true; 
                player.allowedToWalk = false;
                player.isMoving = true;
                if (player.timeEngine.direction == 1)
                {
                    goToPosition = input.position;
                }
                else
                {
                    goToPosition = output.position;
                }
                player.lastDirection = (player.transform.position - goToPosition ).normalized;
                player.direction =  (player.transform.position - goToPosition ).normalized;
                StartCoroutine(reverse());

            }
            
        
     
    }

    IEnumerator reverse()
    {
        
        yield return new WaitForSeconds(1f);
        player.lastDirection = new Vector2(0, -1);
        player.isMoving = false;
        yield return new WaitForSeconds(0.2f);
        changeAnimation("door");
        animationTimes.Add(player.timeEngine.CurrentTime);
        if (player.timeEngine.direction == 1)
        {
            animationTimes.Add(player.timeEngine.CurrentTime+ (int)player.MomentRate * 2); 
        }
        else
        {
            animationTimes.Add(player.timeEngine.CurrentTime- (int)player.MomentRate * 2);
        }
        
        yield return new WaitForSeconds(1f);
        DimentionalPlayer newPlayer =  player.ReverseDirection();
        inReverser = true;
        if (player.timeEngine.direction == 0)
        {
            player.transform.position = output.position;
            newPlayer.transform.position = input.position;
        }
        else
        {
            player.transform.position = input.position;
            newPlayer.transform.position = output.position;
        }
        yield return new WaitForSeconds(1f);
        player.allowedToWalk = true;
        
        yield return new WaitForSeconds(3f);
        working = false;
        inReverser = false;
    }
    
    private void changeAnimation(string animation)
    {
        if (!currentAnimation.Equals(animation)  || IsAnimationFinished(currentAnimation))
        {
            ani.Play(animation);
        }
    }
    
    private bool IsAnimationFinished(string animationName)
    {
        if (ani.GetCurrentAnimatorStateInfo(0).IsName(animationName))
        {
            float normalizedTime = ani.GetCurrentAnimatorStateInfo(0).normalizedTime;
            return normalizedTime >= 1f; // True if animation reached the end
        }
        return false; // Animation isn't playing
    }
}
