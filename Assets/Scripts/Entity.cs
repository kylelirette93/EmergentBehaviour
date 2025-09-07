using UnityEngine;

public class Entity : MonoBehaviour
{
    Vector3 randomDirection;
    float movementSpeed = 5f;

    [Range(0.6f, 1.5f)]
    [SerializeField] float visionRadius = 1.3f;

    private void Start()
    {
        randomDirection = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0);
    }

    private void Update()
    {
        if (randomDirection != Vector3.zero)
        {
            // Apply random direction to the entitys position.
            transform.position += randomDirection.normalized * movementSpeed * Time.deltaTime;
            // Get the angle in radians based on direction and convert to degree float value.
            float angle = Mathf.Atan2(randomDirection.y, randomDirection.x) * Mathf.Rad2Deg;
            // Converts float value to quaternion and apply to object's forward vector.
            transform.rotation = Quaternion.AngleAxis(angle - 90f, Vector3.forward);

        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, visionRadius);
    }
}
