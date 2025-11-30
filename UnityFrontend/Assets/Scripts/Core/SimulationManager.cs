using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

public class SimulationManager : MonoBehaviour
{
    // --- Dependencies (Assigned by UIManager) ---
    [HideInInspector] public APIClient apiClient;
    [HideInInspector] public PointController pointController;
    [HideInInspector] public DataLogManager dataLogManager;

    // --- Simulation State ---
    private bool isRunning = false;
    private float simulationSpeed = 2.0f; // Default speed multiplier
    
    // Current position [w0, w1]
    private float[] current_w = { 0f, 0f }; 
    private float currentCost = float.MaxValue; // Track cost for convergence check

    // --- Configuration (Updated by UIManager) ---
    public string model { get; set; } = "linear_regression";
    public string algorithm { get; set; } = "GradientDescent";
    public string data_id { get; set; } = "default_data";
    
    // Learning Rate
    public HyperparameterData hyperparameters { get; set; } = new HyperparameterData { learning_rate = 0.1f };

    // --- Boundaries & Stopping Criteria ---
    // Must match the Python grid range (-10 to 10)
    private float boundMin = -10f;
    private float boundMax = 10f;
    // Stop if cost change is smaller than this
    private float stopThreshold = 0.0001f; 

    // --- Public Controls (Called by UIManager Buttons) ---

    public void PlaySimulation()
    {
        if (isRunning) return;
        Debug.Log($"Simulation: Started. LR = {hyperparameters.learning_rate}");
        isRunning = true;
        RunSimulationLoop();
    }

    public void PauseSimulation()
    {
        isRunning = false;
        Debug.Log("Simulation: Paused.");
    }

    // Step Button calls this function
    public async Task StepOnce()
    {
        if (isRunning) isRunning = false; 
        await ExecuteSingleStep();
    }
    
    public void SetSpeed(float speed)
    {
        simulationSpeed = Mathf.Max(speed, 0.1f);
    }

    // --- CRITICAL UPDATE: Random Start Point ---
    public void MoveToRandomStartPoint()
    {
        // We want to start from the middle/slopes of the mesh, but not at the very bottom.
        // Our grid is between -10 and 10.
        // The exact center (0,0) is usually close to the lowest point (since data is normalized).
        // Therefore, let's start near the edges so we can watch the descent.
        
        // Select a spot between -8 and -4 OR between 4 and 8.
        // This ensures we start on the "slope".
        
        float w0 = (Random.value > 0.5f) ? Random.Range(4f, 8f) : Random.Range(-8f, -4f);
        float w1 = (Random.value > 0.5f) ? Random.Range(4f, 8f) : Random.Range(-8f, -4f);

        current_w = new float[] { w0, w1 };
        currentCost = float.MaxValue; 

        Debug.Log($"Simulation: New Start Point: [{w0}, {w1}]");

        // We ask Python "What is the cost (height) here?" to visually teleport the ball.
        _ = ProbeInitialHeight(); 
    }

    // "Dummy Step" used only to learn the height and teleport the ball
    private async Task ProbeInitialHeight()
    {
        // We send Learning Rate 0 so w0, w1 don't change, only Cost is calculated.
        var probeParams = new HyperparameterData { learning_rate = 0.0f }; 
        
        StepDataRequest request = new StepDataRequest {
            model = this.model, data_id = this.data_id, current_w = this.current_w,
            algorithm = this.algorithm, hyperparameters = probeParams
        };

        if (apiClient != null)
        {
            StepDataResponse response = await apiClient.CalculateNextStepAsync(request);
            if (response != null && pointController != null)
            {
                // Teleport the ball directly
                pointController.TeleportTo(response.w[0], response.cost, response.w[1]);
            }
        }
    }

    private async void RunSimulationLoop()
    {
        while (isRunning)
        {
            await ExecuteSingleStep();
            int delay = (int)(1000 / simulationSpeed);
            await Task.Delay(delay);
        }
    }

    // --- REAL STEP LOGIC ---
    private async Task ExecuteSingleStep()
    {
        // 1. Prepare Request
        // At this point, the Python backend takes the current w0, w1 and learning_rate
        // and applies the Gradient Descent formula (w_new = w_old - lr * gradient).
        StepDataRequest requestData = new StepDataRequest {
            model = this.model,
            data_id = this.data_id,
            current_w = this.current_w,
            algorithm = this.algorithm,
            hyperparameters = this.hyperparameters
        };
        
        // 2. Send to Python
        if (apiClient != null)
        {
            StepDataResponse response = await apiClient.CalculateNextStepAsync(requestData);

            if (response != null)
            {
                // Convergence Check (Stop if change is very small)
                float costDiff = Mathf.Abs(currentCost - response.cost);
                if (costDiff < stopThreshold)
                {
                    Debug.Log("<color=green>Minimum Reached!</color>");
                    isRunning = false;
                }

                // Boundary Check (Don't hit the walls)
                if (response.w[0] < boundMin || response.w[0] > boundMax || 
                    response.w[1] < boundMin || response.w[1] > boundMax)
                {
                    Debug.LogWarning("Simulation: Ball hit the wall!");
                    isRunning = false;
                }

                // 3. Update Internal Data
                this.current_w = response.w.ToArray(); // New w0, w1
                this.currentCost = response.cost;      // New Cost
                
                // 4. Update Visuals (Move the Ball)
                if (pointController != null)
                    pointController.UpdatePointPosition(response);
                
                // 5. Write to Log Screen
                if (dataLogManager != null)
                    dataLogManager.AddLogEntry(response);
            }
            else
            {
                isRunning = false;
            }
        }
    }
}