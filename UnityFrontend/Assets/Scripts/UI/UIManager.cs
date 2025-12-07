using UnityEngine;
using UnityEngine.UI;
using TMPro; 

public class UIManager : MonoBehaviour
{
    [Header("Core Dependencies")]
    // Drag _SYSTEM_MANAGERS here
    public SimulationManager simManager; 

    [Header("UI Elements")]
    // Drag your UI Buttons here
    public Button generateSurfaceButton;
    public Button playButton;
    public Button pauseButton;
    public Button stepButton;
    
    // public TMP_Dropdown modelDropdown; // Future TODO
    // public Button importDataButton;    // Future TODO

    void Start()
    {
        // 1. AUTO-FIND Dependencies (Backup Plan)
        if (simManager == null)
        {
            simManager = FindObjectOfType<SimulationManager>();
            if (simManager == null) Debug.LogError("UIManager: Critical! SimulationManager not found in scene.");
        }

        // 2. CONNECT BUTTONS
        // We use checks to prevent null reference errors if a button isn't assigned yet.
        
        if (generateSurfaceButton != null)
            generateSurfaceButton.onClick.AddListener(OnGenerateSurfaceClicked);

        if (playButton != null)
            playButton.onClick.AddListener(() => simManager.PlaySimulation());

        if (pauseButton != null)
            pauseButton.onClick.AddListener(() => simManager.PauseSimulation());

        if (stepButton != null)
        {
            // The lambda wrapper { } ensures we fire the async method correctly
            stepButton.onClick.AddListener(() => { _ = simManager.StepOnce(); });
        }
        else
        {
            Debug.LogError("UIManager: Step Button is NOT assigned in Inspector!");
        }
    }

    public async void OnGenerateSurfaceClicked()
    {
        if (simManager == null) return;

        // Hardcoded defaults for now
        string selectedModel = "linear_regression"; 
        string selectedData = "default_data";       
        
        // 1. Configure Simulation
        simManager.model = selectedModel;
        simManager.data_id = selectedData;
        
        // 2. Reset Log
        if (simManager.dataLogManager != null) 
            simManager.dataLogManager.ClearLog();

        // 3. Fetch & Draw
        if (simManager.apiClient != null)
        {
            SurfaceDataResponse surfaceData = await simManager.apiClient.GetCostSurfaceAsync(selectedModel, selectedData);

            if (surfaceData != null)
            {
                // Find renderer dynamically if not linked
                var renderer = FindObjectOfType<GraphRenderer>();
                if (renderer != null) renderer.DrawSurface(surfaceData);
                
                // Reset Ball Position
                simManager.MoveToRandomStartPoint();
            }
        }
    }
}