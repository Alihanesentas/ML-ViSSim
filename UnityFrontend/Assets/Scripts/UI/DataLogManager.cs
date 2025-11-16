using UnityEngine;
using UnityEngine.UI; // ScrollRect için

// Bu sınıf, UI'daki log panelini yönetir
public class DataLogManager : MonoBehaviour
{
    // --- Inspector'dan Sürükle-Bırak ---
    
    // 1. Proje panelinden (Assets) LogEntry prefab'ını buraya sürükle
    public LogEntry logEntryPrefab; 
    
    // 2. Hierarchy panelinden ScrollView'un "Content" objesini buraya sürükle
    public RectTransform logContentArea; 

    private int currentIteration = 0;

    /// <summary>
    /// Log tablosuna yeni bir satır ekler.
    /// Bu, SimulationManager tarafından her adımda çağrılır.
    /// </summary>
    public void AddLogEntry(StepDataResponse stepData)
    {
        currentIteration++;

        // 1. Yeni bir log satırı prefab'ı yarat (Instantiate)
        LogEntry newEntry = Instantiate(logEntryPrefab, logContentArea);

        // 2. İçini doldur
        newEntry.Populate(currentIteration, stepData);

        // TODO:
        // (Opsiyonel) ScrollRect'i otomatik olarak en alta kaydır
    }

    /// <summary>
    /// Yeni bir simülasyon başladığında log'u temizler.
    /// </summary>
    public void ClearLog()
    {
        currentIteration = 0;
        
        // Content alanındaki tüm eski log satırlarını (prefab'ları) yok et
        foreach (Transform child in logContentArea)
        {
            Destroy(child.gameObject);
        }
    }
}