using UnityEngine;
using System.Linq; // Used for .Select()

// This class handles the "Meshing" logic.
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GraphRenderer : MonoBehaviour
{
    private Mesh mesh;

    void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        // Assign a default material (you can change this in the Inspector)
        GetComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));
    }

    // Called by UIManager *once* per simulation.
    public void DrawSurface(SurfaceDataResponse data)
    {
        Debug.Log("Unity: Drawing 3D Mesh...");
        mesh.Clear();

        [cite_start]// --- Convert Python  data to Unity  data ---
        // Efficiently convert array of structs to array of Vector3
        Vector3[] unityVertices = data.vertices.Select(v => new Vector3(v.x, v.y, v.z)).ToArray();
        
        int[] triangles = data.triangles;

        // --- Fix: Python (RHS CCW) to Unity (LHS CW) ---
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int temp = triangles[i + 1];
            triangles[i + 1] = triangles[i + 2];
            triangles[i + 2] = temp;
        }
        // ------------------------------------

        mesh.vertices = unityVertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals(); // Auto-calculate lighting
        mesh.RecalculateBounds(); // Helps the camera focus
    }
}