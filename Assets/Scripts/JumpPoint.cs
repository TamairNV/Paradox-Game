using UnityEngine;

public class JumpPoint : MonoBehaviour
{
    [SerializeField] private Vector3 directionToJump = new Vector3(0, -1);
    [SerializeField] private float speed = 3f;
    [SerializeField] private float jumpDistance = 1f;
    [SerializeField] private float upwardBoost = 0.3f; // How much the player jumps up before falling
    [SerializeField] private LayerMask playerLayer;

    private Transform player;
    private Vector3 originalPosition;
    private Vector3 peakPosition; // Highest point of the jump arc
    private Vector3 targetPosition;
    private float progress = 0f;
    private Player_Controller playerController;

    void Start()
    {
        player = GameObject.Find("player").transform;
        playerController = player.GetComponent<Player_Controller>();
    }

    void Update()
    {
        if (playerController.isJumping)
        {
            progress += Time.deltaTime * speed;
            
            // First half of progress (0-0.5): go up to peak
            if (progress < 0.5f)
            {
                float upProgress = progress * 2f; // Convert to 0-1 range
                player.position = Vector3.Lerp(originalPosition, peakPosition, upProgress);
            }
            // Second half of progress (0.5-1): fall down from peak
            else
            {
                float downProgress = (progress - 0.5f) * 2f; // Convert to 0-1 range
                player.position = Vector3.Lerp(peakPosition, targetPosition, downProgress);
            }

            if (progress >= 1f)
            {
                playerController.isJumping = false;
                player.position = targetPosition;
            }
        }
        else
        {
            
        }
    }

    void OnTriggerStay2D(Collider2D other)
    {
        if (!playerController.isJumping && other.gameObject.layer == 13)
        {
            Vector3 playerToJumpPoint = transform.position - player.position;
            float dotProduct = Vector3.Dot(playerToJumpPoint.normalized, directionToJump.normalized);
            StartJump();
        }
    }

    void StartJump()
    {
        playerController.isJumping = true;
        originalPosition = player.position;
        targetPosition = originalPosition + ((directionToJump).normalized * jumpDistance);
        
        // Calculate peak position (straight up from original position)
        peakPosition = originalPosition + (new Vector3 (0,1,0) * upwardBoost);
        
        progress = 0f;

        player.GetComponent<Player_Controller>().allowedToWalk = false;
        Invoke("EnablePlayerMovement", 1f / speed);
    }

    void EnablePlayerMovement()
    {
        player.GetComponent<Player_Controller>().allowedToWalk = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        // Draw jump path: original -> peak -> target
        Vector3 peakPos = transform.position + (Vector3.up * upwardBoost);
        Vector3 targetPos = transform.position + (directionToJump.normalized * jumpDistance);
        
        Gizmos.DrawLine(transform.position, peakPos);
        Gizmos.DrawLine(peakPos, targetPos);
        Gizmos.DrawWireCube(targetPos, Vector3.one * 0.5f);
    }
}