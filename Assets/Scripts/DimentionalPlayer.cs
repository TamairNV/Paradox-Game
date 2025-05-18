using UnityEngine;

public class DimentionalPlayer : MonoBehaviour
{
    private DimensionalNode startNode;
    public DimensionalNode currentNode;
    private DimensionalLinkedList timeEngine;
    
    

    private bool isMoving;
    private Vector2 direction;
    private SpriteRenderer sp;
    private Animator ani;
    private string currentAnimation = "";


    public void InitDimPlayer(DimensionalNode startNode,DimensionalLinkedList timeEngine)
    {
        this.startNode = startNode;
        this.timeEngine = timeEngine;
        currentNode = startNode;
    }
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Awake()
    {
        sp = GetComponent<SpriteRenderer>();
        ani = GetComponent<Animator>();
    }
    
    public void MomentUpdate()
    {
        loadPlayerState();
        setAnimations();
    }

    private void loadPlayerState()
    {
        if (currentNode != null && currentNode.data != null)
        {
            direction = currentNode.data.Direction;
            isMoving = currentNode.data.isMoving;
            transform.position = currentNode.data.Position;
        }

    }
    public void setInvisable()
    {
        sp.enabled = false;
    }

    public void setVisable()
    {
        sp.enabled = true;
    }
    private void setAnimations()
    {
        if (isMoving)
        {
            AnimateMovement();
        }
        else
        {
            PlayIdleAnimation();
        }
    }
    
    
    void AnimateMovement()
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        
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
    
    private void PlayIdleAnimation()
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

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
