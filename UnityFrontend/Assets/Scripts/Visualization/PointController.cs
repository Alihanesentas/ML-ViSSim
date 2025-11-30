using UnityEngine;

public class PointController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float smoothSpeed = 5.0f; // Higher = faster smoothing
    
    private Vector3 targetPosition;
    private MeshRenderer meshRenderer;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Start()
    {
        // Hide the ball initially. We will show it only after the first TeleportTo call.
        // This prevents the ball from appearing at (0,0,0) incorrectly.
        if (meshRenderer != null) 
            meshRenderer.enabled = false;
            
        targetPosition = transform.position;
    }

    void Update()
    {
        // Smoothly move the point towards the target position every frame.
        // This creates a nice sliding animation between steps.
        transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed * Time.deltaTime);
    }

    // Called by SimulationManager every epoch/step
    public void UpdatePointPosition(StepDataResponse stepData)
    {
        // 1. Extract Raw Data
        float w0 = stepData.w[0];
        float w1 = stepData.w[1];
        float cost = stepData.cost;
        
        // 2. Set Target (No Scaling applied, as requested)
        targetPosition = new Vector3(w0, cost, w1);
    }
    
    // Called by SimulationManager at the START to snap the point instantly
    public void TeleportTo(float w0, float cost, float w1)
    {
        // Enable the mesh now that we have valid data
        if (meshRenderer != null) 
            meshRenderer.enabled = true;

        Vector3 pos = new Vector3(w0, cost, w1);
        
        // Apply instantly to both Transform and Target to prevent smoothing/sliding
        transform.position = pos;
        targetPosition = pos;
    }
}