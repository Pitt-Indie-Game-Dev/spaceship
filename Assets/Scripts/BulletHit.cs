// BulletHit is still in progress (detecting if a bullet is hit) - Benson Liu

using UnityEngine;

public class BulletHit : MonoBehaviour
{
    public string targetTag;

    void OnTriggerEnter2D(Collider2D other)
    {
        // Prints what the bullet touched in the Console so we can see why it missed
        Debug.Log("Bullet touched: " + other.gameObject.name + " | Tag: " + other.tag);

        if (other.CompareTag(targetTag))
        {
            HitFlash flash = other.GetComponent<HitFlash>();
            if (flash != null)
            {
                flash.Flash();
            }

            Destroy(gameObject);
        }
    }
}
