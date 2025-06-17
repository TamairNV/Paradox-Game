using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerBoxHolder : MonoBehaviour
{
    [SerializeField] private float boxHoldLength = 2f;
    [SerializeField] private float boxHoldSpeed = 5f; // Increased for more responsive movement
    [SerializeField] private LayerMask boxLayer;
    [SerializeField] private float pickupRadius = 1f;
    
    [HideInInspector] public Vector3 boxHoldPosition;
    private Vector3 targetBoxPosition;
    private Vector3 lastValidDirection;
    
    private Player player;
    public Transform boxHolding;
    public float holdingSizeMul = 0.85f;

    void Start()
    {
        player = GetComponent<Player>();
        lastValidDirection = Vector3.right; // Default direction
    }

    void Update()
    {
        // Update direction only when there's meaningful input
        if (player.lastDirection.magnitude > 0.1f)
        {
            lastValidDirection = player.lastDirection.normalized;
        }

        GetTargetPosition();
        boxHoldPosition = Vector3.Lerp(boxHoldPosition, targetBoxPosition, Time.deltaTime * boxHoldSpeed);

        // Pickup/drop logic
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            pickUp();
        }

        if (boxHolding != null)
        {
            int t = boxHolding.GetComponent<DimentionalObjects>().Temp; 
            if (t == 1)
            {
                StartCoroutine(player.CauseParadox());
            }
            else if (t == -1)
            {
                boxHolding = null;
            }
            boxHolding.position = boxHoldPosition;
            if (boxHolding.TryGetComponent<DimentionalObjects>(out var dimObj))
            {
                boxHolding.localScale = dimObj.startingScale * holdingSizeMul;
            }
            boxHolding.GetComponent<BoxCollider2D>().enabled = false;
        }
    }

    public void pickUp()
    {
        if (boxHolding == null)
        {
            TryPickupBox();
        }
        else{
            
            boxHolding.GetComponent<BoxCollider2D>().enabled = true;
            boxHolding = null;
                
                
        }
    }

    private void TryPickupBox()
    {
        Collider2D[] nearbyBoxes = Physics2D.OverlapCircleAll(transform.position, pickupRadius, boxLayer);
        
        // Find the closest box
        Transform closestBox = null;
        float closestDistance = Mathf.Infinity;
        
        foreach (var box in nearbyBoxes)
        {
            float distance = Vector2.Distance(transform.position, box.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestBox = box.transform;
            }
        }

        if (closestBox != null && closestBox.GetComponent<DimentionalObjects>().Temp == 0)
        {
            boxHolding = closestBox;
            DimentionalObjects box = boxHolding.GetComponent<DimentionalObjects>();
            if (!box.beenInteractedWith)
            {
                box.data.Add( player.timeEngine.CurrentTime,new objData(box));
                box.beenInteractedWith = true;
            }
            // Start from current position to avoid teleportation
            boxHoldPosition = closestBox.position;
        }
        
    }

    private void GetTargetPosition()
    {
        if (boxHolding == null) return;

        // Calculate desired position
        Vector3 desiredPosition = transform.position + (Vector3)(lastValidDirection * boxHoldLength);


        targetBoxPosition = desiredPosition;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, pickupRadius);
        if (boxHolding != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, boxHolding.position);
        }
    }
}