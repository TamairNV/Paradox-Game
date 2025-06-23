using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Activator : MonoBehaviour
{
    [SerializeField] private Material lineMat;
    [SerializeField] private Gradient boltColour;
    [SerializeField] private float boltWidth = 0.07f;
    [SerializeField] private float numBoltsPerUnit = 1;

    private Transform activeLine;
    public bool isPressed = false;
    
    
    // Layer masks for more efficient collision checking
    private const int PlayerLayer = 13;
    private const int BoxLayer = 15;
    private const int DimPlayer = 12;
    private Player player;

    private void Awake()
    {
        // Combine layers into a single mask for more efficient checks

    }
    
    private List<GameObject> activeLines = new List<GameObject>();

    protected void Start()
    {
        player = GameObject.Find("player").GetComponent<Player>();
        if (transform.childCount == 0)
        {
            return;
        }


        for (int i = 0; i < transform.childCount; i++)
        {
            activeLine = transform.GetChild(i);
            Transform activeLineBolt = activeLine.GetChild(0);
            LineRenderer lineRenderer = CopyPropertiesTo(activeLine.GetComponent<LineRenderer>(), activeLineBolt);
            lineRenderer.material = new Material(lineMat);
            lineRenderer.material.SetFloat("_Number_of_Dashes",CalculateTotalLength(activeLineBolt.GetComponent<LineRenderer>()) * numBoltsPerUnit);
            lineRenderer.colorGradient = boltColour;
            lineRenderer.startWidth = boltWidth;
            lineRenderer.sortingLayerName = "Line";
            activeLine.transform.GetChild(0).gameObject.SetActive(false);
            activeLines.Add(activeLine.gameObject);
            
            
        }

        

    }
    public static LineRenderer CopyPropertiesTo( LineRenderer source, Transform targetTransform)
    {
        if (source == null || targetTransform == null) return null;
        LineRenderer target = targetTransform.gameObject.AddComponent<LineRenderer>();
        // Basic settings
        target.positionCount = source.positionCount;
        target.loop = source.loop;
        target.startWidth = source.startWidth;
        target.endWidth = source.endWidth;
        target.widthCurve = source.widthCurve;
        target.widthMultiplier = source.widthMultiplier;
        target.startColor = source.startColor;
        target.endColor = source.endColor;
        target.colorGradient = source.colorGradient;
        target.numCapVertices = source.numCapVertices;
        target.alignment = source.alignment;
        target.textureMode = source.textureMode;
        target.generateLightingData = source.generateLightingData;
        target.shadowBias = source.shadowBias;

        // Materials and rendering
        target.materials = source.materials;
        target.lightProbeUsage = source.lightProbeUsage;
        target.receiveShadows = source.receiveShadows;
        target.shadowCastingMode = source.shadowCastingMode;
        target.motionVectorGenerationMode = source.motionVectorGenerationMode;
        target.allowOcclusionWhenDynamic = source.allowOcclusionWhenDynamic;
        target.sortingLayerID = source.sortingLayerID;
        target.sortingOrder = source.sortingOrder;

        // Positions
        Vector3[] positions = new Vector3[source.positionCount];
        source.GetPositions(positions);
        target.SetPositions(positions);
        return target;
    }
    public static float CalculateTotalLength(LineRenderer lineRenderer)
    {
        if (lineRenderer == null || lineRenderer.positionCount < 2)
            return 0f;

        float totalLength = 0f;
        Vector3[] positions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(positions);

        for (int i = 1; i < positions.Length; i++)
        {
            totalLength += Vector3.Distance(positions[i - 1], positions[i]);
        }

        return totalLength;
    }


    

    private void OnTriggerStay2D(Collider2D other)
    {
                
        // More efficient layer check using bitwise operation
        int layer = other.gameObject.layer;
        if (layer == PlayerLayer || layer == BoxLayer || layer == DimPlayer)
        {
            isPressed = true;

            foreach (GameObject line in activeLines)
            {
                line.transform.GetChild(0).gameObject.SetActive(true);
            }
            

        }

    }


    private void OnTriggerExit2D(Collider2D other)
    {
        int layer = other.gameObject.layer;
        if (layer == PlayerLayer || layer == BoxLayer || layer == DimPlayer)
        {
            isPressed = false;
         

            foreach (GameObject line in activeLines)
            {
                line.transform.GetChild(0).gameObject.SetActive(false);
            }
            

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