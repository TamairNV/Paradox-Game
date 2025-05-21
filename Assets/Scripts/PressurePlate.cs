using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    
    public bool isPressed = false;
    [SerializeField] private Sprite up;
    [SerializeField] private Sprite down;
    private SpriteRenderer sr;
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

   
    void Update()
    {
        if (isPressed)
        {
            sr.sprite = down;
        }
        else
        {
            sr.sprite = up;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == 12 || other.gameObject.layer == 13 || other.gameObject.layer == 15)
        {
            isPressed = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 12|| other.gameObject.layer == 13|| other.gameObject.layer == 15)
        {
            isPressed = false;
        }
    }
}