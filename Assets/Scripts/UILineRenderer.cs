using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LineRenderer))]
public class UILineRenderer : Graphic
{
    public LineRenderer lineRenderer;
    private Canvas canvas;
    private RectTransform canvasRect;
    public float moveSpeed = 10;
    private float movedAmount = 0;
    [SerializeField] private float normThickness = 0.2f;
    private Vector2 initialCanvasSize;
    private Vector3 initialScale;

    
    protected override void Awake()
    {
        base.Awake();
        canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();
        initialCanvasSize = canvasRect.sizeDelta;
        initialScale = canvasRect.localScale;

    }

    private void Update()
    {
        if (!Application.isPlaying) return;
    
        // Get current canvas size and scale
        Vector2 currentCanvasSize = canvasRect.sizeDelta;
        Vector3 currentScale = canvasRect.localScale;
    
        // Calculate size ratio (how much the canvas has changed)
        Vector2 sizeRatio = new Vector2(
            currentCanvasSize.x / initialCanvasSize.x,
            currentCanvasSize.y / initialCanvasSize.y
        );
    
        // Calculate scale ratio (how much the scaling has changed)
        Vector3 scaleRatio = new Vector3(
            currentScale.x / initialScale.x,
            currentScale.y / initialScale.y,
            currentScale.z / initialScale.z
        );
    
        // Combined effect - we use average of x and y to maintain aspect ratio
        float combinedEffect = (sizeRatio.x * scaleRatio.x + sizeRatio.y * scaleRatio.y) / 2f;
    
        // Apply to line renderer
        LineRenderer line = GetComponent<LineRenderer>();
        line.startWidth = normThickness * combinedEffect;
        line.endWidth = normThickness * combinedEffect;
        
        float lastPointx = lineRenderer.GetPosition(lineRenderer.positionCount - 1).x;
        
        if(lastPointx >currentCanvasSize.x-50 )
        {
            float amountOut  = currentCanvasSize.x-50 - lastPointx;
            movedAmount += amountOut;
            //lineRenderer.GetComponent<RectTransform>().position =
               // new Vector3(50 - lineRenderer.GetPosition(lineRenderer.positionCount - 1).x ,lineRenderer.GetComponent<RectTransform>().position.y,lineRenderer.GetComponent<RectTransform>().position.z );
        lineRenderer.GetComponent<RectTransform>().anchoredPosition = 
            new Vector2(amountOut, lineRenderer.GetComponent<RectTransform>().anchoredPosition.y);
      
        }

    }
    
    protected override void OnPopulateMesh(VertexHelper vh)
    {
        vh.Clear();
        
        if (lineRenderer == null || lineRenderer.positionCount == 0) return;
        
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(
                canvas.worldCamera, 
                lineRenderer.GetPosition(i));
            
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvasRect, 
                screenPoint, 
                canvas.worldCamera, 
                out Vector2 localPoint);
            
            UIVertex vertex = UIVertex.simpleVert;
            vertex.position = localPoint;
            vertex.color = lineRenderer.startColor;
            vh.AddVert(vertex);
        }
        
        for (int i = 0; i < lineRenderer.positionCount - 1; i++)
        {
            vh.AddTriangle(i, i+1, i);
        }
    }
}