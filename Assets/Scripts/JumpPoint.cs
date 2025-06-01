using UnityEngine;

public class JumpPoint : MonoBehaviour
{
    [SerializeField] private Vector3 directionToJump = new Vector3(0, -1);
    [SerializeField] private float speed = 3f;
    [SerializeField] private float jumpDistance = 1f;
    [SerializeField] private float upwardBoost = 0.3f;

    private Transform player;
    private Player_Controller playerController;
    private static JumpPoint activeJumpPoint; // Track which jump point is currently active

    void Start()
    {
        player = GameObject.Find("player").transform;
        playerController = player.GetComponent<Player_Controller>();
    }

    void Update()
    {
        // Only update if this is the active jump point
        if (activeJumpPoint == this && playerController.isJumping)
        {
            playerController.timeEngine.CurrentDimensionalNode.data.Jumped = true;
            playerController.jumpProgress += Time.deltaTime * speed;
            float progress = playerController.jumpProgress;


         
            // Coming down from peak
            float downProgress = (progress - 0.5f) * 2f;
            player.position = Vector3.Lerp(playerController.jumpPeakPosition, 
                                         playerController.jumpTargetPosition, 
                downProgress);
        

            if (progress >= 1f)
            {
                playerController.isJumping = false;
                player.position = playerController.jumpTargetPosition;
                activeJumpPoint = null;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == 13 && !playerController.isJumping)
        {
            // Only trigger if no other jump point is active
            if (activeJumpPoint == null || activeJumpPoint == this)
            {
                activeJumpPoint = this;
                StartJump();
            }
        }
    }

    void StartJump()
    {
        playerController.isJumping = true;
        playerController.jumpProgress = 0f;
        playerController.jumpStartPosition = player.position;
        playerController.jumpTargetPosition = player.position + (directionToJump.normalized * jumpDistance);
        playerController.jumpPeakPosition = player.position + (Vector3.up * upwardBoost);
        playerController.allowedToWalk = false;

        // Automatically re-enable walking after jump duration
        float jumpDuration = 1f / speed;
        Invoke("EnablePlayerMovement", jumpDuration);
    }

    void EnablePlayerMovement()
    {

        playerController.allowedToWalk = true;
        activeJumpPoint = null;
        
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Vector3 peakPos = transform.position + (Vector3.up * upwardBoost);
        Vector3 targetPos = transform.position + (directionToJump.normalized * jumpDistance);
        
        Gizmos.DrawLine(transform.position, peakPos);
        Gizmos.DrawLine(peakPos, targetPos);
        Gizmos.DrawWireCube(targetPos, Vector3.one * 0.5f);
    }
}