using System.Collections.Generic;
using UnityEngine;

public class Bridge : MonoBehaviour
{
    private Animator ani;
    [SerializeField] private Door Door;

    [SerializeField] private Player_Controller player;
    private List<int> breakPoints = new List<int>();
    private string currentAnimation = "";
    private bool hasBroken = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ani = GetComponent<Animator>();
    }


    // Update is called once per frame
    void Update()
    {
        if (!hasBroken)
        {
            
            // Get the first child's BoxCollider2D
        BoxCollider2D collider = transform.GetChild(0).GetComponent<BoxCollider2D>();

        // Check if the player is overlapping this collider
        Collider2D[] overlappingColliders = new Collider2D[10];
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.GetMask("RealPlayer")); // Assuming player is on "Player" layer
        int count = collider.Overlap(filter, overlappingColliders);

        // Check if any overlapping collider belongs to player "13"
        for (int i = 0; i < count; i++) 
        {
            if (overlappingColliders[i].gameObject.layer == 13) 
            {
                print("trigger");
                breakPoints.Add(player.timeEngine.CurrentTime);
                breakPoints.Add(player.timeEngine.CurrentTime+2*(int)player.MomentRate);
                changeAnimation("break");
                hasBroken = true;
                break;
            }
        }
        }


        

        if (breakPoints.Contains(player.timeEngine.CurrentTime))
        {
            if (player.timeEngine.direction == 1)
            {
                changeAnimation("break");
            }
            else
            {
                changeAnimation("fix");
            }
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
