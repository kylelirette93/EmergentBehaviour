using UnityEngine;

public class Entity : MonoBehaviour
{
    // Properties for entity movement.
    [Header("Movement Settings")]
    [SerializeField] float maxSpeed = 2f;
    [SerializeField] float maxForce = 5f;
    [SerializeField] float rotationSpeed = 5f;

    [Header("Properties for boid behaviour")]
    public float CohesionWeight { get { return cohesionWeight; } set { cohesionWeight = value; } }
    public float SeperationWeight { get { return seperationWeight; } set { seperationWeight = value; } }
    public float AlignmentWeight { get { return alignmentWeight; } set { alignmentWeight = value; } }

    [Header("Boid Weights")]
    [SerializeField] float cohesionWeight = 2f;
    [SerializeField] float alignmentWeight = 1f;
    [SerializeField] float seperationWeight = 1f;

    [Header("Vision")]
    [Range(0.5f, 2f)]
    [SerializeField] float influenceRadius = 1.5f;

    [Header("Noise")]
    float noiseScale = 0.1f;
    Vector2 noiseOffset;

    Vector3 velocity;
    Vector3 acceleration;

    float screenX;
    float screenY;

    public Grid grid;

    private void Start()
    {
        // Calculate screen bounds based on camera.
        Camera cam = Camera.main;
        screenY = cam.orthographicSize;
        screenX = cam.aspect * screenY;

        velocity = Random.insideUnitCircle.normalized * maxSpeed;
        grid = FindFirstObjectByType<Grid>();
    }

    private void Update()
    {
        acceleration = Vector3.zero;

        // Apply boundary rules.
        ApplyBoundaryRule();

        // Check neighbours to apply rules.
        grid.CheckNeighbours(this, influenceRadius);

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
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 0.1f);
        }

        // Update the entity's position on the grid.
        grid.UpdateEntity(this);
    }

    /// <summary>
    /// Returns the current velocity to calculate alignment.
    /// </summary>
    /// <returns></returns>
    public Vector3 GetAlignment()
    {
        return velocity;
    }
    /// <summary>
    /// Create a steering vector based on average velocity of neighbours.
    /// </summary>
    /// <param name="averageVelocity">The average velocity of neighbours.</param>
    /// 
    public void ApplyAlignment(Vector3 averageVelocity)
    {
        if (averageVelocity != Vector3.zero)
        {
            Vector3 desiredVelocity = averageVelocity.normalized * maxSpeed;
            Vector3 steeringVector = desiredVelocity - velocity;
            steeringVector = Vector3.ClampMagnitude(steeringVector, maxForce);
            acceleration += steeringVector * alignmentWeight;
        }
    }
    /// <summary>
    /// Creates a steering vector for avoidance based on average avoidance of neighbours.
    /// </summary>
    /// <param name="averageAvoidance"></param>
    public void ApplySeperation(Vector3 averageAvoidance)
    {
        if (averageAvoidance != Vector3.zero)
        {
            Vector3 steeringVector = Vector3.ClampMagnitude(averageAvoidance, maxForce);
            acceleration += steeringVector * seperationWeight;
        }
    }

    /// <summary>
    /// Creates a steering vector towards center of mass of neighbours.
    /// </summary>
    /// <param name="averagePosition"></param>
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
        float boundaryThreshold = 0.5f;
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
            steeringVector = Vector3.ClampMagnitude(steeringVector, maxForce);
            acceleration += steeringVector * 2f;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, influenceRadius);
    }
}
