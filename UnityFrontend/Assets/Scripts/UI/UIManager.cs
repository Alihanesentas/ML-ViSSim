using UnityEngine;
using UnityEngine.UI; // For Sliders, Buttons

// This is the "Orchestra Conductor".
// It knows about all other components and connects them.
public class UIManager : MonoBehaviour
{
    [Header("Component Links")]
    public APIClient apiClient;
    public GraphRenderer graphRenderer;
    public PointController pointController;
    public DataLogManager dataLogManager;
    public SimulationManager simManager;

    [Header("UI Elements (Drag from Hierarchy)")]
    public Button generateSurfaceButton;
    public Button playButton;
    public Button pauseButton;
    public Button stepButton;
    // TODO: Add sliders for LR, Speed, etc.

    void Start()
    {
        // --- Setup Dependencies ---
        // Give the SimulationManager the components it needs to control
        simManager.apiClient = this.apiClient;
        simManager.pointController = this.pointController;
        simManager.dataLogManager = this.dataLogManager;

        // --- Connect UI Buttons to Functions ---
        generateSurfaceButton.onClick.AddListener(OnGenerateSurfaceClicked);
        playButton.onClick.AddListener(simManager.PlaySimulation);
        pauseButton.onClick.AddListener(simManager.PauseSimulation);
        stepButton.onClick.AddListener(() => simManager.StepOnce()); // Lambda for async Task
    }

    // Called when "Generate Surface" button is clicked
    public async void OnGenerateSurfaceClicked()
    {
        // TODO: Get selected model and data from UI dropdowns
        string selectedModel = "linear_regression";
        string selectedData = "default_data";

        // 1. Call the "Heavy Load" API
        SurfaceDataResponse surfaceData = await apiClient.GetCostSurfaceAsync(selectedModel, selectedData);

        // 2. Tell the GraphRenderer to draw the mesh
        if (surfaceData != null)
        {
            graphRenderer.DrawSurface(surfaceData);
        }
    }
    
    // TODO: Add functions to read sliders
    // public void OnLearningRateChanged(float value) { ... }
    // public void OnSpeedChanged(float value) { simManager.SetSpeed(value); }
}