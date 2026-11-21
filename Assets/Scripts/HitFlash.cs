// HitFlash is still in progress (display a brief moment of flash when a player or enemy is hit) - Benson Liu

using System.Collections;
using UnityEngine;

public class HitFlash : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        // Searches both this object AND any child objects for the SpriteRenderer
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    public void Flash()
    {
        if (spriteRenderer == null) return;

        StopAllCoroutines();
        StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        spriteRenderer.color = Color.red;
        yield return new WaitForSeconds(0.15f);
        spriteRenderer.color = originalColor;
    }
}
