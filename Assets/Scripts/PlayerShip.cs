using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using static UnityEngine.Mathf;
using static PlayerShip.ControlType;

[RequireComponent (typeof (Rigidbody2D))]
public class PlayerShip : MonoBehaviour
{
    public float speed = 5.0f;                  // Speed of ship at full throttle
    public float accel = 5.0f;                  // Speed of ship at full throttle
    public float rotationSpeed = 3.5f;
    public float directionAdjustSpeed = 180.0f; // How fast direction changes (degrees/s)
    public float throttleAdjustSpeed = 1.0f;    // How fast throttle changes (1.0 = 1.0 seconds from 0-100%)
    
    public int minimapZoomLevel = 1;            //TODO: ideally there'd be a HUD script for this...
    
    [SerializeField] 
    private Transform cameraTransform;
    
    private Rigidbody2D _rigidbody;
    
    public bool thrusters = true;
    public ControlType controlType = MomentumEfficient;
    public bool pointers = true;

    public enum ControlType { MomentumEfficient, Jank }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    Vector2 Vel
    {
        get => _rigidbody.linearVelocity;
        set => _rigidbody.linearVelocity = value;
    }
    Vector2 Norm => Vel.normalized; 
    Vector2 Forward => transform.rotation * Vector2.right;
    float Mag => Vel.magnitude; 

    // Update is called once per frame
    private void Update()
    {
        cameraTransform.position = new(
            transform.position.x,
            transform.position.y,
            cameraTransform.position.z
        );

        if(Keyboard.current.rKey.wasReleasedThisFrame) controlType = controlType == Jank ? MomentumEfficient : Jank;
        if(Keyboard.current.qKey.wasReleasedThisFrame) thrusters = !thrusters;
        if(Keyboard.current.fKey.isPressed) print("pew"); //guns
        if(Keyboard.current.eKey.wasReleasedThisFrame) pointers = !pointers;

        if(Mouse.current.leftButton.isPressed)
        {            
            var screenMouse = Mouse.current.position.ReadValue() - new Vector2(Screen.width, Screen.height) / 2f;

            if (screenMouse - (Vector2)transform.position != Vector2.zero)
            {   
                transform.rotation = Quaternion.Euler(0, 0,
                    MoveTowardsAngle(
                        transform.eulerAngles.z, 
                        Angle(screenMouse),
                        rotationSpeed * 100 * Time.deltaTime
                    )
                );
            }

            if(_rigidbody.angularVelocity != 0f) _rigidbody.angularVelocity -= 10f;
        }

        //if space pressed: check and land on planet
    }

    // FixedUpdate is called once per physics step
    private void FixedUpdate()
    {
        //set offsets, tilt, and tilt costume
        if (Keyboard.current.wKey.isPressed && thrusters)
        {
            switch (controlType)
            {
                case MomentumEfficient:
                    Vel = Mag > 0f ? Vel + Norm * Time.deltaTime * accel * 100 / Mag : Forward; 
                    break;
                case Jank:
                    _rigidbody.AddForce(Forward * accel);
                    break;
            }
        }
        else if(Keyboard.current.sKey.isPressed)
        {
            switch (controlType)
            {
                //TODO: these may just be doing the same thing
                case MomentumEfficient:
                    Vel = Mag < 4 ? Vector2.zero : Norm * (Mag - 3f);
                    break;
                case Jank:
                    Vel = Mag < 4 ? Vector2.zero
                    : new(
                        Vel.x > 0 ? -3 : 3,
                        Vel.y > 0 ? -3 : 3
                    );
                    break;
            }
        }

        if(controlType == MomentumEfficient) Vel = Forward * Mag;
    }

    float Angle(Vector2 v)
    {
        return Atan2(v.y,v.x) * Rad2Deg;
    }    
}
