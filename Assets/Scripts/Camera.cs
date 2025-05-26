using System;
using UnityEngine;
using UnityEngine.InputSystem;
using TouchPhase = UnityEngine.InputSystem.TouchPhase;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Player_Controller player;
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 15f;
    [SerializeField] private float zoomSpeed = 2f;
    [SerializeField] private float dragSpeed = 2f;
    [SerializeField] private float autoSpeed = 10f;
    [SerializeField] private float easeSpeed = 0.5f;
    [SerializeField] private float easeEaseSpeed = 1f;
    [SerializeField] private float autoKickInTime = 1.25f;
    private float timer = 0;
    private float targetSpeed;

    private Vector3 dragOrigin;
    private bool isDragging = false;
    private Camera mainCamera;
    private float touchDistance;
    private Touchscreen touchscreen;
    

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
        
        mainCamera.orthographicSize = minZoom;
    }

    void LateUpdate()
    {
        // Handle camera movement (mouse drag or touch drag)
        HandleCameraMovement();
        
        // Handle zoom (mouse scroll or touch pinch)
        HandleZoom();
    }

    private void HandleCameraMovement()
    {
        // Mouse input
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            dragOrigin = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            isDragging = true;
        }

        float speed=0;
        if (Mouse.current.leftButton.isPressed && isDragging)
        {
            Vector3 difference = dragOrigin - mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            transform.position += difference;
            
            timer = 0;
        }
        else
        {
            timer += Time.deltaTime;
            
        }



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