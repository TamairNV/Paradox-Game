using System;
using UnityEngine;

public class DimentionalObjects : MonoBehaviour
{

    public static int number = 0;
    public Vector3 targetPosition;

    public Quaternion targetRotation;

    public bool beenInteractedWith = false;

    public Vector3 StartingPosition;
    public Quaternion  StartingRotation;

    public int ID;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameObject player = GameObject.Find("player");
        if (player != null)
        {
            GameObject.Find("player").GetComponent<Player_Controller>().DimentionalObjects.Add(this);
            
        }
        else
        {
            print("No player found in scene");
        }
        
        ID = number;
        number++;
        StartingPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        
        StartingRotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);
    }


    public float moveSpeed = 5f; // Units per second
    public float rotateSpeed = 5f; // Degrees per second

    void Update()
    {
        float step = 0;
        if (targetPosition != Vector3.zero)
        {
             step = moveSpeed * Time.deltaTime;
            transform.position = Vector3.Lerp(transform.position, targetPosition, step);
        }


        step = rotateSpeed * Time.deltaTime;
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, step);
    
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == 13)
        {
            beenInteractedWith = true;
            other.transform.GetComponent<Player_Controller>().UpdateDimObjects();
        }
        
     
    }
}

public class objData
{
    public Vector3 Position;
    public Quaternion Rotation;

    public objData(DimentionalObjects data)
    {
        Position = new Vector3(data.transform.position.x, data.transform.position.y, data.transform.position.z);
        Rotation = new Quaternion(data.transform.rotation.x, data.transform.rotation.y, data.transform.rotation.z, data.transform.rotation.w);
    }
}
