using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D), typeof(MeshFilter), typeof(MeshRenderer))]
public class TerrainGenerator1D : MonoBehaviour
{
    [Min(0)] public int startingIdx = 0;
    [Min(0)] int numberOfPoints = 50;
    [Min(0)] public float terrainWidth = 0;
    [Range(0f, 10f)] public float distanceBetweenPoints = 1f;
    public float maxHeight = 20f;
    public float minHeight = 10f;
    [Range(0f, 1f)] public float noiseSampleSeed = 0.5f;
    public NoiseLayer[] noiseLayers;
    public float meshDepth = 10f;


    float[] heightMap;

    [SerializeField] EdgeCollider2D edgeCollider;
    [SerializeField] MeshFilter meshFilter;

    // Total width
    // offset 

    private void Awake()
    {

    }

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void GetRequiredComponents()
    {
        edgeCollider = GetComponent<EdgeCollider2D>();
        edgeCollider.points = null;
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = null;
    }

    void DoPreCalculations()
    {
        //terrainWidth = numberOfPoints * distanceBetweenPoints;

        numberOfPoints = (int)(terrainWidth / distanceBetweenPoints);
    }

    void GenerateHeightMap()
    {
        heightMap = new float[numberOfPoints];

        for (int i = 0; i < heightMap.Length; i++)
        {
            float value = 0;
            foreach (NoiseLayer layer in noiseLayers)
            {
                value += SimplexNoise.Generate(layer.noisefrequency * distanceBetweenPoints * (startingIdx + i), noiseSampleSeed) * layer.noiseStrength;
            }
            heightMap[i] = Mathf.Clamp(value, minHeight, maxHeight); ;
        }
    }

    void SmoothHeightMap()
    {
        float[] smoothKernel = { 1, 3, 1 };
        int kernelHalfSize = smoothKernel.Length / 2;
        float kernelSum = 0;

        for (int i = 0; i < smoothKernel.Length; i++)
        {
            kernelSum += smoothKernel[i];
        }

        float[] newHeightMap = new float[heightMap.Length];

        for (int i = kernelHalfSize; i < heightMap.Length - kernelHalfSize; i++)
        {
            int i_ = i - kernelHalfSize;
            float value = 0;
            for (int j = 0; j < smoothKernel.Length; j++)
            {
                value += heightMap[i_ + j] * smoothKernel[j];
            }
            newHeightMap[i] = value / kernelSum;
        }
        heightMap = newHeightMap;
    }

    void SmoothHeightMap2()
    {
        if (heightMap == null || heightMap.Length == 0)
        {
            Debug.LogError("GenerateCollider: Height map is null");
            return;
        }
        float[] newHeightMap = new float[heightMap.Length];
        newHeightMap[0] = heightMap[0];
        newHeightMap[heightMap.Length - 1] = heightMap[heightMap.Length - 1];

        for (int i = 1; i < heightMap.Length - 1; i++)
        {
            float diff1 = heightMap[i] - heightMap[i - 1];
            float diff2 = heightMap[i] - heightMap[i + 1];
            if (diff1 > 0 && diff2 > 0) // local maxima
            {
                newHeightMap[i] = diff1 < diff2 ? heightMap[i - 1] : heightMap[i + 1];
            }
            else
            {
                newHeightMap[i] = heightMap[i];
            }
        }

        heightMap = newHeightMap;
    }

    void GenerateCollider()
    {
        if (heightMap == null || heightMap.Length == 0)
        {
            Debug.LogError("GenerateCollider: Height map is null");
            return;
        }
        List<Vector2> pointList = new List<Vector2>();
        for (int i = 0; i < heightMap.Length; i++)
        {
            Vector2 p1 = new Vector3(GetPointLocalXPosition(i), heightMap[i]);
            pointList.Add(p1);
        }
        edgeCollider.points = pointList.ToArray();
    }

    void GenerateMesh()
    {
        if (heightMap == null || heightMap.Length == 0)
        {
            Debug.LogError("GenerateCollider: Height map is null");
            return;
        }

        Mesh mesh = new Mesh();
        List<Vector3> vertexList = new List<Vector3>();
        for (int i = 0; i < heightMap.Length; i++)
        {
            Vector2 p1 = new Vector3(GetPointLocalXPosition(i), heightMap[i]);
            Vector2 p2 = new Vector3(GetPointLocalXPosition(i), -meshDepth);
            vertexList.Add(p1);
            vertexList.Add(p2);
        }

        List<int> triangleList = new List<int>();

        for (int i = 0; i < vertexList.Count - 2; i += 2)
        {
            triangleList.Add(i);
            triangleList.Add(i + 2);
            triangleList.Add(i + 1);

            triangleList.Add(i + 2);
            triangleList.Add(i + 3);
            triangleList.Add(i + 1);
        }
        //Debug.Log($"Mesh Gen tris {triangleList.Count}, verts {vertexList.Count}");
        mesh.vertices = vertexList.ToArray();
        mesh.triangles = triangleList.ToArray();
        meshFilter.mesh = mesh;

    }

    float GetPointLocalXPosition(int idx)
    {
        return distanceBetweenPoints * idx - (terrainWidth / 2);
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        GetRequiredComponents();
        DoPreCalculations();
        GenerateHeightMap();
        //SmoothHeightMap();
        SmoothHeightMap2();
        GenerateCollider();
        GenerateMesh();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        if (heightMap != null && heightMap.Length > 0)
        {
            for (int i = 0; i < heightMap.Length - 1; i++)
            {
                Vector2 p1 = transform.position + new Vector3(GetPointLocalXPosition(i), heightMap[i]);
                Vector2 p2 = transform.position + new Vector3(GetPointLocalXPosition(i + 1), heightMap[i + 1]);
                Gizmos.DrawLine(p1, p2);
            }
        }
    }
#endif
}

[System.Serializable]
public class NoiseLayer
{
    [Range(0f, 100f)] public float noiseStrength = 10f;
    [Range(0f, 100f)] public float noisefrequency = 1f;
}
