using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player_Controller : MonoBehaviour
{


    private string currentAnimation = "sdsf";

    private Animator ani;
    [SerializeField] public float speed = 100;

    public Vector3 direction;
    private Vector2 lastDirection;
    


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {;
        ani = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    
    }

    void Move()
    {
        direction = new Vector2(0, 0);
        if (Keyboard.current.aKey.isPressed)
        {
            direction.x -= 1;
        }
        if (Keyboard.current.dKey.isPressed)
        {
            direction.x +=1;
        }
        if (Keyboard.current.wKey.isPressed)
        {
            direction.y += 1;
        }
        if (Keyboard.current.sKey.isPressed)
        {
            direction.y -= 1;
        }

        direction = direction.normalized;
        
        if (direction.magnitude > 0.1f)
        {
            AnimateMovement();
            lastDirection = direction;
            transform.position += direction * Time.deltaTime * speed;
        }
        else
        {
            PlayIdleAnimation(lastDirection);
        }
        
        
    }

    void AnimateMovement()
    {
        if (direction.magnitude < 0.1f)
        {
            return;
        } 

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        
        // Snap to 8-directional angles (0°, 45°, 90°, etc.)
        int snappedAngle = Mathf.RoundToInt(angle / 45) % 8;
        snappedAngle = (snappedAngle < 0) ? snappedAngle + 8 : snappedAngle;

        switch (snappedAngle)
        {
            case 0:  changeAnimation("run_gun_right"); break;      // 0°
            case 1:  changeAnimation("run_gun_right_up"); break;   // 45°
            case 2:  changeAnimation("run_gun_up"); break;         // 90°
            case 3:  changeAnimation("run_gun_left_up"); break;    // 135°
            case 4:  changeAnimation("run_gun_left"); break;       // 180°
            case 5:  changeAnimation("run_gun_left_down"); break;  // 225°
            case 6:  changeAnimation("run_gun_down"); break;       // 270°
            case 7:  changeAnimation("run_gun_right_down"); break; // 315°
        }
    }
    
    private void PlayIdleAnimation(Vector2 lastDirection)
    {
        float angle = Mathf.Atan2(lastDirection.y, lastDirection.x) * Mathf.Rad2Deg;
        angle = (angle < 0) ? angle + 360 : angle; // Convert to 0-360° range

        string animName;
    
        // Snap to nearest available idle direction
        if (angle >= 22.5f && angle < 67.5f)       animName = "idle_gun_right_up";
        else if (angle >= 67.5f && angle < 112.5f)  animName = "idle_gun_up";
        else if (angle >= 112.5f && angle < 157.5f) animName = "idle_gun_left_up";
        else if (angle >= 157.5f && angle < 202.5f) animName = "idle_gun_left_down"; // (No pure left)
        else if (angle >= 202.5f && angle < 247.5f) animName = "idle_gun_left_down";
        else if (angle >= 247.5f && angle < 292.5f) animName = "idle_gun_down";
        else if (angle >= 292.5f && angle < 337.5f) animName = "idle_gun_right_down";
        else                                        animName = "idle_gun_right_down"; // (No pure right)

        changeAnimation(animName);
    }


    private void changeAnimation(string animation)
    {
        if (!currentAnimation.Equals(animation))
        {
            ani.Play(animation);
        }
    }
}
