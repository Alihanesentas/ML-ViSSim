using UnityEngine;
using System.Linq;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GraphRenderer : MonoBehaviour
{
    private Mesh mesh;
    private MeshRenderer meshRenderer;

    [Header("Visual Settings")]
    public Gradient heightGradient; 
    public Material urpLitMaterial; 

    [Header("Grid Settings")]
    public float gridStepSize = 1.0f; 
    
    // NEW: Tiling scale for the grid texture
    public Vector2 gridTilingScale = new Vector2(1.0f, 1.0f);

    [Header("Opacity")]
    [Range(0f, 1f)] 
    public float surfaceOpacity = 1.0f; 

    // Raw Data rendering
    
    void Awake()
    {
        mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; 
        
        GetComponent<MeshFilter>().mesh = mesh;
        meshRenderer = GetComponent<MeshRenderer>();
        
        if(urpLitMaterial != null)
            meshRenderer.material = urpLitMaterial;
        else
            meshRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
    }

    void Update()
    {
        // Sync Opacity
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
        
        float minY = float.MaxValue; float maxY = float.MinValue;
        float minX = float.MaxValue; float maxX = float.MinValue;
        float minZ = float.MaxValue; float maxZ = float.MinValue;

        // 1. Process Vertices
        for (int i = 0; i < data.vertices.Length; i++)
        {
            var v = data.vertices[i];
            topVertices[i] = new Vector3(v.x, v.y, v.z);

            if (v.y < minY) minY = v.y;
            if (v.y > maxY) maxY = v.y;
            if (v.x < minX) minX = v.x;
            if (v.x > maxX) maxX = v.x;
            if (v.z < minZ) minZ = v.z;
            if (v.z > maxZ) maxZ = v.z;
        }

        // 2. Bottom Surface
        Vector3[] bottomVertices = new Vector3[topVertices.Length];
        for (int i = 0; i < topVertices.Length; i++)
        {
            bottomVertices[i] = topVertices[i] - new Vector3(0, 0.5f, 0);
        }

        Vector3[] allVertices = new Vector3[topVertices.Length * 2];
        System.Array.Copy(topVertices, 0, allVertices, 0, topVertices.Length);
        System.Array.Copy(bottomVertices, 0, allVertices, topVertices.Length, bottomVertices.Length);

        // 3. Colors & UVs
        Color[] colors = new Color[allVertices.Length];
        Vector2[] uvs = new Vector2[allVertices.Length];

        float rangeY = maxY - minY; if (rangeY <= 0.0001f) rangeY = 1f;
        float rangeX = maxX - minX; if (rangeX <= 0.0001f) rangeX = 1f;
        float rangeZ = maxZ - minZ; if (rangeZ <= 0.0001f) rangeZ = 1f;

        for (int i = 0; i < topVertices.Length; i++)
        {
            float normalizedHeight = (topVertices[i].y - minY) / rangeY;
            Color c = heightGradient.Evaluate(normalizedHeight);
            c.a = surfaceOpacity;
            colors[i] = c;
            colors[i + topVertices.Length] = c;

            // UV Calculation for Grid
            // We map the world X/Z to UV coordinates 0-1, 
            // but we multiply by range to keep aspect ratio
            float u = (topVertices[i].x - minX); 
            float v = (topVertices[i].z - minZ);
            
            Vector2 uv = new Vector2(u, v);
            uvs[i] = uv;
            uvs[i + topVertices.Length] = uv;
        }

        // 4. Triangles
        int[] originalTriangles = data.triangles;
        List<int> allTriangles = new List<int>();

        for (int i = 0; i < originalTriangles.Length; i += 3)
        {
            allTriangles.Add(originalTriangles[i]);
            allTriangles.Add(originalTriangles[i + 2]); 
            allTriangles.Add(originalTriangles[i + 1]); 
        }
        int offset = topVertices.Length;
        for (int i = 0; i < originalTriangles.Length; i += 3)
        {
            allTriangles.Add(originalTriangles[i] + offset);
            allTriangles.Add(originalTriangles[i + 1] + offset);
            allTriangles.Add(originalTriangles[i + 2] + offset);
        }

        mesh.vertices = allVertices;
        mesh.triangles = allTriangles.ToArray();
        mesh.colors = colors;
        mesh.uv = uvs;
        
        mesh.RecalculateNormals(); 
        mesh.RecalculateBounds();  

        // 5. Update Shader Tiling (Grid)
        if (meshRenderer.material.HasProperty("_Tiling"))
        {
            // If gridStepSize is 1, we tile every 1 unit
            meshRenderer.material.SetVector("_Tiling", new Vector4(1.0f / gridStepSize, 1.0f / gridStepSize, 0, 0));
        }
        // Fallback for standard URP Lit (uses _BaseMap_ST)
        else if (meshRenderer.material.HasProperty("_BaseMap_ST"))
        {
            meshRenderer.material.SetTextureScale("_BaseMap", new Vector2(1.0f / gridStepSize, 1.0f / gridStepSize));
        }
    }
}