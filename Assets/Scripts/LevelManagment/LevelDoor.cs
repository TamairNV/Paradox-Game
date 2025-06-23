using UnityEngine;

public class LevelDoor : MonoBehaviour
{
    [SerializeField] private int level;

    private Player player;

    private string currentAnimation = "";

    private Animator ani;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("player").GetComponent<Player>();
        ani = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.lastLevelCompleted+1 >= level )
        {
            changeAnimation("close");
        }
        else
        {
            changeAnimation("open");
            
        }
    }
    
    private void changeAnimation(string animation)
    {
        if (!currentAnimation.Equals(animation))
        {
            
            ani.Play(animation);    
        }
    }
}
