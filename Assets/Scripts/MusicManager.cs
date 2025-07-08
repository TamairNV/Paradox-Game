using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class MusicManager : MonoBehaviour
{

    [SerializeField] private AudioMixer mixer;
    private const string MASTER_VOL = "Master";
    private const string MUSIC_VOL = "Music";
    private const string SFX_VOL = "Sound Effects";
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(TurnListenerOn());
    }

    IEnumerator TurnListenerOn()
    {
        GetComponent<AudioListener>().enabled = false;
        yield return new WaitForSeconds(1.2f);
        GetComponent<AudioListener>().enabled = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void updateSoundEffectVolume(float volume)
    {
        float dB = volume > 0 ? 20f * Mathf.Log10(volume) : -80f;
        mixer.SetFloat(SFX_VOL, dB);
    }
    public void updateMasterVolume(float volume)
    {
        float dB = volume > 0 ? 20f * Mathf.Log10(volume) : -80f;
        mixer.SetFloat(MASTER_VOL, dB);
    }
    public void updateMusicVolume(float volume)
    {
        float dB = volume > 0 ? 20f * Mathf.Log10(volume) : -80f;
        mixer.SetFloat(MUSIC_VOL, dB);
    }
    
}
