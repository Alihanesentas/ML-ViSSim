using UnityEngine;
using System.Threading.Tasks;

// This class manages the *state* of the simulation (Play, Pause, Speed).
public class SimulationManager : MonoBehaviour
{
    // --- Links to other components (set by UIManager) ---
    [HideInInspector] public APIClient apiClient;
    [HideInInspector] public PointController pointController;
    [HideInInspector] public DataLogManager dataLogManager;

    // --- Simulation State ---
    private bool isRunning = false;
    private float simulationSpeed = 1.0f;
    private float[] current_w = { 10f, -5f }; // Default start point
    
    // --- Config (set by UIManager) ---
    private string model = "linear_regression";
    private string algo = "GradientDescent";
    private string data_id = "default_data";
    private float lr = 0.01f;

    // Called by UIManager's "Play" button
    public void PlaySimulation()
    {
        isRunning = true;
        RunSimulationLoop(); // Start the async loop
    }

    // Called by UIManager's "Pause" button
    public void PauseSimulation()
    {
        isRunning = false;
    }

    // Called by UIManager's "Step" button
    public async Task StepOnce()
    {
        isRunning = false; // Pause if it was running
        await ExecuteSingleStep();
    }
    
    public void SetSpeed(float speed)
    {
        simulationSpeed = speed; //
    }

    // The main simulation loop (runs when isRunning is true)
    private async void RunSimulationLoop()
    {
        while (isRunning)
        {
            await ExecuteSingleStep();
            
            // Wait based on speed
            await Task.Delay((int)(1000 / simulationSpeed)); 
            
            // TODO: Add stop condition (if cost is min or epoch max)
        }
    }

    // The core logic for one epoch
    private async Task ExecuteSingleStep()
    {
        // 1. Create the request
        StepDataRequest requestData = new StepDataRequest {
            model = this.model,
            data_id = this.data_id,
            current_w = this.current_w,
            algorithm = this.algo,
            learning_rate = this.lr
        };
        

        // 2. Call the API
        StepDataResponse response = await apiClient.CalculateNextStepAsync(requestData);

        if (response != null)
        {
            // 3. Update the state
            this.current_w = response.w;
            
            // 4. Tell other components to update
            pointController.UpdatePointPosition(response);
            dataLogManager.AddLogEntry(response); //
        }
        else
        {
            // Stop simulation if API fails
            isRunning = false; 
            Debug.LogError("Simulation stopped due to API error.");
        }
    }
}