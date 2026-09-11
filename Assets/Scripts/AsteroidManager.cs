using System;
using UnityEngine;
using System.Collections.Generic;

public class AsteroidManager : MonoBehaviour
{
    public event Action<GameObject> OnAsteroidCreated;
    public event Action<GameObject> OnAsteroidDestroying;
    
    [SerializeField] private Transform shipTransform;
    [SerializeField] private Asteroid asteroidPrefab;

    [SerializeField] private float asteroidDensity = 0.1f; // Chance of asteroid spawning in a cell
    [SerializeField] private float cellSize = 16.0f;
    [SerializeField] private int cellGenerationRadius = 4;
    
    private Vector2Int _lastCell = new Vector2Int(-100, -100);
    private Dictionary<Vector2Int, Asteroid> _asteroids = new Dictionary<Vector2Int, Asteroid>();
    private HashSet<Vector2Int> _deletedAsteroids = new HashSet<Vector2Int>();
    
    private void FixedUpdate()
    {
        Vector2Int cell = new Vector2Int(
            Mathf.FloorToInt(shipTransform.position.x / cellSize),
            Mathf.FloorToInt(shipTransform.position.y / cellSize)
        );
        if (_lastCell == cell)
            return;
        
        _lastCell = cell;
        HideOldAsteroids();
        GenerateNewAsteroids();
    }

    private void HideOldAsteroids()
    {
        var keysToRemove = new List<Vector2Int>();
        foreach (var key in _asteroids.Keys)
        {
            if (IsInBounds(key))
                continue;

            keysToRemove.Add(key);
        }

        foreach (var key in keysToRemove)
        {
            HideAsteroid(key);
        }
    }

    private void GenerateNewAsteroids()
    {
        for (var x = -cellGenerationRadius; x <= cellGenerationRadius; x++)
        {
            for (var y = -cellGenerationRadius; y <= cellGenerationRadius; y++)
            {
                var cell = _lastCell + new Vector2Int(x, y);
                if (_deletedAsteroids.Contains(cell) || _asteroids.ContainsKey(cell))
                    continue;

                var random = CreateRandom(cell);
                if (!CellHasAsteroid(random))
                    continue;
                
                CreateAsteroid(random, cell);
            }
        }
    }

    private void CreateAsteroid(System.Random random, Vector2Int cell)
    {
        var position = new Vector3(
            cell.x + (float)random.NextDouble(),
            cell.y + (float)random.NextDouble()
        ) * cellSize;
        var asteroid = Instantiate(
            asteroidPrefab, 
            position,
            Quaternion.identity
        );
        asteroid.Init(cell, this);
        _asteroids.Add(cell, asteroid);
        OnAsteroidCreated?.Invoke(asteroid.gameObject);
    }
    
    private bool CellHasAsteroid(System.Random random)
    {
        return random.NextDouble() < asteroidDensity;
    }

    private System.Random CreateRandom(Vector2Int cell)
    {
        var seed = (cell.x * 73856093) ^ (cell.y * 19349663);
        return new System.Random(seed);
    }

    private bool IsInBounds(Vector2Int cell)
    {
        return Mathf.Abs(cell.x - _lastCell.x) <= cellGenerationRadius &&
               Mathf.Abs(cell.y - _lastCell.y) <= cellGenerationRadius;
    }

    private void HideAsteroid(Vector2Int cell)
    {
        var asteroid = _asteroids[cell];
        _asteroids.Remove(cell);
        OnAsteroidDestroying?.Invoke(asteroid.gameObject);
        Destroy(asteroid.gameObject);
    }

    public void DeleteAsteroid(Vector2Int cell)
    {
        _deletedAsteroids.Add(cell);
        HideAsteroid(cell);
    }
}
