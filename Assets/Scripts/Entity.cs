using UnityEngine;

public class Entity : MonoBehaviour
{
    // Properties for entity movement.
    Vector3 randomDirection;
    float movementSpeed = 2f;

    [Range(0.6f, 1.5f)]
    [SerializeField] float visionRadius = 3f;

    // Vectors for boids simulation.
    Vector3 alignmentVector = Vector3.zero;
    Vector3 seperationVector = Vector3.zero;
    Vector3 cohesionVector = Vector3.zero;

    public Grid grid;

    private void Start()
    {
        randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
        grid = FindObjectOfType<Grid>();
    }

    private void Update()
    {
        if (randomDirection != Vector3.zero)
        {
            // Reset vector at the start of each frame.
            alignmentVector = Vector3.zero;
            seperationVector = Vector3.zero;
            cohesionVector = Vector3.zero;

            // Check neighbours using this entity's vision radius.
            grid.CheckNeighbours(this, visionRadius);

            Vector3 steeringVector = cohesionVector + alignmentVector + seperationVector;
            // Apply vectors to random direction and normalize.
            randomDirection = (randomDirection + steeringVector).normalized;

            // Apply movement to boid.
            transform.position += randomDirection * movementSpeed * Time.deltaTime;

            // Wrap position around screen edges.
            Vector3 newPosition = transform.position;
            if (newPosition.x > 9f) newPosition.x = -9f;
            if (newPosition.x < -9f) newPosition.x = 9f;
            if (newPosition.y > 5f) newPosition.y = -5f;
            if (newPosition.y < -5f) newPosition.y = 5f;
            transform.position = newPosition;
           
            // Get the angle in radians based on direction and convert to degree float value.
            float angle = Mathf.Atan2(randomDirection.y, randomDirection.x) * Mathf.Rad2Deg;

            // Converts float value to quaternion and apply to object's forward vector.
            transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);

            // Update the entity's position on the grid.
            grid.UpdateEntity(this);
        }
    }

    public Vector3 GetAlignment()
    {
        return randomDirection;
    }
    public void ApplyAlignment(Vector3 alignmentVector)
    {
        // Apply alignment vector to random direction.
        this.alignmentVector = alignmentVector - transform.position;
    }

    public void ApplySeperation(Vector3 seperationVector, float distance)
    {
        this.seperationVector += seperationVector;
    }

    public void ApplyCohesion(Vector3 cohesionVector)
    {
        randomDirection = cohesionVector - transform.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
    }
}
