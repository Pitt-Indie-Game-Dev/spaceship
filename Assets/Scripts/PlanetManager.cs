using System;
using UnityEngine;
using System.Collections.Generic;


public class PlanetManager : MonoBehaviour
{
    public event Action<List<GameObject>, Rigidbody2D> OnPlanetsLoaded;
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject mothership;
    public float mothershipSpeed = 5000f;

    private List<GameObject> _planets = new();
    private void Start()
    {
        _planets.Add(Instantiate(prefab, new(1000f, 500f, 0f), Quaternion.identity, transform));
        _planets.Add(Instantiate(prefab, new(-1000f, -25f, 0f), Quaternion.identity, transform));
        _planets.Add(Instantiate(prefab, new(500f, -1005f, 0f), Quaternion.identity, transform));
        _planets.Add(Instantiate(prefab, new(200f, 700f, 0f), Quaternion.identity, transform));
        var r = Instantiate(
            mothership, 
            new(UnityEngine.Random.Range(-1000,1000), UnityEngine.Random.Range(-1000,1000), 0f), 
            Quaternion.Euler(0,0,UnityEngine.Random.Range(0,360)), 
            transform
        ).GetComponent<Rigidbody2D>();
        r.linearVelocity = Utils.Radial(r.rotation, mothershipSpeed);

        OnPlanetsLoaded?.Invoke(_planets, r);
    }
}
