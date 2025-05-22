using System.Collections.Generic;
using UnityEngine;

using System.Collections.Generic;
using UnityEngine;

public class Camera : MonoBehaviour
{
    [SerializeField] private Player_Controller player;
    [SerializeField] private float playerWeight = 2f; // How much more important the main player is
    [SerializeField] private float smoothSpeed = 5f; // How smoothly the camera follows
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 15f;
    [SerializeField] private float zoomPadding = 1.2f; // Extra space around players

    private List<DimentionalPlayer> dimPlayers;
    private Vector3 averagePosition;
    private float requiredZoom;

    void Start()
    {
        
    
        // Initialize camera position to player position at start
        if (player != null)
        {
            transform.position = new Vector3(
                player.transform.position.x,
                player.transform.position.y,
                transform.position.z // Maintain original z position
            );
        }
    
        // Initialize zoom
        GetComponent<UnityEngine.Camera>().orthographicSize = minZoom;
    }

    void LateUpdate()
    {
        dimPlayers = player.DimentionalPlayers;
        if (player == null || dimPlayers == null) return;

        // Calculate weighted average position
        CalculateAveragePosition();
        
        // Calculate required zoom level
        CalculateRequiredZoom();
        
        // Smoothly move camera to average position
        transform.position = Vector3.Lerp(transform.position, averagePosition, smoothSpeed * Time.deltaTime);
        
        // Smoothly adjust camera zoom (assuming orthographic camera)
        GetComponent<UnityEngine.Camera>().orthographicSize = Mathf.Lerp(
            GetComponent<UnityEngine.Camera>().orthographicSize, 
            requiredZoom, 
            smoothSpeed * Time.deltaTime
        );
    }

    void CalculateAveragePosition()
    {
        Vector3 combinedPosition = player.transform.position * playerWeight;
        float totalWeight = playerWeight;

        foreach (DimentionalPlayer dimPlayer in dimPlayers)
        {
            if (dimPlayer != null && dimPlayer.GetComponent<SpriteRenderer>().enabled && dimPlayer.gameObject.activeInHierarchy)
            {
                combinedPosition += dimPlayer.transform.position;
                totalWeight += 1f;
            }
        }

        averagePosition = combinedPosition / totalWeight;
        averagePosition.z = transform.position.z; // Maintain camera's z position
    }

    void CalculateRequiredZoom()
    {
        Bounds bounds = new Bounds(player.transform.position, Vector3.zero);
        
        // Include all active dimensional players in bounds calculation
        foreach (DimentionalPlayer dimPlayer in dimPlayers)
        {
            if (dimPlayer != null && dimPlayer.GetComponent<SpriteRenderer>().enabled  && dimPlayer.gameObject.activeInHierarchy)
            {
                bounds.Encapsulate(dimPlayer.transform.position);
            }
        }

        // Calculate required zoom based on bounds size
        float requiredSize = Mathf.Max(bounds.size.x, bounds.size.y) * 0.5f * zoomPadding;
        requiredZoom = Mathf.Clamp(requiredSize, minZoom, maxZoom);
    }
}