using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(EdgeCollider2D), typeof(MeshFilter), typeof(MeshRenderer))]
public class TerrainGenerator1D : MonoBehaviour
{

    [InspectorButton("GenerateTerrain", ButtonWidth =250)]
    public bool _generateTerrain;


    [Header("Generator Settings")]
    [Min(0)] public int startingIdx = 0;
    [Min(0)] int numberOfPoints = 50;
    [Min(0)] public float terrainWidth = 0;
    [Range(0f, 4f)] public float distanceBetweenPoints = 1f;
    public float maxHeight = 20f;
    public float minHeight = 10f;
    [Range(0f, 1f)] public float noiseSampleSeed = 0.5f;

    [Header("Noise Layers")]
    public NoiseLayer[] noiseLayers;

    [Range(0f, 10f)] public float meshDepth = 10f;

    [Header("Texture Settings")]
    public Vector2 textureScale;
    public Vector2 textureOffset;

    [InspectorButton("GenerateForeground", ButtonWidth = 250)]
    public bool _generateForeground;

    [Header("Foreground Textures")]
    public ForegroundLayer[] foregroundLayers;

    float[] heightMap;
    float lowestPoint = float.MaxValue;
    float highestPoint = float.MinValue;

    [Header("Components")]
    [SerializeField] EdgeCollider2D edgeCollider;
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] MeshRenderer meshRenderer;

    GameObject[] foregroundObjs = null;

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
        meshRenderer = GetComponent<MeshRenderer>();
        //meshRenderer.material = null;

    }

    void DoPreCalculations()
    {
        //terrainWidth = numberOfPoints * distanceBetweenPoints;

        numberOfPoints = (int)(terrainWidth / distanceBetweenPoints);

        lowestPoint = float.MaxValue;
        highestPoint = float.MinValue;
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
            
            if(heightMap[i] < lowestPoint)
            {
                lowestPoint = heightMap[i];
            }

            if(heightMap[i] > highestPoint)
            {
                highestPoint = heightMap[i]; ;
            }
        }
    }

    void SmoothHeightMap()
    {
        if (heightMap == null || heightMap.Length == 0)
        {
            Debug.LogWarning("SmoothHeightMap: Height map is null");
            return;
        }
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
            Debug.LogWarning("SmoothHeightMap2: Height map is null");
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
            //else if (diff1 < 0 && diff2 < 0) // local minima
            //{
            //    newHeightMap[i] = diff1 < diff2 ? heightMap[i - 1] : heightMap[i + 1];
            //}
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
            Debug.LogWarning("GenerateCollider: Height map is null");
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
            Debug.LogWarning("GenerateMesh: Height map is null");
            return;
        }

        Mesh mesh = new Mesh();
        
        // Generate verts
        List<Vector3> vertexList = new List<Vector3>();
        for (int i = 0; i < heightMap.Length; i++)
        {
            Vector3 p1 = new Vector3(GetPointLocalXPosition(i), heightMap[i]);
            Vector3 p2 = new Vector3(GetPointLocalXPosition(i), lowestPoint - meshDepth);
            vertexList.Add(p1);
            vertexList.Add(p2);
        }

        // Generate Tris
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

        // Generate UVs
        List<Vector2> uvList = new List<Vector2>();
        float factor = distanceBetweenPoints / heightMap.Length;
        for (int i = 0, j=0; i < vertexList.Count; i += 2, j++)
        {
            float uvX = j * textureScale.x;
            float uvY1 = textureScale.y * (vertexList[i].y - vertexList[i+1].y) / distanceBetweenPoints;
            uvList.Add(new Vector2(uvX, uvY1));
            uvList.Add(new Vector2(uvX, 0));
        }

        //Debug.Log($"Mesh Gen tris {triangleList.Count}, verts {vertexList.Count}");
        mesh.vertices = vertexList.ToArray();
        mesh.triangles = triangleList.ToArray();
        mesh.uv = uvList.ToArray();
        meshFilter.mesh = mesh;

    }

    void GenerateForeground()
    {
        if(foregroundObjs != null)
        {
            foreach(GameObject obj in foregroundObjs)
            {
                if(obj) DestroyImmediate(obj);
            }
        }

        foregroundObjs = new GameObject[foregroundLayers.Length];

        for(int i = 0; i < foregroundLayers.Length; i++)
        {
            ForegroundLayer fgLayer = foregroundLayers[i];
            GameObject obj = new GameObject($"ForegroundLayer {i + 1}");
            obj.transform.position = transform.position + new Vector3( 0, fgLayer.yOffset, fgLayer.zOffset);
            obj.transform.parent = transform;

            Vector3[] vertList = {
                new Vector3(GetPointLocalXPosition(0), lowestPoint),
                new Vector3(GetPointLocalXPosition(0), lowestPoint - meshDepth),
                new Vector3(GetPointLocalXPosition(heightMap.Length-1), lowestPoint),
                new Vector3(GetPointLocalXPosition(heightMap.Length-1),  lowestPoint - meshDepth),
            };

            int[] tris = {
                0,2,1,2,3,1
            };
            // ToDo better scaling

            float textureRatio = terrainWidth / meshDepth;

            Vector2[] uvList =
            {
                new Vector2(0, 1) * fgLayer.scale,
                new Vector2(0, 0),
                new Vector2(textureRatio, 1) * fgLayer.scale,
                new Vector2(textureRatio, 0) * fgLayer.scale,
            };

            MeshFilter mf = obj.AddComponent<MeshFilter>();
            MeshRenderer mr = obj.AddComponent<MeshRenderer>();
            Mesh mesh = new Mesh();
            mesh.vertices = vertList;
            mesh.triangles = tris;
            mesh.uv = uvList;

            mf.mesh = mesh;
            mr.material = fgLayer.fgMaterial;
            foregroundObjs[i] = obj;

        }
        //Material material = new Material();
        //meshRenderer.
    }

    float GetPointLocalXPosition(int idx)
    {
        return distanceBetweenPoints * idx - (terrainWidth / 2);
    }

    public void GenerateTerrain()
    {
        GetRequiredComponents();
        DoPreCalculations();
        GenerateHeightMap();
        SmoothHeightMap2();
        GenerateCollider();
        GenerateMesh();
        GenerateForeground();
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        GetRequiredComponents();
        DoPreCalculations();
        GenerateHeightMap();
        SmoothHeightMap2();
        GenerateCollider();
        GenerateMesh();
    }

    private void OnDrawGizmosSelected()
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
    [Range(0f, 10f)] public float noisefrequency = 1f;
}

[System.Serializable]
public class ForegroundLayer
{
    public Vector2 scale = new Vector2(1,1);
    public Material fgMaterial;
    public float yOffset;
    public float zOffset;
}