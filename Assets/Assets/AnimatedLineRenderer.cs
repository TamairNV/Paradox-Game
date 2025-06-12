using UnityEngine;

public class AnimatedLineRenderer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float scrollSpeed = 1f;
    
    private Material lineMaterial;
    private Vector2 currentOffset = Vector2.zero;

    void Start()
    {
        // Get the material (create an instance if needed)
        lineMaterial = lineRenderer.material;
    }

    void Update()
    {
        // Update the texture offset
        currentOffset.x -= scrollSpeed * Time.deltaTime;
        lineMaterial.mainTextureOffset = currentOffset;
    }
}