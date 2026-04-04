using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShip : MonoBehaviour
{

    public float speed = 5.0f; // Speed of ship at full throttle
    public float directionAdjustSpeed = 180.0f; // How fast direction changes from AD keys (degrees/s)
    public float throttleAdjustSpeed = 1.0f; // How fast throttle changes (1.0 = 1.0 seconds from 0-100%)

    private float _direction;
    private float _throttle;

    private Rigidbody2D _rigidbody;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // FixedUpdate is called once per physics step
    private void FixedUpdate()
    {
        if (Keyboard.current.wKey.isPressed)
            _throttle += throttleAdjustSpeed * Time.deltaTime;
        if (Keyboard.current.sKey.isPressed)
            _throttle -= throttleAdjustSpeed * Time.deltaTime;
        if (Keyboard.current.aKey.isPressed)
            _direction += directionAdjustSpeed * Time.deltaTime;
        if (Keyboard.current.dKey.isPressed)
            _direction -= directionAdjustSpeed * Time.deltaTime;
        
        _throttle = Mathf.Clamp(_throttle, 0.0f, 1.0f);
        
        _rigidbody.MoveRotation(_direction); // Smoothly rotates to _direction
        _rigidbody.linearVelocity = new Vector2( // Sets velocity based off current visual direction
            Mathf.Cos(_rigidbody.rotation * Mathf.Deg2Rad),
            Mathf.Sin(_rigidbody.rotation * Mathf.Deg2Rad)
            ) * (speed * _throttle);
    }
}
