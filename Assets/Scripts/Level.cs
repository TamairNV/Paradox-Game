using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;
public class Level : MonoBehaviour
{

    public static Dictionary<int, Level> Levels = new Dictionary<int, Level>();
    public static Level CurrentLevel;
    [SerializeField] public int LevelNumber;
    [SerializeField] public Transform startLocation;
    [SerializeField] public float maxZoom = 2f;
    public Collider2D collider;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Levels.Add(LevelNumber,this);
        collider = GetComponent<BoxCollider2D>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool isMouseOverLevel()
    {
        Vector2  mouseWorldPos = Camera.main.ScreenToWorldPoint(Mouse.current.position.value);
        return collider.OverlapPoint(mouseWorldPos);

    }
}
