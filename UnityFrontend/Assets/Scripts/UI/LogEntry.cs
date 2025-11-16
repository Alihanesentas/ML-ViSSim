using UnityEngine;
using TMPro; // TextMeshPro (Unity'nin  modern metin bileşeni) kullanacağız

// Bu script, tek bir log satırı prefab'ının üzerindedir.
public class LogEntry : MonoBehaviour
{
    // --- Inspector'dan Sürükle-Bırak ---
    // Prefab'ın içindeki Text component'lerini buraya sürükle
    public TextMeshProUGUI iterationText;
    public TextMeshProUGUI variablesText;
    public TextMeshProUGUI lossText;
    // TODO: Gerekirse diğer parametreler için Text alanları ekle

    /// <summary>
    /// Bu satırın metin alanlarını gelen adıma göre doldurur.
    /// </summary>
    public void Populate(int iteration, StepDataResponse stepData)
    {
        iterationText.text = iteration.ToString();
        
        // float[] dizisini [w0, w1] formatında bir string'e çevir
        variablesText.text = $"[{stepData.w[0]:F4}, {stepData.w[1]:F4}]"; 
        
        lossText.text = stepData.cost.ToString("F6"); // Virgülden sonra 6 basamak
    }
}