using UnityEngine;
using UnityEngine.UI;

// Manages the scrolling log panel
public class DataLogManager : MonoBehaviour
{
    // --- Drag from Inspector ---
    public LogEntry logEntryPrefab; 
    public RectTransform logContentArea; // The 'Content' object of a ScrollView
    public ScrollRect logScrollRect; // The ScrollRect component itself

    private int currentIteration = 0;

    // Called by SimulationManager every step
    public void AddLogEntry(StepDataResponse stepData)
    {
        currentIteration++;
        LogEntry newEntry = Instantiate(logEntryPrefab, logContentArea);
        newEntry.Populate(currentIteration, stepData);

        // (Optional) Auto-scroll to bottom
        Canvas.ForceUpdateCanvases();
        logScrollRect.verticalNormalizedPosition = 0f; 
    }

    // Called by UIManager when starting a new simulation
    public void ClearLog()
    {
        currentIteration = 0;
        foreach (Transform child in logContentArea)
        {
            Destroy(child.gameObject);
        }
    }
}