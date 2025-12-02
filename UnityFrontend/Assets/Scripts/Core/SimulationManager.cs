using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;

public class SimulationManager : MonoBehaviour
{
    // --- Dependencies ---
    [Header("Dependencies")]
    // [HideInInspector] removed so you can DEBUG by dragging them manually if needed
    public APIClient apiClient;
    public PointController pointController; 
    public DataLogManager dataLogManager;

    // --- Simulation State ---
    private bool isRunning = false;
    private float simulationSpeed = 2.0f; 
    
    private float[] current_w = { 0f, 0f }; 
    private float currentCost = float.MaxValue; 

    // --- Configuration ---
    public string model { get; set; } = "linear_regression";
    public string algorithm { get; set; } = "GradientDescent";
    public string data_id { get; set; } = "default_data";
    
    public HyperparameterData hyperparameters { get; set; } = new HyperparameterData { learning_rate = 0.1f };

    private float boundMin = -10f;
    private float boundMax = 10f;
    private float stopThreshold = 0.0001f; 

    void Start()
    {
        // Auto-find references if missing (Fail-safe)
        if (pointController == null) 
            pointController = FindObjectOfType<PointController>();
            
        if (apiClient == null)
            apiClient = GetComponent<APIClient>();
            
        if (dataLogManager == null)
            dataLogManager = GetComponent<DataLogManager>();
    }

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

    public async Task StepOnce()
    {
        // Force stop continuous run if stepping manually
        if (isRunning) isRunning = false; 
        
        Debug.Log("Simulation: Manual Step Triggered...");
        await ExecuteSingleStep();
    }
    
    public void SetSpeed(float speed)
    {
        simulationSpeed = Mathf.Max(speed, 0.1f);
    }

    // --- RANDOM START POINT LOGIC ---
    public void MoveToRandomStartPoint()
    {
        // Start on the slopes (between 4 and 8, or -8 and -4)
        float w0 = (Random.value > 0.5f) ? Random.Range(4f, 8f) : Random.Range(-8f, -4f);
        float w1 = (Random.value > 0.5f) ? Random.Range(4f, 8f) : Random.Range(-8f, -4f);

        current_w = new float[] { w0, w1 };
        currentCost = float.MaxValue; 

        Debug.Log($"Simulation: New Start Point Set: [{w0}, {w1}]");

        // Ask Python for the cost height to teleport visual
        _ = ProbeInitialHeight(); 
    }

    private async Task ProbeInitialHeight()
    {
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
                Debug.Log($"Simulation: Teleporting point to start: {response.w[0]}, {response.cost}, {response.w[1]}");
                pointController.TeleportTo(response.w[0], response.cost, response.w[1]);
            }
            else
            {
                Debug.LogError("Simulation: Probe failed. API or PointController is null.");
            }
        }
        else
        {
            Debug.LogError("Simulation: API Client is NULL!");
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

    private async Task ExecuteSingleStep()
    {
        // 1. Prepare Request
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
                // Convergence Check
                float costDiff = Mathf.Abs(currentCost - response.cost);
                if (costDiff < stopThreshold)
                {
                    Debug.Log("<color=green>Minimum Reached!</color>");
                    isRunning = false;
                }

                // Boundary Check
                if (response.w[0] < boundMin || response.w[0] > boundMax || 
                    response.w[1] < boundMin || response.w[1] > boundMax)
                {
                    Debug.LogWarning("Simulation: Ball hit the wall!");
                    isRunning = false;
                }

                // 3. Update Internal Data
                this.current_w = response.w.ToArray();
                this.currentCost = response.cost;      
                
                // 4. Update Visuals
                if (pointController != null)
                {
                    Debug.Log($"Simulation: Moving to [{response.w[0]}, {response.cost}, {response.w[1]}]");
                    pointController.UpdatePointPosition(response);
                }
                else
                {
                    Debug.LogError("Simulation: PointController is missing!");
                }
                
                // 5. Write to Log
                if (dataLogManager != null)
                    dataLogManager.AddLogEntry(response);
            }
            else
            {
                Debug.LogError("Simulation: API Response was NULL");
                isRunning = false;
            }
        }
        else
        {
            Debug.LogError("Simulation: API Client not assigned.");
        }
    }
}