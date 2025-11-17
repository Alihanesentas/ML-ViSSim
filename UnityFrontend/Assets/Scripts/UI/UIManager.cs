using UnityEngine;
using UnityEngine.UI;
using TMPro; // For TMP_Dropdown

// The "Orchestra Conductor". Connects UI to logic.
public class UIManager : MonoBehaviour
{
    [Header("Component Links (Drag from Hierarchy)")]
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
    
    // --- TODO: Add these UI elements ---
    // public TMP_Dropdown modelDropdown;
    // public TMP_Dropdown algorithmDropdown;
    // public Slider lrSlider;
    // public Slider speedSlider;
    // public Button importDataButton;
    // public Button generateDataButton;

    void Start()
    {
        // --- Setup Dependencies ---
        simManager.apiClient = this.apiClient;
        simManager.pointController = this.pointController;
        simManager.dataLogManager = this.dataLogManager;

        // --- Connect UI Buttons to Functions ---
        generateSurfaceButton.onClick.AddListener(OnGenerateSurfaceClicked);
        playButton.onClick.AddListener(simManager.PlaySimulation);
        pauseButton.onClick.AddListener(simManager.PauseSimulation);
        stepButton.onClick.AddListener(() => simManager.StepOnce());
        
        // TODO: Connect other UI elements
        // lrSlider.onValueChanged.AddListener(OnLearningRateChanged);
        // algorithmDropdown.onValueChanged.AddListener(OnAlgorithmChanged);
    }

    public async void OnGenerateSurfaceClicked()
    {
        // TODO: Get selected model and data from UI dropdowns
        string selectedModel = "linear_regression"; // (Get from modelDropdown)
        string selectedData = "default_data";       // (Get from dataDropdown)
        
        // Tell SimManager the config
        simManager.model = selectedModel;
        simManager.data_id = selectedData;
        
        // Clear old logs
        dataLogManager.ClearLog();

        // 1. Call the "Heavy Load" API
        SurfaceDataResponse surfaceData = await apiClient.GetCostSurfaceAsync(selectedModel, selectedData);

        // 2. Tell the GraphRenderer to draw the mesh
        if (surfaceData != null)
        {
            graphRenderer.DrawSurface(surfaceData);
        }
    }
    
    public void OnLearningRateChanged(float value)
    {
        // Read value from LR slider and update the SimManager's config
        simManager.hyperparameters.learning_rate = value;
    }
    
    public void OnAlgorithmChanged(int index)
    {
        // Read value from Algorithm dropdown
        // string selectedAlgo = algorithmDropdown.options[index].text;
        // simManager.algorithm = selectedAlgo;
    }
    
    // TODO: Add OnImportDataClicked()
    // TODO: Add OnGenerateDataClicked()
}