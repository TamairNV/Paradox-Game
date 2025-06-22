using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [SerializeField] private SpriteRenderer circleWipe;
    [SerializeField] public GameObject InteractButton;
    public bool InteractButtonOn = false;
    public Vector3 StartPosition;
    public int lastLevelCompleted = 0;
    [SerializeField] public GameObject doorLine;
    public double time = 0;
    [HideInInspector] public float jumpProgress;
    [HideInInspector] public Vector3 jumpStartPosition;
    [HideInInspector] public Vector3 jumpPeakPosition;
    [HideInInspector] public Vector3 jumpTargetPosition;
    [SerializeField] public GameObject shadow;
    
    public bool hasFlamethrower = false;

    //[SerializeField] public List<DimentionalObjects> DimentionalObjects = new List<DimentionalObjects>();
    //private Dictionary<int, List<Tuple<int, objData>>> objDatas = new Dictionary<int, List<Tuple<int, objData>>>();
    public List<DimentionalPlayer> DimentionalPlayers = new List<DimentionalPlayer>();

    public bool isMoving;

    [SerializeField] public float speed = 100;

    [SerializeField] public GameObject DimPlayer;

    public Vector3 direction;
    public Vector2 lastDirection;
    public bool IsImmune = false;
    public DimensionalLinkedList timeEngine;
    private float timer = 0;
    public float MomentRate = 20;

    [SerializeField] public float xStep = 0.1f;
    [SerializeField] public float yStep = 0.5f;
    [SerializeField] public LineRenderer line;

    [SerializeField] private AudioClip footSteps;
    public bool isJumping = false;

    [SerializeField] public AudioSource audioSource;

    [HideInInspector] public bool isFootSteps = false;

    private List<CheckPoint> checkPoints = new List<CheckPoint>();
    [HideInInspector]
    public bool allowedToWalk = true;
    private bool reseting = false;
    
    [HideInInspector]
    public Collider2D collider;

    [SerializeField] private GameObject bomb;
    [SerializeField] private float bombPlaceDistance = 0.1f;
    public int bombCount = 5;
    [SerializeField] private TMP_Text bombCountText;
    [SerializeField] private Image entropyMeter;
    public float Entropy = 0;
    public float EntropyPlayerCollideValue = 5;
    public float EntropyBoxCollideValue = 5;

    public float MaxEntropy = 15;

    public void resetPlayer()
    {
        DimentionalObjects.Objects = new List<DimentionalObjects>();
    }

    IEnumerator setLine()
    {
        while (true)
        {
            line.positionCount = timeEngine.linePositions.Count;
            line.SetPositions(timeEngine.linePositions.ToArray());
            line.Simplify(0);
            line.transform.GetComponent<UILineRenderer>().UpdateLine();
            yield return new WaitForSeconds(0.05f);
        }
    }

    void MomentUpdate()
    {
        //UpdateDimObjects();
        DimentionalObjects.UpdateAllDimObjects();

        timeEngine.stepNode();

        timeEngine.CurrentDimensionalNode.data = CreateMoment();
        int i = 0;
        foreach (var dimNode in timeEngine.CurrentDimensionalNode.SameTimeNodes)
        {
            if (i >= timeEngine.CurrentDimensionalNode.SameTimeNodes.Count - 1)
            {
                break;
            }

            DimentionalPlayer dimentionalPlayer = DimentionalPlayers[i];
            dimentionalPlayer.currentNode = dimNode;
            if (Vector3.Distance(dimentionalPlayer.transform.position, dimNode.data.Position) > 0.4f)
            {
                dimentionalPlayer.transform.position = dimNode.data.Position;
            }

            dimentionalPlayer.MomentUpdate();
            dimentionalPlayer.setVisable();
            i++;
        }


        for (int j = i; j < DimentionalPlayers.Count; j++)
        {
            DimentionalPlayers[j].setInvisable();
        }
    }

    MomentData CreateMoment()
    {
        Vector2 dir = new Vector2(lastDirection.x, lastDirection.y);
        Vector3 pos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        MomentData newMoment = new MomentData(dir, isMoving, pos);
        return newMoment;
    }

    public DimentionalPlayer ReverseDirection()
    {
        Entropy += 1;
        GameObject dimPlayer = GameObject.Instantiate(DimPlayer);
        dimPlayer.GetComponent<DimentionalPlayer>()
            .InitDimPlayer(timeEngine.CurrentDimensionalNode, timeEngine, MomentRate, this);
        dimPlayer.transform.position = transform.position;
        DimentionalPlayers.Add(dimPlayer.GetComponent<DimentionalPlayer>());
        timeEngine.reverseDirection();
        return dimPlayer.GetComponent<DimentionalPlayer>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        collider = GetComponent<Collider2D>();


        timeEngine = new DimensionalLinkedList(xStep, yStep, line);

        timeEngine.CurrentDimensionalNode = new DimensionalNode(null, null, null, null, timeEngine.CurrentTime);
        timeEngine.CurrentDimensionalNode.data = CreateMoment();
        checkPoints.Add(
            new CheckPoint(0, transform.position, timeEngine.CurrentTime, timeEngine.CurrentDimensionalNode));

        StartCoroutine(setLine());
        StartPosition = transform.position;
    }

    private float bombTimer = 0;

    public void reset()
    {
        StartCoroutine(CauseParadox());
    }

    public void GoHome()
    {
        StartCoroutine(sendPlayerHome());
    }
    private IEnumerator sendPlayerHome()
    {
        yield return StartCoroutine(RunCircleWipe());

        transform.position = StartPosition;
        resetPlayer();

        yield return new WaitForSeconds(0.6f);
        resetGame();
        yield return StartCoroutine(ReverseCircleWipe());
        time = 0;
        
        
    }

    
    // Update is called once per frame
    void Update()
    {
        bombCountText.text = bombCount.ToString();
        entropyMeter.fillAmount = Entropy / MaxEntropy;
        if (Entropy >=MaxEntropy && !reseting)
        {
            StartCoroutine(CauseParadox());
            print("Entropy too high");
        }
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            PlaceBomb();
        }

        bombTimer += Time.deltaTime;


        if (Keyboard.current.backspaceKey.wasPressedThisFrame)
        {
            StartCoroutine(CauseParadox());
            print("reset");
        }


        timer += Time.deltaTime;
        if (timer > 1 / MomentRate && !reseting)
        {
            timer = 0;
            MomentUpdate();
        }

        if (timeEngine.CurrentTime <= 0 && !reseting)
        {
            StartCoroutine(CauseParadox());
            print("time went negative");
        }
        
        
    }

    public void PlaceBomb()
    {
        if (bombTimer > 0.5f && bombCount > 0)
        {
            bombCount--;
            GameObject b = Instantiate(bomb);
            b.transform.position = transform.position + (Vector3)(bombPlaceDistance * lastDirection) ;
            bombTimer = 0;
        }
    }


    [SerializeField] public Volume volume;


    public void resetGame()
    {
        allowedToWalk = true;
        Entropy = 0;

        // Destroy players created after checkpoint
        DimentionalPlayers.RemoveAll(player =>
        {
            Destroy(player.gameObject);
            return true;
        });

        // Reset objects
        //objDatas = new Dictionary<int, List<Tuple<int, objData>>>();


        // Final cleanup
        foreach (var bridge in Bridge.Bridges)
        {
            bridge.reset();
        }

        line.SetPositions(new Vector3[0]);
        timeEngine = new DimensionalLinkedList(xStep, yStep, line);
        DimentionalPlayers = new List<DimentionalPlayer>();
        timeEngine.CurrentDimensionalNode = new DimensionalNode(null, null, null, null, timeEngine.CurrentTime);
        timeEngine.CurrentDimensionalNode.data = CreateMoment();
        checkPoints.Add(
            new CheckPoint(0, transform.position, timeEngine.CurrentTime, timeEngine.CurrentDimensionalNode));
        StartCoroutine(setLine());
        Turnstile.resetAll();

        Portal.ResetPortals();
        DimentionalObjects.ResetAllObj();
        Transform box = GetComponent<PlayerBoxHolder>().boxHolding;
        if (box != null)
        {
            box.GetComponent<BoxCollider2D>().enabled = true;
        }
        else
        {
            GetComponent<PlayerBoxHolder>().boxHolding = null;
        }
        




        Bomb.DestroyALlBombs();
        line.transform.position = line.transform.GetComponent<UILineRenderer>().startPosition;
    }

    public float circleWipeSpeed = 2f;
    public IEnumerator RunCircleWipe()
    {
        float radius = 1.5f;
        

        while (radius > -0.2f)
        {
            circleWipe.material.SetFloat("_Radius" , radius);
            radius -= circleWipeSpeed * Time.deltaTime;
            yield return null;
        }
    }
    public IEnumerator ReverseCircleWipe()
    {
        float radius = -0.2f;
        

        while (radius < 1.5f)
        {

            circleWipe.material.SetFloat("_Radius" , radius);
            radius += circleWipeSpeed * Time.deltaTime;
            yield return null;
        }
    }

    public IEnumerator CauseParadox()
    {
        if (IsImmune)
        {
            yield break;
        }

        reseting = true;
        // Get all post-processing effects
        Vignette vignette = null;
        ChromaticAberration chromatic = null;
        FilmGrain noise = null;
        LensDistortion distortion = null;
        Bloom bloom = null; // For screen-space flare effect

        allowedToWalk = false;

        // Verify all effects exist
        bool effectsFound = 
                            volume.profile.TryGet(out chromatic) &&
                            volume.profile.TryGet(out noise) &&
                            volume.profile.TryGet(out distortion) &&
                            volume.profile.TryGet(out bloom);

        if (!effectsFound)
        {
            Debug.LogError("Missing required post-processing effects!");
            yield break;
        }

        // Initial values

        float chromaticValue = 0;
        float noiseValue = 0;
        float distortionValue = 0;
        float bloomValue = bloom.intensity.value;

        // PHASE 1: Reality distortion buildup (3 seconds)
        float duration = 2f;
        float elapsed = 0f;
        float radius = 3f;
        while (elapsed < duration || radius > -0.2f)
        {
            
            circleWipe.material.SetFloat("_Radius" , radius);
            radius -= circleWipeSpeed * Time.deltaTime;

            float t = elapsed / duration;
            
            chromaticValue = Mathf.Lerp(0, 2, t * 2f); // Chromatic aberration grows faster
            distortionValue = Mathf.Lerp(0, -1f, t); // Negative for "implosion" effect
            chromatic.intensity.Override(chromaticValue);
            distortion.intensity.Override(distortionValue);

            elapsed += Time.deltaTime;
            yield return null;
        }
        
        CheckPoint checkPoint = checkPoints.Last();
        transform.position = checkPoint.location;
        timeEngine.direction = checkPoint.direction;
        resetGame();
        time = 0;
        duration = 2f;
        elapsed = 0f;
   
        yield return new WaitForSeconds(0.4f);
        while (elapsed < duration || radius < 1.5f)
        {
            circleWipe.material.SetFloat("_Radius" , radius);
            radius += circleWipeSpeed * Time.deltaTime;
          
            float t = elapsed / duration;
            
            chromaticValue = Mathf.Lerp(2, 0, t * 1.5f); // Chromatic fades faster
            distortionValue = Mathf.Lerp(-1f, 0, t);
            chromatic.intensity.Override(chromaticValue);
            distortion.intensity.Override(distortionValue);

            elapsed += Time.deltaTime;
            yield return null;
        }

        reseting = false;
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 16)
        {
            CheckPoint checkPoint = new CheckPoint(timeEngine.CurrentTime, transform.position, timeEngine.CurrentTime,
                timeEngine.CurrentDimensionalNode);
            checkPoints.Add(checkPoint);
        }

        if (other.gameObject.layer == 18)
        {
            if (other.gameObject.transform.parent.GetComponent<Bomb>().isExploding && !reseting)
            {
                StartCoroutine(CauseParadox());
                print("player hit by bomb");
            }

        }


    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.gameObject.layer == 15)
        {
            DimentionalObjects box = other.GetComponent<DimentionalObjects>();
            if(box.beenInteractedWith && box.data.ContainsKey(timeEngine.CurrentTime) && !IsImmune)
            {
                Entropy += EntropyBoxCollideValue * Time.deltaTime;
            }

            if (box.Temp == 1 && !reseting)
            {
                StartCoroutine(CauseParadox());
                print("Player burnt by object");
            }
            
        }

        if (other.gameObject.layer == 12 && !IsImmune)
        {
            Entropy += EntropyPlayerCollideValue * Time.deltaTime;
            
        }


    }

    public IEnumerator BecomeImmune(int legnth)
    {
        IsImmune = true;
        yield return new WaitForSeconds(legnth);
        IsImmune = false;
    }
}

public class CheckPoint
{
    public int time;
    public Vector3 location;
    public int direction;
    public DimensionalNode Node;

    public CheckPoint(int time, Vector3 location, int direction, DimensionalNode node)
    {
        this.time = time;
        this.location = new Vector3(location.x, location.y, location.z);
        this.direction = direction;
        Node = node;
    }
}