using UnityEngine;

public class Entity : MonoBehaviour
{
    // Properties for entity movement.
    [Header("Movement Settings")]
    [SerializeField] float maxSpeed = 2f;
    [SerializeField] float maxForce = 5f;
    [SerializeField] float rotationSpeed = 5f;

    [Header("Boid Weights")]
    [SerializeField] float cohesionWeight = 2f;
    [SerializeField] float alignmentWeight = 1f;
    [SerializeField] float seperationWeight = 1f;

    [Header("Vision")]
    [Range(0.5f, 2f)]
    [SerializeField] float visionRadius = 1.5f;

    Vector3 velocity;
    Vector3 acceleration;

    float screenX;
    float screenY;

    public Grid grid;

    private void Start()
    {
        Camera cam = Camera.main;
        screenY = cam.orthographicSize;
        screenX = cam.aspect * cam.orthographicSize;

        velocity = Random.insideUnitCircle.normalized * maxSpeed;
        grid = FindObjectOfType<Grid>();
    }

    private void Update()
    {
        acceleration = Vector3.zero;

        // Apply boundary rules.
        ApplyBoundaryRule();

        // Check neighbours to apply rules.
        grid.CheckNeighbours(this, visionRadius);

        // Update the boids velocity.
        velocity += acceleration * Time.deltaTime;
        velocity = Vector3.ClampMagnitude(velocity, maxSpeed);

        // Move the boid based on velocity.
        transform.position += velocity * Time.deltaTime;

        // Rotate entity to face direction its moving.
        if (velocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(0, 0, angle - 90f);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Update the entity's position on the grid.
        grid.UpdateEntity(this);
    }

    public Vector3 GetAlignment()
    {
        return velocity;
    }
    public void ApplyAlignment(Vector3 averageVelocity)
    {
        if (averageVelocity != Vector3.zero)
        {
            Vector3 steeringVector = averageVelocity.normalized * maxSpeed - velocity;
            steeringVector = Vector3.ClampMagnitude(steeringVector, maxForce);
            acceleration += steeringVector * alignmentWeight;
        }
    }

    public void ApplySeperation(Vector3 averageAvoidance)
    {
        if (averageAvoidance != Vector3.zero)
        {
            Vector3 steeringVector = averageAvoidance.normalized * maxSpeed - velocity;
            steeringVector = Vector3.ClampMagnitude(steeringVector, maxForce);
            acceleration += steeringVector * seperationWeight;
        }
    }

    public void ApplyCohesion(Vector3 averagePosition)
    {
        Vector3 targetPosition = averagePosition - transform.position;
        if (targetPosition != Vector3.zero)
        {
            Vector3 steeringVector = targetPosition.normalized * maxSpeed - velocity;
            steeringVector = Vector3.ClampMagnitude(steeringVector, maxForce);
            acceleration += steeringVector * cohesionWeight;
        }
    }

    public void ApplyBoundaryRule()
    {
        Vector3 newPosition = transform.position;
        Vector3 avoidance = Vector3.zero;
        float boundaryThreshold = 2f;
        if (newPosition.x > screenX - boundaryThreshold)
        {
            avoidance.x -= 1;
        }
        if (newPosition.x < -screenX + boundaryThreshold)
        {
            avoidance.x += 1;
        }
        if (newPosition.y > screenY - boundaryThreshold)
        {
            avoidance.y -= 1;
        }
        if (newPosition.y < -screenY + boundaryThreshold)
        {
            avoidance.y += 1;
        }
        if (avoidance != Vector3.zero)
        {
            Vector3 steeringVector = avoidance.normalized * maxSpeed - velocity;
            acceleration += Vector3.ClampMagnitude(steeringVector, maxForce) * 1f;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
    }
}
