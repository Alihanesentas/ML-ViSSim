// DTO = Data Transfer Object. These C# classes *must* match the
[cite_start]// JSON structure coming from Python .
using System;

[Serializable]
public class VertexData
{
    public float x; // w0
    public float y; // cost
    public float z; // w1
}

[Serializable]
public class SurfaceDataResponse // For /get_cost_surface
{
    public VertexData[] vertices;
    public int[] triangles;
}

[Serializable]
public class StepDataResponse // For /calculate_next_step
{
    public float[] w; // [new_w0, new_w1]
    public float cost;
}

[cite_start]// This is what we send *to* Python 
[Serializable]
public class StepDataRequest
{
    public string model;
    public string data_id;
    public float[] current_w;
    public string algorithm;
    public float learning_rate;
}