using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class Rocker : MonoBehaviour
{
    public float Value = 0.5f;

    [SerializeField] private Canvas canvas;
    [SerializeField] private Image scroller;
    
    [System.Serializable]
    public class Event : UnityEvent<float> { } // Custom event class

    [SerializeField] 
    public Event onChange; // This will appear in inspector like Button's OnClick

    
    private float width;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        width = GetComponent<RectTransform>().rect.width;
        scroller.fillAmount = Value;
        onChange.Invoke(Value);
    }

    // Update is called once per frame
    void Update()
    {

        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed && IsTouchingThisUI(Touchscreen.current.primaryTouch.position.value))
        {
            Vector2 touchPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            Value = getRelativeWidthPointThing(touchPosition) / width;

            float multiple = 0.0476f;
            Value = Mathf.Round(Value / multiple) * multiple;
            scroller.fillAmount = Value;
            onChange.Invoke(Value);
        }
    }


    private float getRelativeWidthPointThing(Vector2 screenPosition)
    {
        RectTransform targetRectTransform = transform.GetComponent<RectTransform>();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            targetRectTransform, 
            screenPosition, 
            canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
            out localPoint
        );

        return localPoint.x + targetRectTransform.rect.width / 2;
    }
    
    bool IsTouchingUI(Vector2 touchPosition)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = touchPosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        return results.Count > 0;
    }
    public bool IsTouchingThisUI(Vector2 touchPosition)
    {
        RectTransform thisRectTransform = GetComponent<RectTransform>();
        
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = touchPosition;

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        
        foreach (RaycastResult result in results)
        {
            if (result.gameObject == gameObject)
            {
                return true;
            }

            if (result.gameObject.transform.IsChildOf(transform))
            {
                 return true;
            }
        }
        
        return false;
    }
}
