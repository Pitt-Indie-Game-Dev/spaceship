using UnityEngine;

public class AsteroidRotate : MonoBehaviour
{
    public float rotateSpeed = 1.0f; // Rotation speed (degrees/s)

    // Update is called once per frame
    private void Update()
    {
        transform.Rotate(new Vector3(0.0f, 0.0f, 1.0f), rotateSpeed * Time.deltaTime);
    }
}
