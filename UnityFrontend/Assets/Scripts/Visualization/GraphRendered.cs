using UnityEngine;
using System.Linq;
using System.Collections.Generic;
// using System.Numerics; // Removed to avoid ambiguity with UnityEngine.Vector2


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GraphRenderer : MonoBehaviour
{
    private Mesh mesh;

    [Header("Visual Settings")]
    public Gradient heightGradient; 
    public float thickness = 0.5f; // Thickness amount for the mesh

    public Material urpLitMaterial; // Assign this in Inspector

    public float gridStepSize = 1.0f; // Size of each grid cell for UV mapping

    private MeshRenderer meshRenderer;


    void Awake()
    {
        mesh = new Mesh();
        // Allows for more than 65k vertices (needed for large grids)
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; 
        
        GetComponent<MeshFilter>().mesh = mesh;
        
        if(urpLitMaterial != null)
        {
            GetComponent<MeshRenderer>().material = urpLitMaterial;
        }
        else
        {
            Debug.LogWarning("GraphRenderer: urpLitMaterial is not assigned. Using default.");
            GetComponent<MeshRenderer>().material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        }
    }

    // Called by UIManager *once* per simulation to draw the static mesh.
    public void DrawSurface(SurfaceDataResponse data)
    {
        Debug.Log("Unity: Drawing Thick 3D Mesh with Heatmap...");
        mesh.Clear();

        // 1. Top Surface Vertices
        // Convert Python data (VertexData) to Unity data (Vector3)
        Vector3[] topVertices = data.vertices.Select(v => new Vector3(v.x, v.y, v.z)).ToArray();
        
        // 2. Bottom Surface Vertices
        // Create a copy of top vertices and shift them down by 'thickness'
        Vector3[] bottomVertices = new Vector3[topVertices.Length];
        for (int i = 0; i < topVertices.Length; i++)
        {
            bottomVertices[i] = topVertices[i] - new Vector3(0, thickness, 0);
        }

        // Combine all vertices (Top first, then Bottom)
        Vector3[] allVertices = new Vector3[topVertices.Length * 2];
        System.Array.Copy(topVertices, 0, allVertices, 0, topVertices.Length);
        System.Array.Copy(bottomVertices, 0, allVertices, topVertices.Length, bottomVertices.Length);

        // --- Calculate Vertex Colors (Heatmap)-(Grid) ---
        Color[] colors = new Color[allVertices.Length];
        Vector2[] uvs = new Vector2[allVertices.Length];

        
        // Find min/max height (y) to normalize values
        float minY = topVertices.Min(v => v.y);
        float maxY = topVertices.Max(v => v.y);
        float rangeY = maxY - minY;
        

        if (rangeY <= 0) rangeY = 1f;
        // find to grid extents in x and z for UV mapping
        float minX = topVertices.Min(v => v.x);
        float maxX = topVertices.Max(v => v.x);
        float rangeX = maxX - minX;
        

        float minZ = topVertices.Min(v => v.z);
        float maxZ = topVertices.Max(v => v.z);
        float rangeZ = maxZ - minZ;
        for (int i = 0; i<topVertices.Length;i++){
            // Color Mapping based on Height
            float normalizedHeight = (topVertices[i].y - minY) / rangeY;
            Color c = heightGradient.Evaluate(normalizedHeight);
            colors[i] = c;
            colors[i+topVertices.Length] = c;

            // UV Mapping based on X and Z positions arange(0,1)
            float u = (topVertices[i].x - minX) / rangeX;
            float v = (topVertices[i].z- minZ) / rangeZ;
            Vector2 uv = new Vector2(u,v);
            uvs[i] = uv;
            uvs[i + topVertices.Length] = uv;

        }
        

        // --- Calculate Triangles ---
        int[] originalTriangles = data.triangles;
        List<int> allTriangles = new List<int>();

        // 1. Top Surface Triangles
        // Fix winding order: Convert Python (RHS/CCW) to Unity (LHS/CW) by swapping 2nd and 3rd indices
        for (int i = 0; i < originalTriangles.Length; i += 3)
        {
            allTriangles.Add(originalTriangles[i]);
            allTriangles.Add(originalTriangles[i + 2]); // Swap
            allTriangles.Add(originalTriangles[i + 1]); // Swap
        }

        // 2. Bottom Surface Triangles
        // These should face downwards. Keeping original order (or reversing the reverse) usually works.
        // We offset indices by the number of top vertices.
        int offset = topVertices.Length;
        for (int i = 0; i < originalTriangles.Length; i += 3)
        {
            // No swap here to make them face the opposite direction of top surface
            allTriangles.Add(originalTriangles[i] + offset);
            allTriangles.Add(originalTriangles[i + 1] + offset); 
            allTriangles.Add(originalTriangles[i + 2] + offset);
        }

        // (Optional: Side walls generation logic would go here)

        // --- Assign Data to Mesh ---
        mesh.vertices = allVertices;
        mesh.triangles = allTriangles.ToArray();
        mesh.colors = colors;
        mesh.uv = uvs;
        
        mesh.RecalculateNormals(); // Important for lighting
        mesh.RecalculateBounds();  // Important for camera culling
    if (gridStepSize > 0 ){
        // Set tiling for grid effect
        float tileX = rangeX / gridStepSize;
        float tileY = rangeY / gridStepSize;
        // change property tilling 
        // _BaseMap_ST is the internal name for the main texture's tiling and offset
        meshRenderer.material.mainTextureScale = new Vector2(tileX,tileY);
        if (meshRenderer.material.HasProperty("_Tiling")){
            meshRenderer.material.SetVector("_Tiling", new Vector4(tileX, tileY, 0, 0));
        }


    }
    }
}