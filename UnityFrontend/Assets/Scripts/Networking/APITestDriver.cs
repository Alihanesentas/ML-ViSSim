// APITestDriver.cs
using UnityEngine;

[RequireComponent(typeof(APIClient))]
public class APITestDriver : MonoBehaviour
{
    private APIClient apiClient;

    void Awake()
    {
        apiClient = GetComponent<APIClient>();
    }

    async void Start()
    {
        Debug.Log("TEST 2: Requesting cost surface from Python ...");

        // API'yi  test et
        SurfaceDataResponse data = await apiClient.GetCostSurfaceAsync("linear_regression", "default_data");

        if (data != null)
        {
            Debug.Log($"TEST 2 SUCCESS! Received {data.vertices.Length} vertices and {data.triangles.Length} triangles from Python .");
        }
        else
        {
            Debug.LogError("TEST 2 FAILED. Check Python  server and APIClient.cs logs.");
        }
    }
}