using UnityEngine;

public class Asteroid : MonoBehaviour
{

    [SerializeField] private float angularVelocityAddition = 360.0f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var ship = other.GetComponent<PlayerShip>();
        if (ship == null)
        {
            return;
        }
        
        var rb = other.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            return;
        }
        
        rb.angularVelocity += angularVelocityAddition;
    }
}
