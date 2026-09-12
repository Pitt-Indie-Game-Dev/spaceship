using System.Runtime.CompilerServices;
using Unity.Burst.Intrinsics;
using UnityEngine;
using UnityEngine.InputSystem;

public class RotateTowardMouse : MonoBehaviour
{

    [System.Serializable]
    public class WeaponSprites
    {
        public Sprite rightArm;
        public Sprite leftArm;
    }
    public SpriteRenderer rightRenderer; 
    public SpriteRenderer leftRenderer;
    public WeaponSprites weapon1;
    public WeaponSprites weapon2;

    public Transform armsOffset;

    private void Awake()
    {
        //rightRenderer = GetComponent<SpriteRenderer>();
        //leftRenderer = GetComponentInChildren<SpriteRenderer>();

    }

    private void Update()
    {
        Vector3 mousePosition = GetMouseWorldPosition();
        var kb = Keyboard.current;

        Vector3 aimDirection = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, angle);

        bool aimingLeft = mousePosition.x < transform.position.x;
        Vector3 scale = transform.localScale;
        scale.y = aimingLeft ? -1f : 1f;
        transform.localScale = scale;

        if (kb.digit1Key.wasPressedThisFrame)
        {
            
            equipWeapon(weapon1);
        }
        if (kb.digit2Key.wasPressedThisFrame)
        {
            equipWeapon(weapon2);
        }

        if(rightRenderer.sprite == weapon1.rightArm) armsOffset.transform.localPosition = new Vector3(0.18f, 0, 0);
        if(rightRenderer.sprite == weapon2.rightArm) armsOffset.transform.localPosition = new Vector3(0.30f, 0, 0);
    }

    public static Vector3 GetMouseWorldPosition()
    {
        Camera cam = Camera.main;
        if (Mouse.current == null || cam == null) return Vector3.zero;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 vec = cam.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, -cam.transform.position.z));
        vec.z = 0f;
        return vec;
    }
    
    
    void equipWeapon(WeaponSprites w)
    {
        rightRenderer.sprite = w.rightArm;
        leftRenderer.sprite = w.leftArm;
    }
}