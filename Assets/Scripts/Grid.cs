using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Grid handles spatial partitioning for entities.
/// </summary>
public class Grid : MonoBehaviour
{
    // Dictionary holding cells in grid based on their position.
    Dictionary<Vector2Int, List<Entity>> cells = new Dictionary<Vector2Int, List<Entity>>();
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
                Vector2Int cellPosition = GetCell(new Vector2(x + gridOrigin.x, y + gridOrigin.y));
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
        // Get cell position based on entity's position.
        Vector2Int entityPosition = GetCell(entity.transform.position);
        // Check if the cell exists in dictioanry.
        if (cells.ContainsKey(entityPosition))
        {
            // If it does add the entity to that cell.
            cells[entityPosition].Add(entity);
            Debug.Log("Added new cell at position: " + entityPosition);
        }
        else
        {
            // If cell doesn't exist, create it.
            cells.Add(entityPosition, new List<Entity>());
        }
    }

    public void RemoveEntity(Entity entity)
    {
        // Remove an entity based on the position on grid.
        Vector2Int entityPosition = GetCell(entity.transform.position);
        if (cells.ContainsKey(entityPosition))
        {
            cells[entityPosition].Remove(entity);
        }
    }

    public void CheckNeighbours(Entity entity, float visionRadius)
    {
        int neighbourCount = 0;
        Vector3 totalAlignment = Vector3.zero;
        Vector3 totalCohesion = Vector3.zero;
        Vector3 totalSeperation = Vector3.zero;
        float distance = 0f;

        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                if (x == 0 && y == 0) continue; // skip self.
                Vector2Int neighbourCell = GetCell(entity.transform.position) + new Vector2Int(x, y);
                if (cells.ContainsKey(neighbourCell))
                {
                    List<Entity> neighbours = cells[neighbourCell];
                    foreach (var neighbour in neighbours)
                    {
                        if (entity == neighbour) continue; // skip self.
                        distance = Vector2.Distance(entity.transform.position, neighbour.transform.position);
                        if (distance <= visionRadius)
                        {
                            neighbourCount++;
                            // Calculate vectors.
                            totalAlignment += neighbour.GetAlignment();
                            totalCohesion += neighbour.transform.position;
                            totalSeperation += entity.transform.position - neighbour.transform.position;
                            
                        }
                    }
                }
            }
        }
        if (neighbourCount > 0)
        {
            // Average vectors by amount of neighbours.
            totalAlignment /= neighbourCount;
            totalCohesion /= neighbourCount;
            totalSeperation /= Mathf.Sqrt(distance);

            // Apply rules to boids.
            entity.ApplyCohesion(totalCohesion);
            entity.ApplyAlignment(totalAlignment);
            entity.ApplySeperation(totalSeperation, distance);
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
        foreach (var cell in cells)
        {
            Vector3 cellPosition = new Vector3(cell.Key.x * cellSize, cell.Key.y * cellSize, 0);
            Gizmos.DrawWireCube(cellPosition, new Vector3(cellSize, cellSize, 0));
        }
    }
}
