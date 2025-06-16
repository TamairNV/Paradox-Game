using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(LineRenderer))]
public class UILineRenderer : MonoBehaviour
{
    public LineRenderer lineRenderer;
    [SerializeField] private float lineMaxWidth = 400;

    [SerializeField] private float lineMinWidth = 0;
    [SerializeField] private float buffer = 50;
    private Vector3 startingPosition;
    [SerializeField] private Player player;
    public Vector3 startPosition;

    private void Start()
    {
        startingPosition = transform.position;
        startPosition = transform.position;
    }

    public void UpdateLine()
    {
        if (!Application.isPlaying && lineRenderer.positionCount != 0) return;
        if (lineRenderer.positionCount == 0)
        {return;
        }

        Vector3 lastPointWorldPos = transform.TransformPoint(lineRenderer.GetPosition(lineRenderer.positionCount - 1));
        float lastPointX = lastPointWorldPos.x;


        if (lastPointX > lineMaxWidth && player.timeEngine.direction == 1)
        {
            float overshootAmount = lastPointX - (lineMaxWidth);
            
            transform.position -= new Vector3(overshootAmount, 0, 0);;
        }

        else if (lastPointX < lineMinWidth + buffer&& player.timeEngine.direction == 0)
        {
            float undershootAmount = (lineMinWidth + buffer) - lastPointX;
    
      
            transform.position += new Vector3(undershootAmount, 0, 0);;
        }

        

    }
    

}