using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Grid handles spatial partitioning for entities.
/// </summary>
public class Grid : MonoBehaviour
{
    // Dictionary holding list of entities in each cell on grid.
    Dictionary<Vector2Int, List<Entity>> cells = new Dictionary<Vector2Int, List<Entity>>();
    // Dictionary holding current cell position of each entity.
    Dictionary<Entity, Vector2Int> currentCell = new Dictionary<Entity, Vector2Int>();

    int cellSize = 1;
    Vector2 gridOrigin = new Vector2(-8, -4);
    int gridHeight = 18;
    int gridWidth = 12;

    private void Start()
    {
        CreateGrid(gridHeight, gridWidth);
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
    /// Update an entity in grid based on its current position.
    /// </summary>
    /// <param name="entity">The entity being passed to the grid.</param>
    public void UpdateEntity(Entity entity)
    {
        Vector2Int newCell = GetCell(entity.transform.position);

        // If the old cell isnt the current cell position, remove it from grid.
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
        // Add entity to grid based on current cell position.
        if (cells.ContainsKey(newCell))
        {
            // Avoid adding twice.
            if (!cells[newCell].Contains(entity))
            {
                cells[newCell].Add(entity);
            }
        }

        // Add the entitys current cell position.
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
    /// Check for neighbours and make calculations based on neighbouring boids.
    /// </summary>
    /// <param name="entity"></param>
    /// <param name="influenceRadius">The radius of influence. Determines if an entity is influenced by a neighbour.</param>
    public void CheckNeighbours(Entity entity, float influenceRadius)
    {
        int neighbourCount = 0;
        Vector3 totalAlignment = Vector3.zero;
        Vector3 totalSeperation = Vector3.zero;
        Vector3 centerOfMass = Vector3.zero;
        float distanceFromNeighbour = 0f;

        for (int y = -1; y <= 1; y++)
        {
            for (int x = -1; x <= 1; x++)
            {
                // Check all neighbouring cells, including current.
                Vector2Int neighbourCell = GetCell(entity.transform.position) + new Vector2Int(x, y);
                if (cells.ContainsKey(neighbourCell))
                {
                    List<Entity> neighbours = cells[neighbourCell];

                    foreach (var neighbour in neighbours)
                    {
                        // We're not checking neighbours here.
                        if (entity == neighbour) continue;

                        if (entity != null && neighbour == null) continue;

                        distanceFromNeighbour = Vector2.Distance(entity.transform.position, neighbour.transform.position);
                        Vector3 toNeighbour = neighbour.transform.position - entity.transform.position;
                        Vector3 forward = entity.transform.up;
                        float angle = Vector3.Angle(forward, toNeighbour);

                        if (distanceFromNeighbour < influenceRadius && angle < 120f)
                        {
                            neighbourCount++;
                            //Debug.Log("Entity at: " + GetCell(entity.transform.position) + "has " + neighbourCount + "neighbours.");

                            // Add together neighbours velocity.
                            totalAlignment += neighbour.GetAlignment();

                            // Add together neighbours positions.
                            centerOfMass += neighbour.transform.position;

                            // Total seperation from neighbours.
                            Vector3 seperationVector = entity.transform.position - neighbour.transform.position;
                            if (seperationVector.sqrMagnitude > 0.01f)
                            {
                                // Seperate based on distance from neighbour.
                                totalSeperation += seperationVector.normalized / distanceFromNeighbour;
                            }
                        }
                    }
                }
            }
        }
        if (neighbourCount > 0)
        {
            // Calculate average center of mass of flock.
            centerOfMass /= neighbourCount;

            // Calculate average alignment of flock.
            totalAlignment /= neighbourCount;

            // Apply rules to the boid.
            entity.ApplyCohesion(centerOfMass);
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
