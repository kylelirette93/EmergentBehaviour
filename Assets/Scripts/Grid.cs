using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Grid handles spatial partitioning for entities.
/// </summary>
public class Grid : MonoBehaviour
{
    // Dictionary holding cells in grid based on their position.
    Dictionary<Vector2Int, List<Entity>> cells = new Dictionary<Vector2Int, List<Entity>>();
    Dictionary<Entity, Vector2Int> currentCell = new Dictionary<Entity, Vector2Int>();
    int cellSize = 1;
    Vector2 gridOrigin = new Vector2(-8, -4);

    private void Start()
    {
        CreateGrid(18, 12);
    }

    /// <summary>
    /// Create grid of empty cells and entities.
    /// </summary>
    /// <param name="width">Width the grid is created from.</param>
    /// <param name="height">Height the grid is created from.</param>
    public void CreateGrid(int width, int height)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // Offset cell position by grid origin.
                Vector2Int cellPosition = GetCell(new Vector2(x + (int)gridOrigin.x, y + (int)gridOrigin.y));
                cells.Add(cellPosition, new List<Entity>());
            }
        }
    }

    /// <summary>
    /// Add's an entity to the grid based on position.
    /// </summary>
    /// <param name="entity">The entity being passed to the grid.</param>
    public void UpdateEntity(Entity entity)
    {
        Vector2Int newCell = GetCell(entity.transform.position);

        // Check if the old cell contains entity and remove it.
        if (currentCell.TryGetValue(entity, out Vector2Int oldCell))
        {
            if (oldCell != newCell)
            {
                if (cells.ContainsKey(oldCell))
                {
                    // Remove entity from old cell.
                    cells[oldCell].Remove(entity);
                }
            }
        }
        if (cells.ContainsKey(newCell))
        {
            if (!cells[newCell].Contains(entity))
            {
                cells[newCell].Add(entity);
            }
        }

        // Update the entity's current cell.
        currentCell[entity] = newCell;
    }

    /// <summary>
    /// Removes an entity from a cell.
    /// </summary>
    /// <param name="entity"></param>
    public void RemoveEntity(Entity entity)
    {
        if (currentCell.TryGetValue(entity, out Vector2Int cell))
        {
            if (cells.ContainsKey(cell))
            {
                cells[cell].Remove(entity);
            }
            currentCell.Remove(entity);
        }
    }

    /// <summary>
    /// Check for neighbours and make calculations based on boids.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="visionRadius"></param>
    public void CheckNeighbours(Entity entity, float visionRadius)
    {
        int neighbourCount = 0;
        Vector3 totalAlignment = Vector3.zero;
        Vector3 totalSeperation = Vector3.zero;
        Vector3 centerOfMass = Vector3.zero;
        float distance = 0f;

        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                Vector2Int neighbourCell = GetCell(entity.transform.position) + new Vector2Int(x, y);
                if (cells.ContainsKey(neighbourCell))
                {
                    List<Entity> neighbours = cells[neighbourCell];
                    foreach (var neighbour in neighbours)
                    {
                        if (entity == neighbour) continue; 
                        distance = Vector2.Distance(entity.transform.position, neighbour.transform.position);
                        if (distance < visionRadius)
                        {
                            neighbourCount++;

                            // Average neighbours velocities.
                            totalAlignment += neighbour.GetAlignment();

                            centerOfMass += neighbour.transform.position;

                            // Inverse square law so closer boids have more influence.
                            totalSeperation += (entity.transform.position - neighbour.transform.position);
                            
                        }
                    }
                }
            }
        }
        if (neighbourCount > 0)
        {
            // Direction towards the center of mass.
            centerOfMass /= neighbourCount;
            Vector3 cohesionDirection = (centerOfMass - entity.transform.position);

            // Average alignment of flock.
            totalAlignment /= neighbourCount;

            // Apply rules to boids.
            entity.ApplyCohesion(cohesionDirection);
            entity.ApplyAlignment(totalAlignment);
            entity.ApplySeperation(totalSeperation);
        }
    }

    

    /// <summary>
    /// Converts a vector2 position to a vector2 int(cell position).
    /// </summary>
    /// <param name="position">The vector2 being passed.</param>
    /// <returns></returns>
    public Vector2Int GetCell(Vector2 position)
    {
        // Return the vector as an integer.
        return new Vector2Int(Mathf.FloorToInt(position.x), Mathf.FloorToInt(position.y));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        foreach (var cell in currentCell)
        {
            Vector3 cellPosition = new Vector3(cell.Value.x * cellSize, cell.Value.y * cellSize, 0);
            Gizmos.DrawWireCube(cellPosition, new Vector3(cellSize, cellSize, 0));
        }
    }
}
