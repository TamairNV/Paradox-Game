using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField] public List<DoorButton> buttons = new List<DoorButton>();
    [SerializeField] public List<PressurePlate> PressurePlates = new List<PressurePlate>();

    private Animator ani;
    public bool isOpen;

    private string currentAnimation = ""; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ani = GetComponent<Animator>();
        foreach (var plate in PressurePlates)
        {
            plate.Doors.Add(this.transform);
        }
        foreach (var button in buttons)
        {
            button.Doors.Add(this.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        isOpen = false;
        foreach (var button in buttons)
        {
            if (!button.isPressed)
            {
                changeAnimation("close");
                
                return;
            }
        }
        foreach (var plate in PressurePlates)
        {
            if (!plate.isPressed)
            {
                changeAnimation("close");
                return;
            }
        }

        isOpen = true;
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
