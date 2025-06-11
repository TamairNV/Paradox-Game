using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Activator : MonoBehaviour
{
    public List<Transform> Doors = new List<Transform>();
    public bool isPressed = false;
    
    // Cache the color white to avoid creating new Color instances
    private static readonly float WhiteColor = 0.5f;
    
    // Layer masks for more efficient collision checking
    private const int PlayerLayer = 13;
    private const int BoxLayer = 12;
    private const int DimPlayer = 15;


    private void Awake()
    {
        // Combine layers into a single mask for more efficient checks

    }
    
    private List<GameObject> lines = new List<GameObject>();

    protected void Start()
    {

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        // More efficient layer check using bitwise operation
        int layer = other.gameObject.layer;
        if (layer == PlayerLayer || layer == BoxLayer || layer == DimPlayer)
        {
            isPressed = true;
            
            if (layer == PlayerLayer)
            {
                HandlePlayerInteraction(other);
            }
        }
    }
    
    
    

    private void OnTriggerExit2D(Collider2D other)
    {
        int layer = other.gameObject.layer;
        if (layer == PlayerLayer || layer == BoxLayer || layer == DimPlayer)
        {
            isPressed = false;
            ResetDoorColors();
            if (other.gameObject.layer == PlayerLayer)
            {
                foreach (GameObject line in lines)
                {
                    Destroy(line);
                }
            }
            


        }
    }
    

    private void HandlePlayerInteraction(Collider2D playerCollider)
    {
        var playerController = playerCollider.GetComponent<Player_Controller>();

        // Set camera focus objects
        
        // Process each door with its color
        print("run forest run");
        int i = 0;
        foreach (var door in Doors)
        {
            GameObject doorLine = Instantiate(playerController.doorLine);
            LineRenderer line = doorLine.GetComponent<LineRenderer>();
            line.positionCount = 2;
            line.SetPosition(0, door.transform.position);
            line.SetPosition(1, transform.position);
            lines.Add(doorLine);
            i++;
        }
    }

    private void ColorDoorAndComponents(Transform door, float color)
    {
        ColourObject(door, color);
        
        // Safely handle child objects
        if (door.childCount >= 2)
        {
            Transform child0 = door.GetChild(0);
            Transform child1 = door.GetChild(1);
            
            ColourObject(child0, color);
            ColourObject(child1, color);
            
            // Handle nested children if they exist
            if (child0.childCount > 0)
            {
                ColourObject(child0.GetChild(0), color);
                ColourObject(child1.GetChild(0), color);
            }
        }
    }



    private void ColorConnectedObjects(Door doorComponent, float color)
    {
        if (doorComponent.buttons != null)
        {
            ColourObjects(doorComponent.buttons
                .Where(button => button != null)
                .Select(button => button.transform)
                .ToList(), color);
        }
        
        if (doorComponent.PressurePlates != null)
        {
            ColourObjects(doorComponent.PressurePlates
                .Where(plate => plate != null)
                .Select(plate => plate.transform)
                .ToList(), color);
        }
    }



    private void ResetDoorColors()
    {
        foreach (var door in Doors.Where(d => d != null))
        {
            ColorDoorAndComponents(door, WhiteColor);
            
            var doorComponent = door.GetComponent<Door>();
            if (doorComponent != null)
            {
                ColorConnectedObjects(doorComponent, WhiteColor);
            }
        }
    }

    public static void ColourObjects(List<Transform> objs, float color)
    {
        if (objs == null) return;
        
        foreach (var obj in objs.Where(o => o != null))
        {
            ColourObject(obj, color);
        }
    }

    public static void ColourObject(Transform obj, float color)
    {
        if (obj == null) return;

        var renderer = obj.GetComponent<Light2D>();
        if (renderer != null)
        {
            renderer.intensity = color;
        }
    }

    public static Color GetColorFromNumber(int number)
    {
        // Use a consistent seed for the same number
        System.Random rand = new System.Random(number+4);
    
        // Generate vibrant colors
        float r = rand.Next(150, 256) / 255f;
        float g = rand.Next(150, 256) / 255f;
        float b = rand.Next(150, 256) / 255f;
    
        // Randomly select one channel to be darker
        switch (rand.Next(3))
        {
            case 0: r = rand.Next(70) / 255f; break;
            case 1: g = rand.Next(70) / 255f; break;
            case 2: b = rand.Next(70) / 255f; break;
        }
    
        return new Color(r, g, b);
    }
}