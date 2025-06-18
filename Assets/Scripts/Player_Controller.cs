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

public class Player_Controller : MonoBehaviour
{

    [SerializeField] VariableJoystick variableJoystick;
    [SerializeField] private LayerMask layer;
    [SerializeField] private float distance = 2f;
    [SerializeField] private float sideRayDistance;
    [SerializeField] private Transform flameThrower;
    [SerializeField] public bool OnMobile = true;
    private Player player;
    private Animator ani;
    private string currentAnimation = "sdsf";
    private ParticleSystem fireParticleSystem;
    private float startingFireSpeed = 5f;
    private Dictionary<String, Vector2> directions = new Dictionary<String,Vector2>()
    {
        {"W", Vector2.up},
        {"S",Vector2.down},
            {"A",Vector2.left},
            {"D",Vector2.right},

    };


    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = transform.GetComponent<Player>();
        ani = GetComponent<Animator>();
        fireParticleSystem = flameThrower.GetChild(0).GetComponent<ParticleSystem>();
        startingFireSpeed = fireParticleSystem.main.startSpeed.constant;
        
    }
    
    

    // Update is called once per frame
    void Update()
    {
        //Debugging code 
        foreach (var direction in directions)
        {
            if (direction.Key == "A" || direction.Key == "D")
            {
                Debug.DrawLine(transform.position + Vector3.down * sideRayDistance,transform.position + Vector3.down * sideRayDistance + (Vector3)directions[direction.Key] * distance,Color.blue);
                Debug.DrawLine(transform.position + Vector3.up * sideRayDistance,transform.position + Vector3.up * sideRayDistance + (Vector3)directions[direction.Key] * distance,Color.blue);
            }
            else
            {
                Debug.DrawLine(transform.position,transform.position + (Vector3)directions[direction.Key] * distance,Color.blue);
            }
        }
        float angle = Mathf.Atan2(player.lastDirection.y, player.lastDirection.x) * Mathf.Rad2Deg;
        flameThrower.transform.rotation = Quaternion.Euler(0, 0, angle);

        
        if (OnMobile)
        {
            player.direction = MoveMobile();
        }
        else
        {
            player.direction = MovePC();
        }

        TestForInteractions();
    }
    

    private void FixedUpdate()
    {
        Move();
    }

    private List<Collider2D> touchingTurnstiles = new List<Collider2D>();
    private List<Collider2D> touchingBoxes = new List<Collider2D>();
    public void Interact()
    {
       
        if (GetComponent<PlayerBoxHolder>().boxHolding != null)
        {
            GetComponent<PlayerBoxHolder>().pickUp();
        }
        touchingTurnstiles.Clear();
        player.collider.GetContacts(touchingTurnstiles);
        
        foreach (Collider2D turnstileCollider in touchingTurnstiles)
        {
            if (turnstileCollider.CompareTag("Turnstile"))
            {
                turnstileCollider.GetComponentInParent<Turnstile>().checkForInteraction();
                return;
                
            }
        }
        touchingBoxes.Clear();
        player.collider.GetContacts(touchingBoxes);
        
        foreach (Collider2D boxCollider in touchingBoxes)
        {
            if (boxCollider.gameObject.layer == 15)
            {
                GetComponent<PlayerBoxHolder>().pickUp();
                return;
                
            }
        }
        
        
    }

    private void TestForInteractions()
    {
        if (GetComponent<PlayerBoxHolder>().boxHolding != null)
        {
            player.InteractButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "Drop Box";
            player.InteractButton.SetActive(true);
            return;
        }
        touchingTurnstiles.Clear();
        player.collider.GetContacts(touchingTurnstiles);
        
        foreach (Collider2D turnstileCollider in touchingTurnstiles)
        {
            if (turnstileCollider.CompareTag("Turnstile") && turnstileCollider.GetComponentInParent<Turnstile>().overTernstile)
            {
                player.InteractButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "Enter";
                player.InteractButton.SetActive(true);
                return;
                
            }
        }
        touchingBoxes.Clear();
        player.collider.GetContacts(touchingBoxes);
        
        foreach (Collider2D boxCollider in touchingBoxes)
        {
            if (boxCollider.gameObject.layer == 15)
            {
                player.InteractButton.transform.GetChild(0).GetComponent<TMP_Text>().text = "Pick Up Box";
                player.InteractButton.SetActive(true);
                return;
                
            }
        }
        player.InteractButton.SetActive(false);
    }

    bool rayCastHit(String direction)
    {
        if (direction == "A" || direction == "D")
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position + Vector3.down * sideRayDistance, directions[direction], distance, layer);
            RaycastHit2D hit1 = Physics2D.Raycast(transform.position + Vector3.up * sideRayDistance, directions[direction], distance, layer);
            Debug.DrawLine(transform.position + Vector3.down * sideRayDistance,transform.position + Vector3.down * sideRayDistance + (Vector3)directions[direction] * distance,Color.blue);
            Debug.DrawLine(transform.position + Vector3.up * sideRayDistance,transform.position + Vector3.up * sideRayDistance + (Vector3)directions[direction] * distance,Color.blue);
            return hit.transform == null || hit1.transform ;
        }
        else
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, directions[direction], distance, layer);
            Debug.DrawLine(transform.position,transform.position + (Vector3)directions[direction] * distance,Color.blue);
            return hit.transform == null;
        }

        



    }

    bool anyDirectionPressed()
    {
        return Keyboard.current.sKey.isPressed || Keyboard.current.wKey.isPressed || Keyboard.current.aKey.isPressed ||
               Keyboard.current.dKey.isPressed || Vector2.Distance(variableJoystick.Direction,new Vector2(0,0) ) > 0.1f;
    }

    Vector2 MoveMobile()
    {
        Vector3 d = Vector3.forward * variableJoystick.Vertical + Vector3.right * variableJoystick.Horizontal;
        Vector2 direction = new Vector2(d.x, d.z);

        if (direction.x < 0 && !rayCastHit("A"))
        {
            direction.x = 0;
        }
        if (direction.x > 0 && !rayCastHit("D"))
        {
            direction.x = 0;
        }
        if (direction.y < 0 && !rayCastHit("S"))
        {
            direction.y = 0;
        }
        if (direction.y > 0 && !rayCastHit("W"))
        {
            direction.y = 0;
        }

        return direction;


    }

    Vector2 MovePC()
    {
        Vector2 direction = new Vector2(0, 0);
        if (Keyboard.current.aKey.isPressed)
        {

            if (rayCastHit("A"))
            {
                direction.x -= 1;
            }

            

        }

        if (Keyboard.current.dKey.isPressed)
        {
            if (rayCastHit("D"))
            {
                direction.x += 1;
            }
            
       
        }

        if (Keyboard.current.wKey.isPressed)
        {
            if (rayCastHit("W"))
            {
                direction.y += 1;
            }


        }

        if (Keyboard.current.sKey.isPressed)
        {
            if (rayCastHit("S"))
            {
                direction.y -= 1;
            }
        }

        return direction;

    }
    
    void Move()
    {
        
        if (player.allowedToWalk)
        {
            player.direction = player.direction.normalized;
        }

        bool shooting = animateFire();

        if (anyDirectionPressed() && !player.isJumping)
        {

            if (player.direction.magnitude > 0.1f)
            {
                player.lastDirection = player.direction;
                
                var main = fireParticleSystem.main;
                main.startSpeed = startingFireSpeed * 3f;

            }
            else
            {
                var main = fireParticleSystem.main;
                main.startSpeed = startingFireSpeed * 3f;
            }
            
            if (player.allowedToWalk)
            {
                if(!shooting)
                {
                    AnimateMovement();
                }
                
                // Apply speed reduction if on stairs
                float currentSpeed = player.speed * Stair.GetSpeedModifier();

                // Apply slight upward force if moving horizontally on stairs
                if (Stair.IsPlayerOnAnyStair() && (player.direction.x > 0.1f || player.direction.x < 0.1f) )
                {
                    if (player.direction.x > 0.1f)
                    {
                        player.direction.y -= Stair.GetInclineForce();
                    }
                    else
                    {
                        player.direction.y += Stair.GetInclineForce();
                    }
                    
                    player.direction = player.direction.normalized; // Re-normalize to prevent faster diagonal movement
                }

                transform.position += (Vector3)(player.direction * currentSpeed * Time.deltaTime);
            }
            
            

            player.isMoving = true;
        }
        else
        {
            if (player.isFootSteps)
            {
                player.audioSource.Stop();
                player.isFootSteps = false;
            }
            player.isMoving = false;
            if (!player.isJumping )
            {
                if(!shooting)
                {
                    PlayIdleAnimation(player.lastDirection);
                }
                
            }
            else
            {
                changeAnimation("jump");
            }
            var main = fireParticleSystem.main;
            main.startSpeed = startingFireSpeed ;
        }
        
    }
    
    
    public void AnimateMovement()
    {

        if (player.isJumping)
        {
            changeAnimation("jump");
            return;
        }
        if (!anyDirectionPressed())
        {

            return;
        }

        if (player.isMoving && !player.isFootSteps)
        {
            player.audioSource.Play();
            player.isFootSteps = true;
        }

        float angle = Mathf.Atan2(player.lastDirection.y, player.lastDirection.x) * Mathf.Rad2Deg;

        // Snap to 8-directional angles (0°, 45°, 90°, etc.)
        int snappedAngle = Mathf.RoundToInt(angle / 45) % 8;
        snappedAngle = (snappedAngle < 0) ? snappedAngle + 8 : snappedAngle;

       
        switch (snappedAngle)
        {
            case 0: changeAnimation("run_gun_right"); break; // 0°
            case 1: changeAnimation("run_gun_right_up"); break; // 45°
            case 2: changeAnimation("run_gun_up"); break; // 90°
            case 3: changeAnimation("run_gun_left_up"); break; // 135°
            case 4: changeAnimation("run_gun_left"); break; // 180°
            case 5: changeAnimation("run_gun_left_down"); break; // 225°
            case 6: changeAnimation("run_gun_down"); break; // 270°
            case 7: changeAnimation("run_gun_right_down"); break; // 315°
        }

    }

    public bool animateFire()
    {
        if (!player.hasFlamethrower)
        {
            return false;
        }
        if (Keyboard.current.spaceKey.isPressed)
        {
            float angle = Mathf.Atan2(player.lastDirection.y, player.lastDirection.x) * Mathf.Rad2Deg;

            // Snap to 8-directional angles (0°, 45°, 90°, etc.)
            int snappedAngle = Mathf.RoundToInt(angle / 45) % 8;
            snappedAngle = (snappedAngle < 0) ? snappedAngle + 8 : snappedAngle;

            string sufix = "idle";
            if (player.isMoving)
            {
                sufix = "run";

            }
            switch (snappedAngle)
            {
                case 0: changeAnimation("right" + sufix); break; // 0°
                case 1: changeAnimation("upright"+ sufix); break; // 45°
                case 2: changeAnimation("up"+ sufix); break; // 90°
                case 3: changeAnimation("upleft"+ sufix); break; // 135°
                case 4: changeAnimation("left"+ sufix); break; // 180°
                case 5: changeAnimation("downleft"+ sufix); break; // 225°
                case 6: changeAnimation("down"+ sufix); break; // 270°
                case 7: changeAnimation("downright"+ sufix); break; // 315°
            }
            return true;
        }
        return false;
    }

    public void PlayIdleAnimation(Vector2 lastDirection)
    {
        float angle = Mathf.Atan2(lastDirection.y, lastDirection.x) * Mathf.Rad2Deg;
        angle = (angle < 0) ? angle + 360 : angle; // Convert to 0-360° range

        string animName;

        // Snap to nearest available idle direction
        if (angle >= 22.5f && angle < 67.5f) animName = "idle_gun_right_up";
        else if (angle >= 67.5f && angle < 112.5f) animName = "idle_gun_up";
        else if (angle >= 112.5f && angle < 157.5f) animName = "idle_gun_left_up";
        else if (angle >= 157.5f && angle < 202.5f) animName = "idle_gun_left_down"; // (No pure left)
        else if (angle >= 202.5f && angle < 247.5f) animName = "idle_gun_left_down";
        else if (angle >= 247.5f && angle < 292.5f) animName = "idle_gun_down";
        else if (angle >= 292.5f && angle < 337.5f) animName = "idle_gun_right_down";
        else animName = "idle_gun_right_down"; // (No pure right)

        changeAnimation(animName);
    }


    public void changeAnimation(string animation)
    {
        if (!currentAnimation.Equals(animation))
        {
            
            ani.Play(animation);    
        }
    }
}
