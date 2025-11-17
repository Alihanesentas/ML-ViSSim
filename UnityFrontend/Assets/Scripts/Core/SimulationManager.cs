using UnityEngine;
using System.Threading.Tasks;

// Manages the *state* of the simulation (Play, Pause, Speed).
public class SimulationManager : MonoBehaviour
{
    // --- Links (Set by UIManager) ---
    [HideInInspector] public APIClient apiClient;
    [HideInInspector] public PointController pointController;
    [HideInInspector] public DataLogManager dataLogManager;

    // --- Simulation State ---
    private bool isRunning = false;
    private float simulationSpeed = 1.0f;
    private float[] current_w = { 8f, -8f }; // Default start point
    
    // --- Config (Set by UIManager) ---
    // These values will be updated by UIManager based on dropdowns
    public string model { get; set; } = "linear_regression";
    public string algorithm { get; set; } = "GradientDescent";
    public string data_id { get; set; } = "default_data";
    public HyperparameterData hyperparameters { get; set; } = new HyperparameterData { learning_rate = 0.1f };

    // --- Public Controls (Called by UIManager buttons) ---
    
    public void PlaySimulation()
    {
        if (isRunning) return; // Already playing
        isRunning = true;
        RunSimulationLoop();
    }

    public void PauseSimulation()
    {
        isRunning = false;
    }

    public async Task StepOnce()
    {
        isRunning = false; // Pause if it was running
        await ExecuteSingleStep();
    }
    
    public void SetSpeed(float speed)
    {
        simulationSpeed = Mathf.Max(speed, 0.1f); // Ensure speed isn't zero
    }
    
    public void ResetSimulation(float[] start_w)
    {
        // Called by UIManager to reset the point
        isRunning = false;
        current_w = start_w;
        // TODO: Tell PointController to move to start_w
    }

    // --- Private Logic ---

    private async void RunSimulationLoop()
    {
        while (isRunning)
        {
            await ExecuteSingleStep();
            await Task.Delay((int)(1000 / simulationSpeed));
            
            // TODO: Add stop condition (if cost is min or epoch max)
        }
    }

    private async Task ExecuteSingleStep()
    {
        // 1. Create the request object
        StepDataRequest requestData = new StepDataRequest {
            model = this.model,
            data_id = this.data_id,
            current_w = this.current_w,
            algorithm = this.algorithm,
            hyperparameters = this.hyperparameters
        };
        
        // 2. Call the API
        StepDataResponse response = await apiClient.CalculateNextStepAsync(requestData);

        if (response != null)
        {
            // 3. Update the state
            this.current_w = response.w.ToArray(); // Convert List<float> to float[]
            
            // 4. Tell other components to update
            pointController.UpdatePointPosition(response);
            dataLogManager.AddLogEntry(response);
        }
        else
        {
            isRunning = false; 
            Debug.LogError("Simulation stopped due to API error.");
        }
    }
}