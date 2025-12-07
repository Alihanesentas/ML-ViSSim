using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class PointController : MonoBehaviour
{
    [Header("Movement Settings")]
    
    public GraphRenderer graphRenderer;
    private MeshRenderer meshRenderer;
    public float raycastOffset = 100.0f; // height of raycast origin above the graph

    public float smoothSpeed = 2.0f; // Higher = faster smoothing
    private Vector3 targetPosition;
    private Vector3 currentXZ;


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
        currentXZ = transform.position;
    }

    void Update()
    {
        // horizontal position movement only (XZ plane)
        // just XZ entrerpolition
        Vector3 targetXZ = new Vector3(targetPosition.x,0,targetPosition.z);
        Vector3 newXZ = Vector3.Lerp(new Vector3(currentXZ.x,0,currentXZ.z), targetXZ, smoothSpeed * Time.deltaTime);
        currentXZ  = newXZ;

        // vertical position movement find to floor(Y)
        Vector3 rayOrigin = new Vector3(currentXZ.x, raycastOffset, currentXZ.z);
        RaycastHit hit;
        //layermask not use, collide to meshcollider
        if(Physics.Raycast(rayOrigin, Vector3.down, out hit, Mathf.Infinity)){
            transform.position = hit.point + Vector3.up * 0.2f; // slight offset above the surface
        }
        else{
            // if we did not found the floor, go to height
            float newY = Mathf.Lerp(transform.position.y,targetPosition.y,smoothSpeed * Time.deltaTime);
            transform.position = new Vector3(currentXZ.x,newY,currentXZ.z);
        }

    }

    // Called by SimulationManager every epoch/step
    public void UpdatePointPosition(StepDataResponse stepData)
    {
        // 1. Extract Raw Data
        float w0 = stepData.w[0];
        float w1 = stepData.w[1];
        float cost = stepData.cost;
        
        float multiplier = (graphRenderer != null) ? graphRenderer.heightScale : 1.0f;

        // 2. Set Target (No Scaling applied, as requested)
        targetPosition = new Vector3(w0, cost*multiplier, w1);
    }
    
    // Called by SimulationManager at the START to snap the point instantly
    public void TeleportTo(float w0, float cost, float w1)
    {
        // Enable the mesh now that we have valid data
        if (meshRenderer != null) 
            meshRenderer.enabled = true;
        
        float multiplier = (graphRenderer != null) ? graphRenderer.heightScale : 1.0f;

        Vector3 pos = new Vector3(w0, cost * multiplier, w1);
        
        // Apply instantly to both Transform and Target to prevent smoothing/sliding
        transform.position = pos;
        targetPosition = pos;
    }
}