using System;
using System.Collections;
using UnityEngine;
using static UnityEngine.Mathf;

public class EnemyShip : MonoBehaviour
{
    float velocity = 0f;
    float distanceToPlayer = 0f;
    float playerVelocity = 0f;

    // These can be changed based on type of ship
    // The distance at which the enemy will try to follow the player at
    float followDistance = 5f;
    // Range at which enemy will pursue the player
    // Also affects the speed of the curve
    // Lower means it enters optimal follow distance faster
    float maxRange = 80f;
    // Determines turning radius
    float turnSpeed = 100f;
    // Fastest the enemy can move
    float maxSpeed = 20f;
    // How fast the enemy circles around the player at a standstill
    float circlingSpeed = 2f;

    public GameObject playerShip;
    private PlayerShip pShip;

    private Rigidbody2D _rigidbody;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Start()
    {
        // TODO: object instantiation (or possibly pooling?)
        // Bottom line: make more than one enemy
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.linearVelocity = Vector2.right;
        pShip = playerShip.GetComponent<PlayerShip>();
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = (transform.position - playerShip.transform.position).magnitude;
        playerVelocity = pShip.getVelocity();

        if (distanceToPlayer <= maxRange) {
            transform.RotateTowards(playerShip.transform, turnSpeed);
            velocity = Math.Clamp((float)Math.Pow(distanceToPlayer - followDistance, 3f) / (maxRange / 10f) + playerVelocity, 0f, maxSpeed);
        } else {
            // TODO: despawn the ship here
            // For now, it just stops moving
            velocity = 0f;
        }

        if (Math.Abs(distanceToPlayer - followDistance) <= 1f & playerVelocity == 0f) {
            _rigidbody.linearVelocity = transform.rotation * (Vector2.right * velocity + Vector2.down * circlingSpeed * (1 - Math.Abs(distanceToPlayer - followDistance)));
        } else {
            _rigidbody.linearVelocity = transform.rotation * Vector2.right * velocity;
        }
    }
}