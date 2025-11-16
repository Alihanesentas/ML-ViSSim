using UnityEngine;

// This class controls the "moving point" GameObject.
public class PointController : MonoBehaviour
{
    // This is called by UIManager *every* epoch.
    public void UpdatePointPosition(StepDataResponse stepData)
    {
        // Update this GameObject's position based on the new (w0, cost, w1)
        float w0 = stepData.w[0];
        float w1 = stepData.w[1];
        float cost = stepData.cost;
        
        transform.position = new Vector3(w0, cost, w1);
    }
}