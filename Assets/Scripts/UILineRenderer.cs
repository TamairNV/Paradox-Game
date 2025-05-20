using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LineRenderer))]
public class UILineRenderer : Graphic
{
    public LineRenderer lineRenderer;
    private Canvas canvas;
    private RectTransform canvasRect;
    
    protected override void Awake()
    {
        base.Awake();
        canvas = GetComponentInParent<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();
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