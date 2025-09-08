using UnityEngine;

public class Entity : MonoBehaviour
{
    // Properties for entity movement.
    Vector3 randomDirection;
    float movementSpeed = 5f;

    [Range(0.6f, 1.5f)]
    [SerializeField] float visionRadius = 1.3f;

    // Vectors for boids simulation.
    Vector2 alignmentVector = Vector2.zero;
    Vector2 seperationVector = Vector2.zero;
    Vector2 cohesionVector = Vector2.zero;

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
            // Apply random direction to the entitys position.
            transform.position += randomDirection.normalized * movementSpeed * Time.deltaTime;
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
            grid.UpdateEntity(this);
            grid.CheckNeighbours(this, visionRadius);
        }
    }

    public Vector3 GetAlignment()
    {
        return Vector3.forward;
    }
    public void ApplyAlignment(Vector3 alignmentVector)
    {
        // Apply alignment vector to random direction.
        randomDirection = alignmentVector;
    }

    public void ApplySeperation(Entity entity)
    {
        // Apply seperation based on entity's position.
        Vector3 directionFromEntity = transform.position - entity.transform.position;
        randomDirection += directionFromEntity.normalized;
    }

    public void ApplyCohesion(Entity entity)
    {
        // Apply cohesion based on entity's position.
        Vector3 directionToEntity = entity.transform.position - transform.position;
        randomDirection += directionToEntity.normalized;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
    }
}
