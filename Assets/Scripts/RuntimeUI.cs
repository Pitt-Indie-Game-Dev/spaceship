using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using System.Security.AccessControl;
using UnityEditor;

[RequireComponent (typeof (UIDocument))]
public class RuntimeUI : MonoBehaviour
{
    public VisualTreeAsset pointerTemplate;

    [SerializeField] 
    private PlayerShip ship;
    private Rigidbody2D _srb;

    private Label _x;
    private Label _y;
    private Label _speed;
    private Label _zoom;
    private VisualElement _pointers;
    private VisualElement _radar;
    private VisualElement _numberreadings;
    private VisualElement _velocitybars;
    private VisualElement _velocitygrid;
    private Image _compass;
    private Image _xvelgrid;
    private Image _yvelgrid;
    private Image _rspin;
    private Image _thbutton;
    private Image _jankbutton;
    private Image _ptrButton;

    private VectorImage onbutton;
    private VectorImage offbutton;
    
    private float _startTime;
    private int _curBlink = 1;

    private readonly List<Arrow> _arrows = new();
    private List<Image> divisionLine = new();

    private void OnEnable()
    {
        _srb = ship.GetComponent<Rigidbody2D>();
        
        VisualElement r = GetComponent<UIDocument>().rootVisualElement;
        _zoom  = r.Q<Label>("zoomLabel");
        _x     = r.Q<Label>("xposLabel");
        _y     = r.Q<Label>("yposLabel");
        _speed = r.Q<Label>("speedLabel");
        _rspin = r.Q<Image>("radarspin");

        _pointers       = r.Q("Pointers");
        _radar          = r.Q("Radar");
        _numberreadings = r.Q("NumberReadings");
        _velocitybars   = r.Q("VelocityBars");
        _compass        = r.Q<Image>("Compass");
        _velocitygrid   = r.Q("VelocityGrid");
        _xvelgrid       = r.Q<Image>("RedGridMarker");
        _yvelgrid       = r.Q<Image>("GreenGridMarker");
        _thbutton       = r.Q<Image>("Thrusters");
        _jankbutton     = r.Q<Image>("ConType");
        _ptrButton      = r.Q<Image>("PointerIndicator");

        _pointers      .visible = false;
        _radar         .visible = false;
        _numberreadings.visible = false;
        _velocitybars  .visible = false;
        _compass       .visible = false;
        _velocitygrid  .visible = false;

        onbutton = LoadVector("on");
        offbutton = LoadVector("off");

        divisionLine.Add(r.Q<Image>("DL1")); SetWidth (divisionLine[0], Length.Percent(0));
        divisionLine.Add(r.Q<Image>("DL2")); SetHeight(divisionLine[1], Length.Percent(0));
        divisionLine.Add(r.Q<Image>("DL3")); SetWidth (divisionLine[2], Length.Percent(0));
        divisionLine.Add(r.Q<Image>("DL4")); SetHeight(divisionLine[3], Length.Percent(0));
        divisionLine.Add(r.Q<Image>("DL5")); SetWidth (divisionLine[4], Length.Percent(0));

        _startTime = Time.time;
    }

    VectorImage LoadVector(string fileName)
    {
        var guid = AssetDatabase.FindAssets(fileName + " t:VectorImage", new[] { "Assets/OneDrive/svg/spaceHUD/button" })[0];
        return AssetDatabase.LoadAssetAtPath<VectorImage>(AssetDatabase.GUIDToAssetPath(guid));
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
        
        foreach (var a in _arrows) a.Point();

        if     (Width (divisionLine[0]) < 100) IncWidth (divisionLine[0], 400 * Time.deltaTime);
        else if(Height(divisionLine[1]) < 100) IncHeight(divisionLine[1], 400 * Time.deltaTime);
        else if(Width (divisionLine[2]) < 100) IncWidth (divisionLine[2], 100 * Time.deltaTime);
        else if(Height(divisionLine[3]) < 100) IncHeight(divisionLine[3], 400 * Time.deltaTime);
        else if(Width (divisionLine[4]) < 100) IncWidth (divisionLine[4], 400 * Time.deltaTime);
    }

    private float Height(VisualElement v) { return v.style.height.value.value; }
    private float Width(VisualElement v) { return v.style.width.value.value; }
    private void SetHeight(VisualElement v, Length a) { v.style.height = a; }
    private void SetWidth(VisualElement v, Length a) { v.style.width = a; }
    private void IncHeight(VisualElement v, float a) { SetHeight(v, Length.Percent(Mathf.Min(Height(v) + a,100))); }
    private void IncWidth(VisualElement v, float a) { SetWidth(v, Length.Percent(Mathf.Min(Width(v) + a,100))); }

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

    private class Arrow
    {
        private readonly Transform _target;
        private readonly Transform _ship;
        private readonly TemplateContainer _pointer;
        
        public Arrow(Transform target, Transform ship, VisualTreeAsset defaultPtr, VisualElement parent)
        {
            _target = target;
            _ship = ship;
            parent.Add(_pointer = defaultPtr.Instantiate());
            _pointer.style.position = Position.Absolute;
            _pointer.style.right = 0;
            _pointer.style.width = Length.Percent(100);
            _pointer.style.height = Length.Percent(8);
        }

        public void Point()
        {
            Vector3 d = _target.position - _ship.position;
            _pointer.style.rotate = new Rotate(Angle.Degrees(-Mathf.Atan2(d.y, d.x) * Mathf.Rad2Deg));
        }
    }
}
