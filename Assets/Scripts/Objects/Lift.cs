using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;
public class Lift : MonoBehaviour
{
    public float getInLiftSpeed = 2;
    public float liftSpeed = 5;
    
    [SerializeField] public Lift Exit;
    [SerializeField] public bool goingDown = true;
    private Vector3 startingPosition;
    private Player player;
    private BoxCollider2D boxCollider;
    private CapsuleCollider2D playerCollider;
    [SerializeField] public int MinLevel = 0;

    public bool playerOver = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("player").GetComponent<Player>();
        startingPosition = transform.GetChild(0).position;
        playerCollider = player.GetComponent<CapsuleCollider2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (boxCollider.IsTouching(playerCollider))
        {
            playerOver = true;
        }
        else
        {
            playerOver = false;
        }

        if (playerOver && Keyboard.current.eKey.wasPressedThisFrame && player.lastLevelCompleted >= MinLevel)
        {

            StartCoroutine(EnterLift());
            
        }
    }

    public IEnumerator EnterLift()
    {
        Transform lift = transform.GetChild(0);
        float  timer = 0;
        player.allowedToWalk = false;
        player.collider.enabled = false;
        while (Vector3.Distance(player.transform.position, lift.GetChild(1).position) > 0.01f && timer < 1f)
        {
            player.transform.position =
                Vector3.Lerp(player.transform.position, lift.position, getInLiftSpeed * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        player.transform.position = lift.GetChild(1).position;

        player.direction = new Vector3(0, -1, 0);
        player.lastDirection = new Vector3(0, -1, 0);
        player.GetComponent<Player_Controller>().AnimateMovement();
        Transform liftDoors = lift.GetChild(0);
        liftDoors.GetComponent<Animator>().Play("doorClose");
        player.GetComponent<SpriteRenderer>().sortingLayerName = "lift_person";
        player.shadow.SetActive(false);
        
        yield return new WaitForSeconds(1.2f);
        



        
       
        timer = 0;
        CameraController cam = Camera.main.transform.GetComponent<CameraController>();
        cam.enabled = false;
        while (timer < 0.5f)
        {
            timer += Time.deltaTime;
            Vector3 movement =  Vector3.down * liftSpeed * Time.deltaTime;
            if (!goingDown)
            {
                movement =  Vector3.up * liftSpeed * Time.deltaTime;
            }
            
            player.transform.position += movement;
            lift.transform.position += movement;
            yield return null;
        }
        yield return StartCoroutine(player.RunCircleWipe());

        
        lift.transform.position = startingPosition;
        player.transform.position = Exit.transform.GetChild(0).GetChild(1).transform.position;
        cam.transform.position = new Vector3(player.transform.position.x, player.transform.position.y,
            cam.transform.position.z);
        
        yield return new WaitForSeconds(0.6f);
        
        yield return StartCoroutine(player.ReverseCircleWipe());
        Exit.transform.GetChild(0).GetChild(0).GetComponent<Animator>().Play("doorOpen");
        yield return new WaitForSeconds(0.8f);
        player.GetComponent<SpriteRenderer>().sortingLayerName = "Player";
        player.allowedToWalk = true;
        player.collider.enabled = true;
        player.shadow.SetActive(true);
        cam.enabled = true;
        
    }
}
