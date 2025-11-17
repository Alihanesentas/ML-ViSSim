using UnityEngine;
using TMPro; // Use TextMeshPro

// This script sits on your Log Entry Prefab.
public class LogEntry : MonoBehaviour
{
    // --- Drag these from the Prefab's Hierarchy ---
    public TextMeshProUGUI iterationText;
    public TextMeshProUGUI variablesText;
    public TextMeshProUGUI lossText;
    
    // Fills the text fields
    public void Populate(int iteration, StepDataResponse stepData)
    {
        iterationText.text = iteration.ToString();
        variablesText.text = $"[{stepData.w[0]:F4}, {stepData.w[1]:F4}]"; // F4 = 4 decimal places
        lossText.text = stepData.cost.ToString("F6"); // F6 = 6 decimal places
    }
}