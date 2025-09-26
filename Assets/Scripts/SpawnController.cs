using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Spawn Controller manages spawning and despawning of entities based on a UI slider.
/// </summary>
public class SpawnController : MonoBehaviour
{
    public int spawnedEntities = 0;
    public TextMeshProUGUI spawnedText;
    public Slider spawnSlider;

    List<GameObject> entities = new List<GameObject>();
    public GameObject birdPrefab;

    
    private void Start()
    {
        // Add listener to update target spawn count when value changes.
        spawnSlider.onValueChanged.AddListener(UpdateSpawnTarget);

        spawnedEntities = Mathf.RoundToInt(spawnSlider.value);
    }

    /// <summary>
    /// Update target spawn count based on slider value.
    /// </summary>
    /// <param name="newValue"></param>
    public void UpdateSpawnTarget(float newValue)
    {
        spawnedEntities = Mathf.RoundToInt(newValue);
    }

    private void UpdateSpawnText()
    {
        spawnedText.text = "Spawned Entities: " + entities.Count + "/ " + spawnedEntities;
    }
    /// <summary>
    /// Spawn an entity, add to a list and update UI.
    /// </summary>
    public void SpawnEntity()
    {
        GameObject entity = Instantiate(birdPrefab, new Vector3(Random.Range(-8f, 8f), Random.Range(-4f, 4f), 0), Quaternion.identity);
        entity.SetActive(true);
        entities.Add(entity);
        UpdateSpawnText();
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
    /// <summary>
    /// Remove the last spawned entity from the scene and the list.
    /// </summary>
    private void RemoveEntity()
    {
        int lastIndex = entities.Count - 1;

        GameObject entityToRemove = entities[lastIndex];

        if (entityToRemove != null)
        {
            Destroy(entityToRemove);
        }

        entities.RemoveAt(lastIndex);

        UpdateSpawnText();
    }
}
