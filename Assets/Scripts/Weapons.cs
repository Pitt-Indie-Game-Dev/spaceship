using UnityEngine;
using UnityEngine.InputSystem;

public class RotateTowardMouse : MonoBehaviour
{
    private void Update()
    {
        Vector3 mousePosition = GetMouseWorldPosition();

        Vector3 aimDirection = (mousePosition - transform.position).normalized;
        float angle = Mathf.Atan2(aimDirection.y, aimDirection.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0, 0, angle);

        bool aimingLeft = mousePosition.x < transform.position.x;
        Vector3 scale = transform.localScale;
        scale.y = aimingLeft ? -1f : 1f;
        transform.localScale = scale;
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
    
}