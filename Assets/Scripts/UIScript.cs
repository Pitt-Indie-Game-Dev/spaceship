using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

#if UNITY_EDITOR
using UnityEditor;
#endif
[CreateAssetMenu]
public class UIScript : ScriptableObject
{

    [SerializeField] private Rigidbody2D rb;
    
    [SerializeField] private PlayerShip playerShip;
    
    
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
