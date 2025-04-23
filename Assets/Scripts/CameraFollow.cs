using UnityEngine;

public class SimpleCameraFollow : MonoBehaviour
{
    public Transform target;  // Drag your player object here in Inspector
    public Vector3 offset = new Vector3(0f, 0f, -10f); // Camera's position relative to player
    
    void LateUpdate()
    {
        if (target != null)
        {
            // Snap directly to player position + offset
            transform.position = target.position + offset;
        }
    }
}