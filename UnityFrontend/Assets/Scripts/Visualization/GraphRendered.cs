using UnityEngine;
using System.Linq;
using System.Collections.Generic;
[RequireComponent(typeof(MeshFilter), typeof(MeshCollider))]

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GraphRenderer : MonoBehaviour
{
    private Mesh mesh;
    private MeshRenderer meshRenderer;

    private MeshCollider meshCollider;
    [Header("Visual Settings")]
    [Tooltip("If checked, 'Scale Gradient' is used. If unchecked, 'Height Gradient' is used.")]
    public bool useScaleGradient = false; // NEW: Checkbox to toggle gradient mode

    public Gradient heightGradient; // Default gradient (acts as if Scale is 1.0f)
    
    [Tooltip("Special color palette to use when the view is scaled down")]
    public Gradient scaleGradient; // Gradient used when scaled

    public Material urpLitMaterial; 
    
    [Tooltip("Y axis Scale for mesh geometry (visual reduction)")]
    public float heightScale = 0.1f;

    [Header("Grid Settings")]
    public float gridStepSize = 1.0f; 
    public Vector2 gridTilingScale = new Vector2(1.0f, 1.0f);

    [Header("Opacity")]
    [Range(0f, 1f)] 
    public float surfaceOpacity = 1.0f; 

    void Awake()
    {
        mesh = new Mesh();
        // Allows for more vertices than the standard limit if needed
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; 
        
        GetComponent<MeshFilter>().mesh = mesh;
        meshRenderer = GetComponent<MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
        
        if(urpLitMaterial != null)
            meshRenderer.material = urpLitMaterial;
        else
            meshRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));

        GetComponent<MeshCollider>();
    }

    void Update()
    {
        // Sync Opacity with Material at runtime
        if (meshRenderer != null && meshRenderer.material != null)
        {
            if (meshRenderer.material.HasProperty("_Opacity"))
                meshRenderer.material.SetFloat("_Opacity", surfaceOpacity);
            else if (meshRenderer.material.HasProperty("_BaseColor"))
            {
                Color c = meshRenderer.material.GetColor("_BaseColor");
                c.a = surfaceOpacity;
                meshRenderer.material.SetColor("_BaseColor", c);
            }
        }
    }

    public void DrawSurface(SurfaceDataResponse data)
    {
        Debug.Log("Unity: Drawing Mesh...");
        mesh.Clear();

        Vector3[] topVertices = new Vector3[data.vertices.Length];
        
        // Calculate Min/Max values based on RAW DATA (data.vertices)
        // We need the true data range for correct color normalization
        float minY = float.MaxValue; float maxY = float.MinValue;
        float minX = float.MaxValue; float maxX = float.MinValue;
        float minZ = float.MaxValue; float maxZ = float.MinValue;

        for (int i = 0; i < data.vertices.Length; i++)
        {
            var v = data.vertices[i];
            
            // Apply Scale to Geometry (Visual Reduction)
            topVertices[i] = new Vector3(v.x, v.y * heightScale, v.z);

            // Use Original Data (Raw v.y) for Min/Max calculation
            if (v.y < minY) minY = v.y;
            if (v.y > maxY) maxY = v.y;
            
            if (v.x < minX) minX = v.x;
            if (v.x > maxX) maxX = v.x;
            if (v.z < minZ) minZ = v.z;
            if (v.z > maxZ) maxZ = v.z;
        }

        // Create Bottom Surface (for thickness)
        Vector3[] bottomVertices = new Vector3[topVertices.Length];
        for (int i = 0; i < topVertices.Length; i++)
        {
            bottomVertices[i] = topVertices[i] - new Vector3(0, 0.5f, 0);
        }

        // Combine Top and Bottom vertices
        Vector3[] allVertices = new Vector3[topVertices.Length * 2];
        System.Array.Copy(topVertices, 0, allVertices, 0, topVertices.Length);
        System.Array.Copy(bottomVertices, 0, allVertices, topVertices.Length, bottomVertices.Length);

        // --- COLORS & UVs ---
        Color[] colors = new Color[allVertices.Length];
        Vector2[] uvs = new Vector2[allVertices.Length];

        float rangeY = maxY - minY; if (rangeY <= 0.0001f) rangeY = 1f;
        float rangeX = maxX - minX; if (rangeX <= 0.0001f) rangeX = 1f;
        float rangeZ = maxZ - minZ; if (rangeZ <= 0.0001f) rangeZ = 1f;

        // Determine which Gradient to use based on the boolean flag
        Gradient activeGradient = useScaleGradient ? scaleGradient : heightGradient;

        for (int i = 0; i < topVertices.Length; i++)
        {
            // IMPORTANT FIX:
            // When calculating color, use the Original Data (data.vertices), 
            // NOT the Scaled geometry (topVertices).
            // This ensures that even if scale is 0.1, the highest point 
            // corresponds to the max color (e.g., Red) of the gradient.
            float rawHeightValue = data.vertices[i].y;
            
            float normalizedHeight = (rawHeightValue - minY) / rangeY;
            
            // Sample color from the selected gradient
            Color c = activeGradient.Evaluate(normalizedHeight);
            
            c.a = surfaceOpacity;
            colors[i] = c;
            colors[i + topVertices.Length] = c;

            // UV Calculation for Grid texture
            float u = (topVertices[i].x - minX); 
            float v = (topVertices[i].z - minZ);
            Vector2 uv = new Vector2(u, v);
            uvs[i] = uv;
            uvs[i + topVertices.Length] = uv;
        }

        // Generate Triangles
        int[] originalTriangles = data.triangles;
        List<int> allTriangles = new List<int>();

        // Top Surface Triangles
        for (int i = 0; i < originalTriangles.Length; i += 3)
        {
            allTriangles.Add(originalTriangles[i]);
            allTriangles.Add(originalTriangles[i + 2]); 
            allTriangles.Add(originalTriangles[i + 1]); 
        }
        
        // Bottom Surface Triangles (reversed order for correct facing)
        int offset = topVertices.Length;
        for (int i = 0; i < originalTriangles.Length; i += 3)
        {
            allTriangles.Add(originalTriangles[i] + offset);
            allTriangles.Add(originalTriangles[i + 1] + offset);
            allTriangles.Add(originalTriangles[i + 2] + offset);
        }

        // Assign to Mesh
        mesh.vertices = allVertices;
        mesh.triangles = allTriangles.ToArray();
        mesh.colors = colors; // Embed Vertex Colors
        mesh.uv = uvs;
        if (meshCollider != null){
            meshCollider.sharedMesh = null; // clear first
            meshCollider.sharedMesh = mesh;
        }


        mesh.RecalculateNormals(); 
        mesh.RecalculateBounds();  

        // Update Shader Tiling (Grid)
        if (meshRenderer.material.HasProperty("_Tiling"))
        {
            meshRenderer.material.SetVector("_Tiling", new Vector4(1.0f / gridStepSize, 1.0f / gridStepSize, 0, 0));
        }
        else if (meshRenderer.material.HasProperty("_BaseMap_ST"))
        {
            meshRenderer.material.SetTextureScale("_BaseMap", new Vector2(1.0f / gridStepSize, 1.0f / gridStepSize));
        }
    }
}