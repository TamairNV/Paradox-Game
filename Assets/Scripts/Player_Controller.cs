using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Player_Controller : MonoBehaviour
{
    [SerializeField] private List<DimentionalObjects> DimentionalObjects = new List<DimentionalObjects>();
    private Dictionary<int, List<Tuple<int, objData>>> objDatas = new Dictionary<int, List<Tuple<int, objData>>>();
    public List<DimentionalPlayer> DimentionalPlayers = new List<DimentionalPlayer>();
    private string currentAnimation = "sdsf";
    public bool isMoving;
    private Animator ani;
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

    private List<CheckPoint> checkPoints = new List<CheckPoint>();

    public bool allowedToWalk = true;

    IEnumerator setLine()
    {
        while (true)
        {
            line.positionCount = timeEngine.linePositions.Count;
            line.SetPositions(timeEngine.linePositions.ToArray());
            line.Simplify(0);
            yield return new WaitForSeconds(0.05f);
        }
    }

    void MomentUpdate()
    {
        UpdateDimObjects();
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

    public void UpdateDimObjects()
    {
        if (!objDatas.ContainsKey(timeEngine.CurrentTime))
        {
            List<Tuple<int, objData>> newList = new List<Tuple<int, objData>>();
            objDatas.Add(timeEngine.CurrentTime, newList);
        }

        foreach (var obj in DimentionalObjects)
        {
            if (obj.beenInteractedWith && !objDatas[timeEngine.CurrentTime].Any(t => t.Item1 == obj.ID))
            {
                objDatas[timeEngine.CurrentTime].Add(new Tuple<int, objData>(obj.ID, new objData(obj)));
            }
        }


        foreach (var obj in DimentionalObjects)
        {
            if (objDatas.ContainsKey(timeEngine.CurrentTime))
            {
                if (objDatas[timeEngine.CurrentTime].Any(t => t.Item1 == obj.ID))
                {
                    objData d = objDatas[timeEngine.CurrentTime].Find(t => t.Item1 == obj.ID).Item2;
                    obj.targetPosition = d.Position;
                    obj.targetRotation = d.Rotation;
                }
            }
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
        GameObject dimPlayer = GameObject.Instantiate(DimPlayer);
        dimPlayer.GetComponent<DimentionalPlayer>()
            .InitDimPlayer(timeEngine.CurrentDimensionalNode, timeEngine, MomentRate,this);
        dimPlayer.transform.position = transform.position;
        DimentionalPlayers.Add(dimPlayer.GetComponent<DimentionalPlayer>());
        timeEngine.reverseDirection();
        return dimPlayer.GetComponent<DimentionalPlayer>();
    }


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ;

        
        timeEngine = new DimensionalLinkedList(xStep, yStep, line);

        timeEngine.CurrentDimensionalNode = new DimensionalNode(null, null, null, null, timeEngine.CurrentTime);
        timeEngine.CurrentDimensionalNode.data = CreateMoment();
        checkPoints.Add(
            new CheckPoint(0, transform.position, timeEngine.CurrentTime, timeEngine.CurrentDimensionalNode));
        ani = GetComponent<Animator>();
        StartCoroutine(setLine());
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        if (Keyboard.current.backspaceKey.wasPressedThisFrame)
        {
            StartCoroutine(CauseParadox());
        }


        timer += Time.deltaTime;
        if (timer > 1 / MomentRate)
        {
            timer = 0;
            MomentUpdate();
        }

        if (timeEngine.CurrentTime <= 0)
        {
            StartCoroutine(CauseParadox());
        }
    }

    void Move()
    {
        direction = new Vector2(0, 0);
        if (Keyboard.current.aKey.isPressed)
        {
            direction.x -= 1;
        }

        if (Keyboard.current.dKey.isPressed)
        {
            direction.x += 1;
        }

        if (Keyboard.current.wKey.isPressed)
        {
            direction.y += 1;
        }

        if (Keyboard.current.sKey.isPressed)
        {
            direction.y -= 1;
        }

        if (allowedToWalk)
        {
            direction = direction.normalized;
        }


        if (direction.magnitude > 0.1f)
        {
            AnimateMovement();
            lastDirection = direction;
            if (allowedToWalk)
            {
                transform.position += direction * Time.deltaTime * speed;
            }

            isMoving = true;
        }
        else
        {
            isMoving = false;
            PlayIdleAnimation(lastDirection);
        }
    }

    void AnimateMovement()
    {
        if (direction.magnitude < 0.1f)
        {
            return;
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Snap to 8-directional angles (0°, 45°, 90°, etc.)
        int snappedAngle = Mathf.RoundToInt(angle / 45) % 8;
        snappedAngle = (snappedAngle < 0) ? snappedAngle + 8 : snappedAngle;

        switch (snappedAngle)
        {
            case 0: changeAnimation("run_gun_right"); break; // 0°
            case 1: changeAnimation("run_gun_right_up"); break; // 45°
            case 2: changeAnimation("run_gun_up"); break; // 90°
            case 3: changeAnimation("run_gun_left_up"); break; // 135°
            case 4: changeAnimation("run_gun_left"); break; // 180°
            case 5: changeAnimation("run_gun_left_down"); break; // 225°
            case 6: changeAnimation("run_gun_down"); break; // 270°
            case 7: changeAnimation("run_gun_right_down"); break; // 315°
        }
    }

    private void PlayIdleAnimation(Vector2 lastDirection)
    {
        float angle = Mathf.Atan2(lastDirection.y, lastDirection.x) * Mathf.Rad2Deg;
        angle = (angle < 0) ? angle + 360 : angle; // Convert to 0-360° range

        string animName;

        // Snap to nearest available idle direction
        if (angle >= 22.5f && angle < 67.5f) animName = "idle_gun_right_up";
        else if (angle >= 67.5f && angle < 112.5f) animName = "idle_gun_up";
        else if (angle >= 112.5f && angle < 157.5f) animName = "idle_gun_left_up";
        else if (angle >= 157.5f && angle < 202.5f) animName = "idle_gun_left_down"; // (No pure left)
        else if (angle >= 202.5f && angle < 247.5f) animName = "idle_gun_left_down";
        else if (angle >= 247.5f && angle < 292.5f) animName = "idle_gun_down";
        else if (angle >= 292.5f && angle < 337.5f) animName = "idle_gun_right_down";
        else animName = "idle_gun_right_down"; // (No pure right)

        changeAnimation(animName);
    }


    private void changeAnimation(string animation)
    {
        if (!currentAnimation.Equals(animation))
        {
            ani.Play(animation);
        }
    }

    [SerializeField] private Volume volume;

    public IEnumerator CauseParadox()
    {
        if (IsImmune)
        {
            yield break;
        }
        // Get all post-processing effects
        Vignette vignette= null;
        ChromaticAberration chromatic = null;
        FilmGrain noise= null;
        LensDistortion distortion= null;
        Bloom bloom= null; // For screen-space flare effect

        allowedToWalk = false;

        // Verify all effects exist
        bool effectsFound = volume.profile.TryGet(out vignette) &&
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
        float vignetteValue = 0;
        float chromaticValue = 0;
        float noiseValue = 0;
        float distortionValue = 0;
        float bloomValue = bloom.intensity.value;

        // PHASE 1: Reality distortion buildup (3 seconds)
        float duration = 3f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            // Animate all effects simultaneously
            vignetteValue = Mathf.Lerp(0, 6f, t/10f);
            chromaticValue = Mathf.Lerp(0, 2, t * 2f); // Chromatic aberration grows faster
            noiseValue = Mathf.Lerp(0, 0.8f, t);
            distortionValue = Mathf.Lerp(0, -1f, t); // Negative for "implosion" effect
            bloomValue = Mathf.Lerp(bloom.intensity.value, 10f, t); // Hyper-bright flare

            // Apply values
            vignette.intensity.Override(vignetteValue);
            chromatic.intensity.Override(chromaticValue);
            noise.intensity.Override(noiseValue);
            distortion.intensity.Override(distortionValue);
            bloom.intensity.Override(bloomValue);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // PHASE 2: Reality reset (teleport/cleanup)
        //yield return new WaitForSeconds(0.5f); // Hold at peak distortion

        CheckPoint checkPoint = checkPoints.Last();
        transform.position = checkPoint.location;
        allowedToWalk = true;
        timeEngine.direction = checkPoint.direction;

        // Destroy players created after checkpoint
        DimentionalPlayers.RemoveAll(player =>
        {
            Destroy(player.gameObject);
            return true;
        });

        // Reset objects
        objDatas = new Dictionary<int, List<Tuple<int, objData>>>();
        foreach (var obj in DimentionalObjects)
        {
            obj.transform.position = obj.StartingPosition;
            obj.transform.rotation = obj.StartingRotation;
            obj.targetPosition = obj.StartingPosition;
            obj.targetRotation = obj.StartingRotation;
            obj.beenInteractedWith = false;
        }

        
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
        ani = GetComponent<Animator>();
        StartCoroutine(setLine());
        Turnstile.resetAll();
        
        duration = 2f;
        elapsed = 0f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

            vignetteValue = Mathf.Lerp(10, 0, t*2f);
            chromaticValue = Mathf.Lerp(2, 0, t * 1.5f); // Chromatic fades faster
            noiseValue = Mathf.Lerp(0.8f, 0, t);
            distortionValue = Mathf.Lerp(-1f, 0, t);
            bloomValue = Mathf.Lerp(10f, bloom.intensity.value, t);

            vignette.intensity.Override(vignetteValue);
            chromatic.intensity.Override(chromaticValue);
            noise.intensity.Override(noiseValue);
            distortion.intensity.Override(distortionValue);
            bloom.intensity.Override(bloomValue);

            elapsed += Time.deltaTime;
            yield return null;
        }


    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.layer == 12)
        {
            StartCoroutine(CauseParadox());
        }
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 16)
        {
            CheckPoint checkPoint = new CheckPoint(timeEngine.CurrentTime, transform.position, timeEngine.CurrentTime,
                timeEngine.CurrentDimensionalNode);
            checkPoints.Add(checkPoint);
        }

        if (other.gameObject.layer == 17)
        {
            StartCoroutine(CauseParadox());
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