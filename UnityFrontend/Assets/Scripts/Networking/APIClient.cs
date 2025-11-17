using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System.Text;

[cite_start]// This is the *only* class that talks to Python  [cite: 188-191, 638-651, 722-723].
public class APIClient : MonoBehaviour
{
    private string baseUrl = "http://127.0.0.1:5000";

    // "Heavy Load" call
    public async Task<SurfaceDataResponse> GetCostSurfaceAsync(string model, string data_id)
    {
        string url = $"{baseUrl}/get_cost_surface?model={model}&data_id={data_id}";
        
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                return JsonUtility.FromJson<SurfaceDataResponse>(json);
            }
            else
            {
                Debug.LogError($"API Error GetCostSurface: {request.error} | {request.downloadHandler.text}");
                return null;
            }
        }
    }

    // "Light Load" call
    public async Task<StepDataResponse> CalculateNextStepAsync(StepDataRequest requestData)
    {
        string url = $"{baseUrl}/calculate_next_step";
        string jsonRequestBody = JsonUtility.ToJson(requestData);
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonRequestBody);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            var operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string json = request.downloadHandler.text;
                return JsonUtility.FromJson<StepDataResponse>(json);
            }
            else
            {
                Debug.LogError($"API Error CalculateNextStep: {request.error} | {request.downloadHandler.text}");
                return null;
            }
        }
    }
}