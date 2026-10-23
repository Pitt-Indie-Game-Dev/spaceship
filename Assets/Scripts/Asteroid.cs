using UnityEngine;

public class Asteroid : MonoBehaviour
{
    private Vector2Int _cell;
    private AsteroidManager _manager;

    public void Init(Vector2Int cell, AsteroidManager manager)
    {
        _cell = cell;
        _manager = manager;
    }

    private void OnCollisionEnter2D(Collision2D other) => _manager.DeleteAsteroid(_cell);
}
