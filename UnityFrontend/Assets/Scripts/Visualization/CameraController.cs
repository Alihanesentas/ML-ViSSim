using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    // The object to look at (Drag your GraphRenderer here)
    public Transform target; 
    public Vector3 offset = new Vector3(0, 5, -10); // Initial position offset relative to target

    [Header("Settings")]
    public float rotateSpeed = 5.0f;
    public float zoomSpeed = 2.0f;
    public float minZoom = 2.0f;
    public float maxZoom = 50.0f;

    private float currentZoom = 10.0f;
    private float currentYaw = 0.0f;   // Horizontal rotation
    private float currentPitch = 20.0f; // Vertical rotation

    void Start()
    {
        // Try to find the graph automatically if not assigned
        if (target == null)
        {
            var graph = FindObjectOfType<GraphRenderer>();
            if (graph != null) target = graph.transform;
        }
    }

    void LateUpdate()
    {
        if (target == null) return;

        // 1. Rotation Input (Right Mouse Button)
        if (Input.GetMouseButton(1)) 
        {
            currentYaw += Input.GetAxis("Mouse X") * rotateSpeed;
            currentPitch -= Input.GetAxis("Mouse Y") * rotateSpeed;
            
            // Clamp vertical rotation so we don't flip over
            currentPitch = Mathf.Clamp(currentPitch, -10, 85);
        }

        // 2. Zoom Input (Mouse Scroll Wheel)
        currentZoom -= Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * 10;
        currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);

        // 3. Calculate Position
        // Rotate around the Y axis
        Quaternion rotation = Quaternion.Euler(currentPitch, currentYaw, 0);
        
        // Calculate position: Target + Rotation * (Distance away)
        Vector3 negDistance = new Vector3(0.0f, 0.0f, -currentZoom);
        Vector3 position = rotation * negDistance + target.position;

        transform.rotation = rotation;
        transform.position = position;
    }
}