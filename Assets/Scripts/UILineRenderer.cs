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
    [SerializeField] private Player_Controller player;

    private void Start()
    {
        startingPosition = transform.position;
    }

    public void UpdateLine()
    {
        if (!Application.isPlaying && lineRenderer.positionCount != 0) return;

        Vector3 lastPointWorldPos = transform.TransformPoint(lineRenderer.GetPosition(lineRenderer.positionCount - 1));
        float lastPointX = lastPointWorldPos.x;


        if (lastPointX > lineMaxWidth && player.timeEngine.direction == 1)
        {
            float overshootAmount = lastPointX - (lineMaxWidth);
            float smoothSpeed = 14f; // Adjust for desired smoothness
    
            // Smoothly move the transform left
          
            transform.position = transform.position - new Vector3(overshootAmount, 0, 0);;
        }

        else if (lastPointX < lineMinWidth + buffer&& player.timeEngine.direction == 0)
        {
            float undershootAmount = (lineMinWidth + buffer) - lastPointX;
            float smoothSpeed = 14f; // Same smoothness as right boundary
    
      
            transform.position = transform.position + new Vector3(undershootAmount, 0, 0);;
        }

        

    }
    

}