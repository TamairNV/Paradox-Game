using UnityEngine;

public class BurnWall : MonoBehaviour
{
    private SpriteRenderer sr;

    public bool startBurned = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        sr = transform.GetComponent<SpriteRenderer>();
        if (startBurned)
        {
            sr.enabled = false;
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == 18)
        {
            
            Bomb bomb = other.gameObject.GetComponentInParent<Bomb>();
            
            if (bomb.isHot)
            {
                sr.enabled = false;
                transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = false;
            }
            else
            {
                sr.enabled = true;
                transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = true;
                
            }
        }
        
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.layer == 18)
        {
            Bomb bomb = other.gameObject.GetComponentInParent<Bomb>();
            if (bomb.isHot)
            {
                sr.enabled = false;
                transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = false;
            
            }
            else
            {
                sr.enabled = true;
                transform.GetChild(0).GetComponent<BoxCollider2D>().enabled = true;
              
            }
        }
    }
}
