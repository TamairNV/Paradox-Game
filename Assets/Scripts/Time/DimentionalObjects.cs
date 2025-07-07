using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DimentionalObjects : MonoBehaviour
{

    public static int number = 0;
    public Vector3 targetPosition;

    public Quaternion targetRotation;
    private bool targetRotationNull = true;
    public bool beenInteractedWith = false;

    public Vector3 StartingPosition;
    public Quaternion  StartingRotation;

    public Dictionary<int, objData> data = new Dictionary<int, objData>();
    private Player player;

    public static List<DimentionalObjects> Objects = new List<DimentionalObjects>();
    public bool isDestoryed = false;
    public int ID;
    [HideInInspector]
    public Vector3 startingScale;
    
    


    public int Temp = 0;
    private int StartTemp;

    private Rigidbody2D rb;
    private float startingMass;

    public float playerInsideBannedBox = 0;
    private float beforeInsideBannedBox = 0;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startingMass = rb.mass;
        StartTemp = Temp;
        changeBoxType();
        startingScale = transform.localScale;
        Objects.Add(this);
        GameObject playerObj = GameObject.Find("player");
        if (playerObj != null)
        {
            //playerObj.GetComponent<Player_Controller>().DimentionalObjects.Add(this);
            player = playerObj.GetComponent<Player>();

        }
        else
        {
            print("No player found in scene");
        }
        
        ID = number;
        number++;
        StartingPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        
        StartingRotation = new Quaternion(transform.rotation.x, transform.rotation.y, transform.rotation.z, transform.rotation.w);

        StartCoroutine(PulseWhite());
    }

    public static void UpdateAllDimObjects()
    {
        foreach (var obj in Objects)
        {
            obj.MomentUpdate();
        }
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
            if (Vector3.Distance(transform.position, targetPosition) > 0.05f && Temp != 0)
            {
                StartCoroutine(player.CauseParadox());
                print("past self moved a object that is currently unmovable");
            }
        }

        if (!targetRotationNull)
        {
            step = rotateSpeed * Time.deltaTime;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, step);

        }



        if (isBurning)
        {
            burnTimer += Time.deltaTime;
            if (burnTimer > 1.5f)
            {
                isDestoryed = true;
                isBurning = false;
                burnTimer = 0;
            }
        }

        Temp = Math.Clamp(Temp, -1, 1);

        if (Temp == -1)
        {
            rb.mass = 10000f;
        }
        else
        {
            rb.mass = startingMass;
        }

        if (playerInsideBannedBox > 0.1f)
        {
            player.Entropy += player.EntropyBoxCollideValue * Time.deltaTime;
        }

        if (beforeInsideBannedBox == playerInsideBannedBox)
        {
            playerInsideBannedBox = 0;
        }

        

    }

    public void FixedUpdate()
    {
        beforeInsideBannedBox = playerInsideBannedBox;
    }

    public static void ResetAllObj()
    {
        foreach (var obj in Objects)
        {
            obj.RestObj();
        }
    }

    public void RestObj()
    {
        transform.position = StartingPosition;
        transform.rotation = StartingRotation;
        targetPosition = StartingPosition;
        targetRotation = StartingRotation;
        beenInteractedWith = false;
        GetComponent<SpriteRenderer>().enabled = true;
        data = new Dictionary<int, objData>();
        Temp = StartTemp;
        changeBoxType();
        MomentUpdate();

    }

    public void MomentUpdate()
    {
        int time = player.timeEngine.CurrentTime;
// Ensure time is non-negative
        if (time < 0) return; // or handle error

        if (beenInteractedWith && !data.ContainsKey(time))
        {
            data.Add(time,new objData(this));

            GetComponent<SpriteRenderer>().enabled = !isDestoryed;
            //GetComponent<Collider2D>().enabled = !isDestoryed;
            
        }

        if (beenInteractedWith && data.ContainsKey(time))
        {
            targetRotationNull = false;
            targetPosition = data[time].Position;
            targetRotation = data[time].Rotation;
            GetComponent<SpriteRenderer>().enabled = !data[time].isDestroyed;
            //GetComponent<Collider2D>().enabled = !data[time].isDestroyed;
        }
    }


    



    public float burnTimer = 0;
    private bool isBurning = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 17)
        {
            isBurning = true;
        }
        if (other.gameObject.layer == 13)
        {
            beenInteractedWith = true;
            if (!data.ContainsKey(player.timeEngine.CurrentTime))
            {
                data.Add( player.timeEngine.CurrentTime,new objData(this));
            }
            
            //other.transform.GetComponent<Player_Controller>().UpdateDimObjects();
        }

    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == 13)
        {
            if (beenInteractedWith && data.ContainsKey(player.timeEngine.CurrentTime))
            {
                playerInsideBannedBox += Time.deltaTime;
                
            }
        }
    }


    public void changeBoxType()
    {
        if (Temp == 0)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
        }

        if (Temp == -1)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
        }

        if (Temp == 1)
        {
            transform.GetChild(0).gameObject.SetActive(true);
            transform.GetChild(1).gameObject.SetActive(false);
        }
    }


    
    
    
    private IEnumerator PulseWhite()
    {
        while (true)
        {
            yield return new WaitForSeconds(Time.deltaTime);
            if (isBurning)
            {
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 1,1-(burnTimer/1.5f));
            }
            else
            {
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 1,1);
            }


            
        }
    }
}

public class objData
{
    public Vector3 Position;
    public Quaternion Rotation;
    public bool isDestroyed;

    public objData(DimentionalObjects data)
    {
        Position = new Vector3(data.transform.position.x, data.transform.position.y, data.transform.position.z);
        Rotation = new Quaternion(data.transform.rotation.x, data.transform.rotation.y, data.transform.rotation.z, data.transform.rotation.w);
        isDestroyed = data.isDestoryed;
    }
}
