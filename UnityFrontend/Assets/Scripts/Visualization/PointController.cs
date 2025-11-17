using UnityEngine;

// This class controls the "moving point" GameObject.
public class PointController : MonoBehaviour
{
    // This is called by SimulationManager *every* epoch.
    public void UpdatePointPosition(StepDataResponse stepData)
    {
        float w0 = stepData.w[0];
        float w1 = stepData.w[1];
        float cost = stepData.cost;
        
        // Update this GameObject's 3D position
        transform.position = new Vector3(w0, cost, w1);
    }
}