using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
public class LevelPorter : MonoBehaviour
{

    [SerializeField] public int LevelNumber;
    
    private Player player;
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("player").GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        
        
        
        
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 13)
        {
            StartCoroutine(SendPlayerToLevel());
        }
    }


    public IEnumerator SendPlayerToLevel()
    {
        Vignette vignette= null;
        ChromaticAberration chromatic = null;
        FilmGrain noise= null;
        LensDistortion distortion= null;
        Bloom bloom= null; // For screen-space flare effect

        player.allowedToWalk = false;
        Volume volume = player.volume;

        // Verify all effects exist
        bool effectsFound = volume.profile.TryGet(out vignette) &&
                            volume.profile.TryGet(out chromatic) &&
                            volume.profile.TryGet(out noise) &&
                            volume.profile.TryGet(out distortion) &&
                            volume.profile.TryGet(out bloom);

        if (!effectsFound)
        {
            Debug.LogError("Missing required post-processing effects!");
            yield break;
        }
        
        
        float duration = 2f;
        float elapsed = 0f;
        float vignetteValue = 0;

        while (elapsed < duration)
        {
            float t = elapsed / duration;

         
            vignetteValue = Mathf.Lerp(0, 6f, t/4f);
            
            
            vignette.intensity.Override(vignetteValue);


            elapsed += Time.deltaTime;
            yield return null;
        }

        player.resetGame();

        player.transform.position = Level.Levels[LevelNumber].startLocation.position;
        Level.CurrentLevel = Level.Levels[LevelNumber];
        elapsed = 0;
        while (elapsed < duration)
        {
            float t = elapsed / duration;

         
            vignetteValue = Mathf.Lerp(6f, 0, t);
            vignette.intensity.Override(vignetteValue);
            
            elapsed += Time.deltaTime;
            yield return null;
        }
        vignette.intensity.Override(0);
        player.allowedToWalk = true;
        player.time = 0;
        yield return new WaitForSeconds(2f);
    }   
}
