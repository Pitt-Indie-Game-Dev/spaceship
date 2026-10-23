using Unity.Collections;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using static PlayerShip.ControlType;

[RequireComponent (typeof (Rigidbody2D))]
public class PlayerShip : MonoBehaviour
{
    public float accelerationConstant = 5.0f;   
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
    
    private string _curPlanet;
    private bool _isTrackingMouse = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _curPlanet = null;
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public Vector2 Vel
    {
        get => _rigidbody.linearVelocity;
        private set => _rigidbody.linearVelocity = value;
    }
    Vector2 Norm => Vel.normalized; 
    Vector2 Forward => transform.rotation * Vector2.right;
    public float Mag => Vel.magnitude;
    public float Rads => _rigidbody.rotation * Mathf.Deg2Rad;

    // Update is called once per frame
    private void Update()
    {
        cameraTransform.position = transform.position + new Vector3(0, 0, cameraTransform.position.z);

        if(Keyboard.current.rKey.wasReleasedThisFrame) controlType = controlType == Jank ? MomentumEfficient : Jank;
        if(Keyboard.current.qKey.wasReleasedThisFrame) thrusters = !thrusters;
        if(Keyboard.current.eKey.wasReleasedThisFrame) pointers = !pointers;
        if(Keyboard.current.spaceKey.wasReleasedThisFrame) LandOnPlanet();
        if(Keyboard.current.fKey.isPressed) print("pew"); //guns
        if(Mouse.current.leftButton.wasPressedThisFrame) _isTrackingMouse = !_isTrackingMouse;
    }

    // FixedUpdate is called once per physics step
    private void FixedUpdate()
    {
        //TODO set offsets, tilt, and tilt costume
        if (Keyboard.current.wKey.isPressed && thrusters) Accelerate();
        else if(Keyboard.current.sKey.isPressed) Decelerate();

        if(_isTrackingMouse)
        {
            RotateTowardsMouse();
            DecreaseSpin();
        }
        
        if(controlType == MomentumEfficient) Vel = Forward * Mag;
    }

    private void Accelerate()
    {
        switch (controlType)
        {
            case MomentumEfficient:
                Vel = Mag > 0f ? Vel + Norm * Time.deltaTime * accelerationConstant * 100 / Mag : Forward; 
                break;
            case Jank:
                _rigidbody.AddForce(Forward * accelerationConstant);
                break;
        }
    }

    private void Decelerate()
    {
        switch (controlType)
        {
            //TODO: these may just be doing the same thing
            case MomentumEfficient:
                Vel = Mag < 4 ? Vector2.zero : Norm * (Mag - 3f);
                break;
            case Jank:
                Vel = Mag < 4 ? Vector2.zero
                : Vel + new Vector2(
                    Vel.x > 0 ? -3 : 3,
                    Vel.y > 0 ? -3 : 3
                );
                break;
        }
    }

    private void LandOnPlanet()
    {
        if(_curPlanet != null) LevelManager.Instance.StartLoading(_curPlanet + "Land");
    }

    private void RotateTowardsMouse()
    {
        var screenMouse = Mouse.current.position.ReadValue() - new Vector2(Screen.width, Screen.height) / 2f;

        if (screenMouse - (Vector2)transform.position != Vector2.zero)
            transform.RotateTowards(screenMouse, rotationSpeed * 100 * Time.deltaTime);
    }

    private void DecreaseSpin()
    {
        if(_rigidbody.angularVelocity != 0f) _rigidbody.angularVelocity -= 10f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Planet")
        {
            _curPlanet = other.gameObject.name;
            if(_curPlanet == "Mothership") LandOnPlanet();
        }
    }
 
    private void OnTriggerExit2D(Collider2D other)
    {
        if(other.gameObject.tag == "Planet") _curPlanet = null;
    }
}
