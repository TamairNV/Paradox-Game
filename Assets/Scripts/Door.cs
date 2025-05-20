using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] public List<DoorButton> buttons = new List<DoorButton>();

    private Animator ani;

    private string currentAnimation = ""; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ani = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var button in buttons)
        {
            if (!button.isPressed)
            {
                changeAnimation("close");
                return;
            }
        }

        changeAnimation("open");
    }
    private void changeAnimation(string animation)
    {
        if (!currentAnimation.Equals(animation))
        {
            ani.Play(animation);
        }
    }
}
