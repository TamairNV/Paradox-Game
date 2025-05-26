using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class volumeDistanceController : MonoBehaviour
{
    private Transform player;
    [SerializeField]
    public float maxDistance = 10;

    [SerializeField] public float maxVolume = 0.7f;

    private AudioSource audioSource;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        player = GameObject.Find("player").transform;
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        audioSource.volume = (1 - (distance / maxDistance)) * maxVolume;

    }
}
