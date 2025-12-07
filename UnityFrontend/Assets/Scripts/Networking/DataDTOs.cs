// DTO = Data Transfer Object. These C# classes *must* match the
// JSON structure coming from Python.
using System;
using System.Collections.Generic; // For Dictionaries

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
    public List<float> w; // [new_w0, new_w1] (Python .tolist() sends a List)
    public float cost;
}

// This is what we send *to* Python
[Serializable]
public class StepDataRequest
{
    public string model;
    public string data_id;
    public float[] current_w;
    public string algorithm;
    
    // We need to send the hyperparameters now
    public HyperparameterData hyperparameters;
}

// A flexible class to hold hyperparameters
[Serializable]
public class HyperparameterData
{
    // Add any hyperparam you need.
    // Make sure the name (e.g., "learning_rate") matches
    // the key used in the Python algorithm classes.
    public float learning_rate;
    public int resolution; // For BruteForce
}