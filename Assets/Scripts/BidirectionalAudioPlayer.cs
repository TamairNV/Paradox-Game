using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class BidirectionalAudioPlayer : MonoBehaviour
{
    public AudioSource audioSource;
    public AudioClip forwardClip;
    public AudioClip reversedClip;
    [FormerlySerializedAs("PlayerController")] public Player player;
    
    private int currentDirection = 1;
    private bool isPlayingForward = true;
    private float currentPosition = 0f;
    private float clipLength;
    private bool wasPlaying = false;
    
    void Start()
    {
        clipLength = forwardClip.length;

        StartCoroutine(loadAudio());
            
        audioSource.loop = true;
      
    }

    IEnumerator loadAudio()
    {
        audioSource.clip = reversedClip;
        audioSource.Play();
        yield return null;yield return null;
        audioSource.clip = forwardClip;
        audioSource.Play();

    }
    
    void Update()
    {
        // Check for direction change
        if (player.timeEngine.direction != currentDirection)
        {
            currentDirection = player.timeEngine.direction;
            TogglePlaybackDirection();
        }
        
        // Update playback position
        if (isPlayingForward)
        {
            currentPosition += Time.deltaTime;
            if (currentPosition >= clipLength)
            {
                print("sound finished");
                audioSource.Play();
                currentPosition = 0f;
                
            }
        }
        else
        {
            currentPosition -= Time.deltaTime;
            if (currentPosition <= 0)
            {
                print("sound finished");
                audioSource.Play();
                currentPosition = clipLength;
            }
        }
        

    }
    
    void UpdateAudioPosition()
    {
        // Only update position if audio is playing to prevent crackling
        if (audioSource.isPlaying)
        {
            float targetPosition = isPlayingForward ? currentPosition : (clipLength - currentPosition);
            
            // Use timeSamples for more precise positioning
            int targetSample = Mathf.FloorToInt(targetPosition * forwardClip.frequency);
            targetSample = Mathf.Clamp(targetSample, 0, audioSource.clip.samples - 1);
            
            // Only update if we've moved significantly to reduce crackling
            if (Mathf.Abs(audioSource.timeSamples - targetSample) > 10)
            {
                audioSource.timeSamples = targetSample;
            }
        }
    }
    
    public void TogglePlaybackDirection()
    {
        // Store current playback state
        wasPlaying = audioSource.isPlaying;
        
        // Pause before switching to prevent artifacts
        audioSource.Pause();
        
        // Switch direction
        isPlayingForward = !isPlayingForward;
        
        // Calculate equivalent position in new direction
        currentPosition = clipLength - currentPosition;
        currentPosition = Mathf.Clamp(currentPosition, 0, clipLength);
        
        // Switch clips
        audioSource.clip = isPlayingForward ? forwardClip : reversedClip;
        
        // Set new position (using timeSamples for precision)
        int targetSample = Mathf.FloorToInt((isPlayingForward ? currentPosition : (clipLength - currentPosition)) * forwardClip.frequency);
        targetSample = Mathf.Clamp(targetSample, 0, audioSource.clip.samples - 1);
        audioSource.timeSamples = targetSample;
        
        // Resume playback if it was playing before
        if (wasPlaying)
        {
            audioSource.Play();
        }
    }
}