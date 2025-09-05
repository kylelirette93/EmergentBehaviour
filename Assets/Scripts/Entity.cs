using UnityEngine;

public class Entity : MonoBehaviour
{
    float movementSpeed = 5f;
    Vector3 randomDirection;

    private void Start()
    {
        randomDirection = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), 0);
    }

    private void Update()
    {
        if (randomDirection != Vector3.zero)
        {
            transform.Translate(randomDirection * movementSpeed * Time.deltaTime);
        }
    }
}
