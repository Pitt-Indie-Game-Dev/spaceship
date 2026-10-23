using UnityEngine;

public class PlanetManager : MonoBehaviour
{
    [SerializeField] private GameObject prefab;
    [SerializeField] private GameObject mothership;
    public float mothershipSpeed = 5000f;

    private void Start()
    {
        Instantiate(prefab, new(1000f, 500f, 0f), Quaternion.identity, transform);
        Instantiate(prefab, new(-1000f, -25f, 0f), Quaternion.identity, transform);
        Instantiate(prefab, new(500f, -1005f, 0f), Quaternion.identity, transform);
        Instantiate(prefab, new(200f, 700f, 0f), Quaternion.identity, transform).name = "Metroid";
        var m = Instantiate(mothership, 
            new(Random.Range(-1000,1000), Random.Range(-1000,1000), 0f), 
            Quaternion.Euler(0,0,Random.Range(0,360)), 
            transform
        );
        var r = m.GetComponent<Rigidbody2D>();
        r.linearVelocity = Utils.Radial(r.rotation, mothershipSpeed);
        m.name = "Mothership";
    }
}
