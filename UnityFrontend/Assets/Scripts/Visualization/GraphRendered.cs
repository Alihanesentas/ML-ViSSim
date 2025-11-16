using UnityEngine;

// This class handles the "Meshing" logic.
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GraphRenderer : MonoBehaviour
{
    private Mesh mesh;

    void Awake()
    {
        // Setup the Mesh components on this GameObject
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        GetComponent<MeshRenderer>().material = new Material(Shader.Find("Standard"));
    }

    // This is called by UIManager *once* per simulation.
    public void DrawSurface(SurfaceDataResponse data)
    {
        Debug.Log("Unity: Drawing 3D Mesh...");
        mesh.Clear();

        [cite_start]// --- Convert Python  data to Unity  data ---
        Vector3[] unityVertices = new Vector3[data.vertices.Length];
        for (int i = 0; i < data.vertices.Length; i++)
        {
            // Note: Python's (x,y,z) becomes Unity's (x,y,z)
            // We map w0->x, cost->y, w1->z
            unityVertices[i] = new Vector3(
                data.vertices[i].x, 
                data.vertices[i].y, 
                data.vertices[i].z
            );
        }
        
        int[] triangles = data.triangles;

        // --- Fix: Python (RHS) to Unity (LHS) ---
        // Flip the winding order of triangles
        for (int i = 0; i < triangles.Length; i += 3)
        {
            int temp = triangles[i + 1];
            triangles[i + 1] = triangles[i + 2];
            triangles[i + 2] = temp;
        }
        // ------------------------------------

        // --- Assign data to the Mesh ---
        mesh.vertices = unityVertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals(); // Auto-calculate lighting
    }
}