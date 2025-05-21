using UnityEngine;
using UnityEngine.Rendering.Universal;

[RequireComponent(typeof(Light2D))]
public class HospitalLightFlicker : MonoBehaviour
{
    [Header("Main Light Settings")]
    [SerializeField] private float normalIntensity = 1f;
    [SerializeField] private Color normalColor = Color.white;

    [Header("Flicker Settings")]
    [Tooltip("How often flickers occur (in seconds)")]
    [SerializeField] private float minTimeBetweenFlickers = 5f;
    [SerializeField] private float maxTimeBetweenFlickers = 15f;
    
    [Tooltip("Duration of a flicker event")]
    [SerializeField] private float minFlickerDuration = 0.1f;
    [SerializeField] private float maxFlickerDuration = 0.5f;
    
    [Tooltip("How many quick flashes during a flicker event")]
    [SerializeField] private int minFlashesPerFlicker = 1;
    [SerializeField] private int maxFlashesPerFlicker = 4;
    
    [Header("Flicker Appearance")]
    [SerializeField] private float flickerMinIntensity = 0.2f;
    [SerializeField] private float flickerMaxIntensity = 1.5f;
    [SerializeField] private Color flickerColor = new Color(0.9f, 0.9f, 0.7f); // Slightly yellow for hospital lights
    
    [Tooltip("How fast the light transitions during flickers")]
    [SerializeField] private float flickerSpeed = 15f;

    private Light2D globalLight;
    private float nextFlickerTime;
    private bool isFlickering = false;
    private float flickerEndTime;
    private int flashesRemaining;
    private float nextFlashTime;
    private Color targetColor;
    private float targetIntensity;

    private void Awake()
    {
        globalLight = GetComponent<Light2D>();
        if (globalLight == null)
        {
            Debug.LogError("HospitalLightFlicker requires a Light2D component!");
            enabled = false;
            return;
        }
        
        ResetToNormal();
        ScheduleNextFlicker();
    }

    private void Update()
    {
        if (isFlickering)
        {
            HandleFlickering();
        }
        else if (Time.time >= nextFlickerTime)
        {
            StartFlicker();
        }
        
        // Smooth transition to target values
        globalLight.intensity = Mathf.Lerp(globalLight.intensity, targetIntensity, flickerSpeed * Time.deltaTime);
        globalLight.color = Color.Lerp(globalLight.color, targetColor, flickerSpeed * Time.deltaTime);
    }

    private void HandleFlickering()
    {
        if (Time.time >= flickerEndTime)
        {
            EndFlicker();
            return;
        }
        
        if (flashesRemaining > 0 && Time.time >= nextFlashTime)
        {
            Flash();
        }
    }

    private void StartFlicker()
    {
        isFlickering = true;
        float flickerDuration = Random.Range(minFlickerDuration, maxFlickerDuration);
        flickerEndTime = Time.time + flickerDuration;
        flashesRemaining = Random.Range(minFlashesPerFlicker, maxFlashesPerFlicker + 1);
        
        // Initial flicker
        Flash();
    }

    private void Flash()
    {
        // Random intensity for this flash
        targetIntensity = Random.Range(flickerMinIntensity, flickerMaxIntensity);
        targetColor = flickerColor;
        
        // Schedule next flash or end if done
        flashesRemaining--;
        if (flashesRemaining > 0)
        {
            float flashInterval = Random.Range(0.05f, 0.2f);
            nextFlashTime = Time.time + flashInterval;
        }
    }

    private void EndFlicker()
    {
        isFlickering = false;
        ResetToNormal();
        ScheduleNextFlicker();
    }

    private void ResetToNormal()
    {
        targetIntensity = normalIntensity;
        targetColor = normalColor;
    }

    private void ScheduleNextFlicker()
    {
        nextFlickerTime = Time.time + Random.Range(minTimeBetweenFlickers, maxTimeBetweenFlickers);
    }

    // Public methods for adjustment
    public void SetFlickerFrequency(float minInterval, float maxInterval)
    {
        minTimeBetweenFlickers = Mathf.Clamp(minInterval, 1f, maxInterval);
        maxTimeBetweenFlickers = Mathf.Max(minInterval, maxInterval);
    }

    public void SetFlickerDuration(float minDuration, float maxDuration)
    {
        minFlickerDuration = Mathf.Clamp(minDuration, 0.1f, maxDuration);
        maxFlickerDuration = Mathf.Max(minDuration, maxDuration);
    }
}