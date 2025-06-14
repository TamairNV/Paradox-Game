using UnityEngine;
using UnityEngine.InputSystem;

public class flameThrower : MonoBehaviour
{
    [SerializeField] private Player player;
    private Animator ani;
    private string currentAnimation = "";
    private bool stopped = false;
    private bool isFireing = true;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ani = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.spaceKey.isPressed)
        {
            isFireing = true;
        }
        else
        {
            isFireing = false;
        }
    }

    private void handleFire()
    {
        if (isFireing)
        {
            float angle = Mathf.Atan2(player.lastDirection.y, player.lastDirection.x) * Mathf.Rad2Deg;
    
            // Snap to 8-directional angles (0°, 45°, 90°, etc.)
            int snappedAngle = Mathf.RoundToInt(angle / 45) % 8;
            snappedAngle = (snappedAngle < 0) ? snappedAngle + 8 : snappedAngle;
    
            switch (snappedAngle)
            {
                case 0: changeAnimation("fireStraight"); break; // 0°
                case 1: changeAnimation("run_gun_right_up"); break; // 45°
                case 2: changeAnimation("run_gun_up"); break; // 90°
                case 3: changeAnimation("run_gun_left_up"); break; // 135°
                case 4: changeAnimation("run_gun_left"); break; // 180°
                case 5: changeAnimation("run_gun_left_down"); break; // 225°
                case 6: changeAnimation("run_gun_down"); break; // 270°
                case 7: changeAnimation("fireDown"); break; // 315°
            }
        }
        else if (!stopped)
        {
            stopped = true;
        }
    }

    private void changeAnimation(string animation)
    {
        if (animation != currentAnimation)
        {
            ani.Play(animation);
            currentAnimation = animation;
        }
    }
}
