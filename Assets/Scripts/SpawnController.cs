using System.Collections.Generic;
using UnityEngine;

public class SpawnController : MonoBehaviour
{
    [Range(0, 1000)]
    public int spawnedEntities = 0;

    List<GameObject> entities = new List<GameObject>();
    public GameObject birdPrefab;


    public void SpawnEntity()
    {
        GameObject entity = Instantiate(birdPrefab, new Vector3(Random.Range(-8f, 8f), Random.Range(-4f, 4f), 0), Quaternion.identity);
        entity.SetActive(true);
        entities.Add(entity);
    }

    private void Update()
    {
        if (entities.Count < spawnedEntities)
        {
            SpawnEntity();
        }
        else if (spawnedEntities < entities.Count)
        {
            RemoveEntity();
        }
    }
    private void RemoveEntity()
    {
        List<GameObject> newList = new List<GameObject>(entities);
        foreach (GameObject entity in entities)
        {
            if (entity != null)
            {
                entity.SetActive(false);
                newList.Remove(entity);
            }
        }
        entities = newList;
    }
}
