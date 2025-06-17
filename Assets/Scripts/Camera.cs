using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 15f;
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float autoSpeed = 10f;
    [SerializeField] private float autoKickInTime = 1.25f;
    private float timer = 0;
    private float targetSpeed;

    private Vector3 dragOrigin;
    private bool isDragging = false;
    private Camera mainCamera;
    private float touchDistance;
    private Touchscreen touchscreen;
    private Vector3 targetPostion;
    

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        touchscreen = Touchscreen.current;
        
        if (player != null)
        {
            transform.position = new Vector3(
                player.transform.position.x,
                player.transform.position.y,
                transform.position.z
            );
        }
        
        mainCamera.orthographicSize= 1.6f;
        targetPostion = player.transform.position;
    }

    void LateUpdate()
    {
        // Handle camera movement (mouse drag or touch drag)
        HandleCameraMovement();
        
        // Handle zoom (mouse scroll or touch pinch)
        if (Level.CurrentLevel != null)
        {
            maxZoom = Level.CurrentLevel.maxZoom;
        }
        else
        {
            maxZoom = 2f;
        }

        if (mainCamera.orthographicSize > maxZoom)
        {
            mainCamera.orthographicSize = maxZoom;
        }
        HandleZoom();
    }

    private void HandleCameraMovement()
    {
        
        Vector3 target = new Vector3(player.transform.position.x, player.transform.position.y, transform.position.z);
        if (timer > autoKickInTime)
        {
            
            transform.position = Vector3.Lerp(transform.position,
                target,autoSpeed * Time.deltaTime);
        }

        if (Vector3.Distance(transform.position, target) < 0.05f)
        {
            transform.position = target;
        }
        timer += Time.deltaTime;
        return;
        // Mouse input
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            dragOrigin = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            isDragging = true;
            
        }

        if (targetPostion != null && timer < autoKickInTime)
        {
            
            Vector3 newPostion = Vector3.Lerp(transform.position, targetPostion ,10 * Time.deltaTime);
            
            if ( Level.CurrentLevel == null || Level.CurrentLevel.collider.OverlapPoint(newPostion))
            {
                transform.position = new Vector3(newPostion.x,newPostion.y,transform.position.z);
            }
        }


        
        if (Mouse.current.leftButton.isPressed && isDragging)
        {
            Vector3 difference = dragOrigin - mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            
            targetPostion = transform.position + difference;
            targetPostion = new Vector3(targetPostion.x, targetPostion.y, transform.position.z);
            timer = 0;
        }
        else
        {
            timer += Time.deltaTime;
            
        }





            
        
        
        

        if (Mouse.current.leftButton.wasReleasedThisFrame)
        {
            isDragging = false;
        }

        // Touch input (for mobile) - New Input System
        if (touchscreen != null && touchscreen.touches.Count == 1)
        {
            var touch = touchscreen.touches[0];
            if (touch.phase.ReadValue() == TouchPhase.Moved)
            {
                Vector2 touchPos = touch.position.ReadValue();
                Vector2 delta = touch.delta.ReadValue();
                Vector3 touchDelta = mainCamera.ScreenToWorldPoint(touchPos) - 
                                    mainCamera.ScreenToWorldPoint(touchPos - delta);
                transform.position -= touchDelta;
            }
        }
    }

    private void HandleZoom()
    {
        // Mouse scroll wheel zoom
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (scroll != 0)
        {
            mainCamera.orthographicSize = Mathf.Clamp(
                mainCamera.orthographicSize - scroll * zoomSpeed * mainCamera.orthographicSize,
                minZoom,
                maxZoom
            );
        }

        // Touch pinch zoom - New Input System
        if (touchscreen != null && touchscreen.touches.Count == 2)
        {
            var touchZero = touchscreen.touches[0];
            var touchOne = touchscreen.touches[1];

            var touchZeroPhase = touchZero.phase.ReadValue();
            var touchOnePhase = touchOne.phase.ReadValue();

            if (touchZeroPhase == TouchPhase.Began || touchOnePhase == TouchPhase.Began)
            {
                touchDistance = Vector2.Distance(touchZero.position.ReadValue(), touchOne.position.ReadValue());
            }
            else if (touchZeroPhase == TouchPhase.Moved || touchOnePhase == TouchPhase.Moved)
            {
                float currentDistance = Vector2.Distance(touchZero.position.ReadValue(), touchOne.position.ReadValue());
                float difference = currentDistance - touchDistance;

                mainCamera.orthographicSize = Mathf.Clamp(
                    mainCamera.orthographicSize - (difference * 0.01f * zoomSpeed * mainCamera.orthographicSize),
                    minZoom,
                    maxZoom
                );

                touchDistance = currentDistance;
            }
        }
    }
}