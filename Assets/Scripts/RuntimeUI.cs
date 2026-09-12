using System.Collections.Generic;
using System.Collections;
using UnityEngine.UIElements;
using UnityEngine;

[RequireComponent (typeof (UIDocument))]
public class RuntimeUI : MonoBehaviour
{
    public VisualTreeAsset pointerTemplate, blipTemplate, asteroidPointerTemplate;

    [SerializeField] private AsteroidManager asteroidManager;
    [SerializeField] private PlayerShip ship;

    private Rigidbody2D _srb;

    private VisualElement _root, _pointers, _radar, _numberreadings, _velocitybars, _velocitygrid, _asteroidPointersContainer;
    private Image _compass, _xvelgrid, _yvelgrid, _rspin, _thbutton, _jankbutton, _ptrButton, dl1, dl2, dl3, dl4, dl5;
    private VectorImage onbutton, offbutton;
    private Label _x, _y, _speed, _zoom;
    
    private int _curBlink = 1;
    
    private readonly Dictionary<GameObject, AsteroidPointer> _asteroidPointers = new();
    private readonly Dictionary<GameObject, Arrow> _arrows = new();
    private readonly Dictionary<GameObject, Blip> _blips = new();

    private void Link<T>(out T x, string name) where T : VisualElement => x = _root.Q<T>(name);

    private void OnEnable()
    {
        asteroidManager.OnAsteroidCreated += HandleAsteroidCreated;
        asteroidManager.OnAsteroidDestroying += HandleAsteroidDestroying;
        
        _srb = ship.GetComponent<Rigidbody2D>();
        
        _root = GetComponent<UIDocument>().rootVisualElement;

        Link(out _speed, "speedLabel");
        Link(out _zoom, "zoomLabel");
        Link(out _x, "xposLabel");
        Link(out _y, "yposLabel");
        Link(out _asteroidPointersContainer, "PointersContainer");
        Link(out _numberreadings, "NumberReadings");
        Link(out _velocitybars, "VelocityBars");
        Link(out _velocitygrid, "VelocityGrid");
        Link(out _radar, "RadarImage");
        Link(out _ptrButton, "PointerIndicator");
        Link(out _yvelgrid, "GreenGridMarker");
        Link(out _xvelgrid, "RedGridMarker");
        Link(out _thbutton, "Thrusters");
        Link(out _rspin, "radarspin");
        Link(out _pointers, "Pointers");
        Link(out _jankbutton, "ConType");
        Link(out _compass, "Compass");
        Link(out dl1, "DL1"); dl1.SetWidthPercent (0);
        Link(out dl2, "DL2"); dl2.SetHeightPercent(0);
        Link(out dl3, "DL3"); dl3.SetWidthPercent (0);
        Link(out dl4, "DL4"); dl4.SetHeightPercent(0);
        Link(out dl5, "DL5"); dl5.SetWidthPercent (0);

        _pointers      .visible = false;
        _radar         .visible = false;
        _numberreadings.visible = false;
        _velocitybars  .visible = false;
        _compass       .visible = false;
        _velocitygrid  .visible = false;

        onbutton = Utils.LoadVector("Assets/Art/svg/spaceHUD/button/HUD_button_on");
        offbutton = Utils.LoadVector("Assets/Art/svg/spaceHUD/button/HUD_button_off");
    }    

    private void Update()
    {
        _zoom .text = "Z: " + ship.minimapZoomLevel;
        _x    .text = "X: " + ship.transform.position.x.ToString("N0");
        _y    .text = "Y: " + ship.transform.position.y.ToString("N0");
        _speed.text = "V: " + _srb.linearVelocity.magnitude.ToString("0.000");
        _rspin.style.rotate = new Rotate(new Angle(_rspin.style.rotate.value.angle.value + 1));

        if(_srb.linearVelocity.magnitude != 0)
        {
            _xvelgrid.style.left = Length.Percent(       50 + _srb.linearVelocity.x/_srb.linearVelocity.magnitude * 50  - 5); 
            _yvelgrid.style.top  = Length.Percent(100 - (50 + _srb.linearVelocity.y/_srb.linearVelocity.magnitude * 50) - 5);
        }
        else
        {
            _xvelgrid.style.left = Length.Percent(      50 - 5); 
            _yvelgrid.style.top  = Length.Percent(100 - 50 - 5);
        }
        
        _pointers.visible = ship.pointers;
        _thbutton.vectorImage = ship.thrusters ? onbutton : offbutton;
        _ptrButton.vectorImage = ship.pointers ? onbutton : offbutton;
        _jankbutton.vectorImage = ship.controlType == PlayerShip.ControlType.Jank ? onbutton : offbutton;        

        if(_curBlink < 7)
        {
            switch (_curBlink++)
            {
                case 1: StartCoroutine(Blink(_pointers))      ; break;
                case 2: StartCoroutine(Blink(_radar))         ; break;
                case 3: StartCoroutine(Blink(_numberreadings)); break;
                case 4: StartCoroutine(Blink(_velocitybars))  ; break;
                case 5: StartCoroutine(Blink(_compass))       ; break;
                case 6: StartCoroutine(Blink(_velocitygrid))  ; break;
            }
        }
        
        foreach (var a in _arrows) a.Value.Point();
        foreach (var blip in _blips) blip.Value.Update();
        foreach (var pointer in _asteroidPointers) pointer.Value.Update();

        if     (dl1.Width()  < 100) dl1.IncWidthByPercent (400 * Time.deltaTime);
        else if(dl2.Height() < 100) dl2.AddHeightPercent(400 * Time.deltaTime);
        else if(dl3.Width()  < 100) dl3.IncWidthByPercent (100 * Time.deltaTime);
        else if(dl4.Height() < 100) dl4.AddHeightPercent(400 * Time.deltaTime);
        else if(dl5.Width()  < 100) dl5.IncWidthByPercent (400 * Time.deltaTime);
    }

    private IEnumerator Blink(VisualElement v)
    {
        yield return new WaitForSeconds(0.3f*_curBlink);
        for (int i = 0; i < 3; i++)
        {
            yield return new WaitForSeconds(0.1f);
            v.visible = false;
            yield return new WaitForSeconds(0.1f);
            v.visible = true;
        }
    }

    private void HandleAsteroidCreated(GameObject asteroid)
    {
        if (_blips.ContainsKey(asteroid)) return;
        
        var blip = new Blip(asteroid.transform, ship.transform, blipTemplate, _radar);
        var pointer = new AsteroidPointer(asteroid.transform, ship.transform, asteroidPointerTemplate,
            _asteroidPointersContainer);
        _blips[asteroid] = blip;
        _asteroidPointers[asteroid] = pointer;
    }

    private void HandleAsteroidDestroying(GameObject asteroid)
    {
        if (!_blips.ContainsKey(asteroid)) return;

        var blip = _blips[asteroid];
        blip.CleanUp();
        _blips.Remove(asteroid);

        var pointer = _asteroidPointers[asteroid];
        pointer.CleanUp();
        _asteroidPointers.Remove(asteroid);
    }

    private class AsteroidPointer
    {
        private readonly Transform _target;
        private readonly Transform _ship;
        private readonly TemplateContainer _pointer;
        private readonly VisualElement _parent;
        
        private static readonly float maxDisplayDistance = 16.0f;
        private static readonly float minDisplayDistance = 5.0f;

        public AsteroidPointer(Transform target, Transform ship, VisualTreeAsset pointer, VisualElement parent)
        {
            _target = target;
            _ship = ship;
            _pointer = pointer.Instantiate();
            _parent = parent;
            _parent.Add(_pointer);

            _pointer.style.position = Position.Absolute;
            _pointer.style.width = Length.Percent(7);
            _pointer.style.height = Length.Percent(7);
        }

        public void CleanUp()
        {
            _parent.Remove(_pointer);
        }
        
        public void Update()
        {
            Vector3 d = _target.position - _ship.position;
            if (d.magnitude < minDisplayDistance || d.magnitude > maxDisplayDistance)
            {
                _pointer.visible = false;
                return;
            }

            var transparency = (d.magnitude - minDisplayDistance) / (maxDisplayDistance - minDisplayDistance);
            _pointer.style.opacity = 1 - transparency;
            
            var angle = -Mathf.Atan2(d.y, d.x);
            _pointer.style.rotate = new Rotate(Angle.Radians(angle));
            _pointer.style.left = Length.Percent((Mathf.Cos(angle) / 2.0f + 0.5f) * 100);
            _pointer.style.top = Length.Percent((Mathf.Sin(angle) / 2.0f + 0.5f) * 100);
            
            _pointer.visible = true;
        }
    }
    
    private class Blip
    {
        private readonly Transform _target;
        private readonly Transform _ship;
        private readonly TemplateContainer _blip;
        private readonly VisualElement _parent;

        private static readonly int _radarRange = 64;

        public Blip(Transform target, Transform ship, VisualTreeAsset blip, VisualElement parent)
        {
            _target = target;
            _ship = ship;
            _blip = blip.Instantiate();
            _parent = parent;
            _parent.Add(_blip);

            _blip.style.position = Position.Absolute;
            _blip.style.width = Length.Percent(10);
            _blip.style.height = Length.Percent(10);
        }

        public void CleanUp()
        {
            _parent.Remove(_blip);
        }

        public void Update()
        {
            var delta = (_target.position - _ship.position) / _radarRange;
            if (delta.magnitude >= 0.95f || !_parent.visible)
            {
                _blip.visible = false;
                return;
            }

            var positionPercentX = (delta.x / 2.0f + 0.5f);
            var positionPercentY = (1 - (delta.y / 2.0f + 0.5f));

            _blip.visible = true;
            _blip.style.left = positionPercentX * _parent.resolvedStyle.width - _blip.resolvedStyle.width / 2.0f;
            _blip.style.top = positionPercentY * _parent.resolvedStyle.height - _blip.resolvedStyle.height / 2.0f;
        }
    }

    private class Arrow
    {
        private readonly Transform _target;
        private readonly Transform _ship;
        private readonly TemplateContainer _pointer;
        private readonly VisualElement _parent;
        
        public Arrow(Transform target, Transform ship, VisualTreeAsset defaultPtr, VisualElement parent)
        {
            _target = target;
            _ship = ship;
            _parent = parent;
            parent.Add(_pointer = defaultPtr.Instantiate());
            _pointer.style.position = Position.Absolute;
            _pointer.style.right = 0;
            _pointer.style.width = Length.Percent(100);
            _pointer.style.height = Length.Percent(8);
        }

        public void CleanUp()
        {
            _parent.Remove(_pointer);
        }

        public void Point()
        {
            Vector3 d = _target.position - _ship.position;
            _pointer.style.rotate = new Rotate(Angle.Degrees(-Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg));
        }
    }
}
