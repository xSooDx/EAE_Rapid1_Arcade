using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(EdgeCollider2D), typeof(MeshFilter), typeof(MeshRenderer))]
[RequireComponent(typeof(LineRenderer))]
public class TerrainGenerator1D : MonoBehaviour
{
    public UnityEvent<TerrainGenerator1D, Vector2[]> onTerrainGenerated;

    [Header("Generator Settings")]

    public bool planetTerrain = false;
    public float topLayerThickness = 1f;

    [Header("Planet Terrain Settings")]
    [Min(0)] public float planetRadius = 0;
    [Range(0.01f, 5f)] public float angleBetweenPoints = 1f;
    [Min(1)]public int foregroundResolution = 10;

    [Header("Flat Terrain Settings")]
    [Min(0)] public float terrainWidth = 0;
    [Range(0f, 4f)] public float distanceBetweenPoints = 1f;

    public float maxHeight = 20f;
    public float minHeight = 10f;
    [Min(0)] public int startingIdx = 0;
    [Range(0f, 1f)] public float noiseSampleSeed = 0.5f;

    [Header("Noise Layers")]
    public NoiseLayer[] noiseLayers;

    [Range(0f, 10f)] public float meshDepth = 10f;

    [Header("Texture Settings")]
    public Vector2 textureScale;
    public Vector2 textureOffset;

    public bool randomForegroundOffset = false;

    [InspectorButton("GenerateTerrainInternal", ButtonWidth = 250f)]
    public bool _generateTerrain;

    [Header("Foreground Textures")]
    public ForegroundLayer[] foregroundLayers;

    [Header("Terrain Morph Settings")]
    public float morphSpeed = 10f;
    public float morphDuration = 2f;
    public float morphInterval = 10f;

    int numberOfPoints = 0;
    float[] heightMap;
    float lowestPoint = float.MaxValue;
    float highestPoint = float.MinValue;

    [Header("Components")]
    [SerializeField] EdgeCollider2D edgeCollider;
    [SerializeField] MeshFilter meshFilter;
    [SerializeField] MeshRenderer meshRenderer;
    [SerializeField] LineRenderer lineRenderer;

    private void Start()
    {
        GenerateTerrain(true, true);

        StartCoroutine(MorphTerrain());
    }

    public Vector2[] GetLocalTerrainPoints()
    {
        return edgeCollider.points;
    }

    public Vector3[] GetLocalTerrainPoints3D()
    {
        Vector3[] positions = new Vector3[lineRenderer.positionCount];
        lineRenderer.GetPositions(positions);
        return positions;
    }


    void GetRequiredComponents()
    {
        edgeCollider = GetComponent<EdgeCollider2D>();
        edgeCollider.points = null;
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = null;
        meshRenderer = GetComponent<MeshRenderer>();
        lineRenderer = GetComponent<LineRenderer>();
    }

    void DoPreCalculations()
    {
        //terrainWidth = numberOfPoints * distanceBetweenPoints;

        if(planetTerrain)
        {
            numberOfPoints = Mathf.FloorToInt(360f / angleBetweenPoints);
        }
        else
        {
            numberOfPoints = (int)(terrainWidth / distanceBetweenPoints);
        }

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
            else if (diff1 < 0 && diff2 < 0) // local minima
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

    Vector2[] GenerateTerrainEdge()
    {
        if (heightMap == null || heightMap.Length == 0)
        {
            Debug.LogWarning("GenerateCollider: Height map is null");
            return null;
        }
        List<Vector2> pointList = new List<Vector2>();
        for (int i = 0; i < heightMap.Length; i++)
        {
            Vector2 p1 = new Vector2(GetPointLocalXPosition(i), heightMap[i]);
            pointList.Add(p1);
        }
        return pointList.ToArray();
    }

    Vector2[] GeneratePlanetEdge()
    {
        if (heightMap == null || heightMap.Length == 0)
        {
            Debug.LogWarning("GenerateCollider: Height map is null");
            return null;
        }
        List<Vector2> pointList = new List<Vector2>();
        for (int i = 0; i < heightMap.Length; i++)
        {
            Vector2 position = GetPointLocalPlanetPosition(i);
            pointList.Add(position);
        }
        pointList.Add(pointList[0]);
         return pointList.ToArray();
    }

    Mesh GenerateMesh()
    {
        if (heightMap == null || heightMap.Length == 0)
        {
            Debug.LogWarning("GenerateMesh: Height map is null");
            return null;
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
        return mesh;
        
    }

    Mesh GeneratePlanetMesh()
    {
        if (heightMap == null || heightMap.Length == 0)
        {
            Debug.LogWarning("GenerateMesh: Height map is null");
            return null;
        }

        Mesh mesh = new Mesh();
        float terrainDepth = planetRadius - meshDepth;
        // Generate verts
        List<Vector3> vertexList = new List<Vector3>();
        for (int i = 0; i < heightMap.Length; i++)
        {
            Vector3 p1 = GetPointLocalPlanetPosition(i);
            Vector3 p2 = GetPointOnCircle(i) * terrainDepth;
            vertexList.Add(p1);
            vertexList.Add(p2);
        }
        
        // Close the loop
        Vector3 p1_ = GetPointLocalPlanetPosition(0);
        Vector3 p2_ = GetPointOnCircle(0) * terrainDepth;
        vertexList.Add(p1_);
        vertexList.Add(p2_);

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
        float circumfence = 2 * Mathf.PI * (planetRadius);
        float distanceBetweenPoints = circumfence / numberOfPoints;
        for (int i = 0, j = 0; i < vertexList.Count; i += 2, j++)
        {
            float uvX = j * textureScale.x;
            float uvY1 = textureScale.y * (vertexList[i].magnitude - vertexList[i + 1].magnitude) / distanceBetweenPoints;
            uvList.Add(new Vector2(uvX, uvY1));
            uvList.Add(new Vector2(uvX, 0));
        }

        //Debug.Log($"Mesh Gen tris {triangleList.Count}, verts {vertexList.Count}");
        mesh.vertices = vertexList.ToArray();
        mesh.triangles = triangleList.ToArray();
        mesh.uv = uvList.ToArray();
        return mesh;
    }

    void RemoveForeground()
    {
        int childIdx = 0;
        GameObject childObj;
        while (childIdx < transform.childCount)
        {
            childObj = transform.GetChild(childIdx).gameObject;

            if (childObj)
            {
                DestroyImmediate(childObj);
            }
            else
            {
                childIdx++;
            }
        }
    }

    void GenerateForeground()
    {
        RemoveForeground();

        for (int i = 0; i < foregroundLayers.Length; i++)
        {
            ForegroundLayer fgLayer = foregroundLayers[i];
            GameObject obj = new GameObject($"ForegroundLayer {i + 1}");
            obj.transform.position = transform.position + new Vector3( 0, fgLayer.yOffset, fgLayer.zOffset);
            obj.transform.localScale = transform.localScale;
            obj.transform.rotation = transform.rotation;
            obj.transform.parent = transform;

            Vector3[] vertList = {
                new Vector3(GetPointLocalXPosition(0), lowestPoint),
                new Vector3(GetPointLocalXPosition(0), lowestPoint - meshDepth),
                new Vector3(GetPointLocalXPosition(heightMap.Length-1), lowestPoint),
                new Vector3(GetPointLocalXPosition(heightMap.Length-1), lowestPoint - meshDepth),
            };

            int[] tris = {
                0,2,1,2,3,1
            };
            // ToDo better scaling

            float randomOffset = randomForegroundOffset ? Random.Range(0f, 1f) : 0f;

            float textureRatio = terrainWidth / meshDepth;

            Vector2[] uvList =
            {
                new Vector2(randomOffset, 1 * fgLayer.scale.y) ,
                new Vector2(randomOffset, 0),
                new Vector2(randomOffset + textureRatio, 1) * fgLayer.scale,
                new Vector2(randomOffset + textureRatio, 0) * fgLayer.scale,
            };

            MeshFilter mf = obj.AddComponent<MeshFilter>();
            MeshRenderer mr = obj.AddComponent<MeshRenderer>();
            Mesh mesh = new Mesh();
            mesh.vertices = vertList;
            mesh.triangles = tris;
            mesh.uv = uvList;

            mf.mesh = mesh;
            mr.material = fgLayer.fgMaterial;
        }
        //Material material = new Material();
        //meshRenderer.
    }

    void GeneratePlanetForeground()
    {
        RemoveForeground();

        int foregroundVertexCount = foregroundResolution + 3;
        float angelBetweenVerts = 360f / foregroundVertexCount;

        for (int i = 0; i < foregroundLayers.Length; i++)
        {
            ForegroundLayer fgLayer = foregroundLayers[i];
            float fgLayerHeight = (planetRadius - meshDepth + fgLayer.yOffset);

            GameObject obj = new GameObject($"ForegroundLayer {i + 1}");
            obj.transform.position = transform.position + new Vector3(0, 0, fgLayer.zOffset);
            obj.transform.localScale = transform.localScale;
            obj.transform.rotation = transform.rotation;
            obj.transform.parent = transform;

            List<Vector3> vertexList = new List<Vector3>();
            for (int j = 0; j < foregroundVertexCount; j++)
            {
                Vector3 p1 = GetPointOnCircle(j, angelBetweenVerts);
                vertexList.Add(p1 * fgLayerHeight);
                vertexList.Add(p1 * fgLayer.yDepth);
            }

            // Close the loop
            Vector3 p1_ = GetPointOnCircle(0, angelBetweenVerts);
            vertexList.Add(p1_ * fgLayerHeight);
            vertexList.Add(p1_ * fgLayer.yDepth);

            List<int> triangleList = new List<int>();
            for (int j = 0; j < vertexList.Count - 2; j += 2)
            {
                triangleList.Add(j);
                triangleList.Add(j + 2);
                triangleList.Add(j + 1);

                triangleList.Add(j + 2);
                triangleList.Add(j + 3);
                triangleList.Add(j + 1);
            }

            // ToDo better scaling

            //float randomOffset = randomForegroundOffset ? Random.Range(0f, 1f) : 0f;

            //float textureRatio = terrainWidth / meshDepth;

            List<Vector2> uvList = new List<Vector2>();

            float scaleFactor = 1f / (foregroundVertexCount);
            float randomOffset = randomForegroundOffset ? Random.Range(0f, 1f) : 0f;

            for (int j = 0, k = 0; j < vertexList.Count; j += 2, k++)
            {
                float uvX = randomOffset + k * fgLayer.scale.x * scaleFactor;
                float uvY1 = fgLayer.scale.y;
                uvList.Add(new Vector2(uvX, uvY1));
                uvList.Add(new Vector2(uvX, 0));
            }

            MeshFilter mf = obj.AddComponent<MeshFilter>();
            MeshRenderer mr = obj.AddComponent<MeshRenderer>();
            Mesh mesh = new Mesh();
            mesh.vertices = vertexList.ToArray();
            mesh.triangles = triangleList.ToArray();
            mesh.uv = uvList.ToArray();

            mf.mesh = mesh;
            mr.material = fgLayer.fgMaterial;

        }
        //Material material = new Material();
        //meshRenderer.
    }

    void GenerateTopLayer()
    {
        if (heightMap == null || heightMap.Length == 0)
        {
            Debug.LogWarning("GenerateCollider: Height map is null");
            return;
        }
        List<Vector3> pointList = new List<Vector3>();
        for (int i = 0; i < heightMap.Length; i++)
        {
            Vector3 p1 = new Vector3(GetPointLocalXPosition(i), heightMap[i]);
            pointList.Add(p1);
        }
        lineRenderer.loop = false;
        lineRenderer.useWorldSpace = false;
        lineRenderer.positionCount = pointList.Count;
        lineRenderer.SetPositions(pointList.ToArray());
    }

    void GeneratePlanetTopLayer()
    {
        if (heightMap == null || heightMap.Length == 0)
        {
            Debug.LogWarning("GenerateCollider: Height map is null");
            return;
        }
        List<Vector3> pointList = new List<Vector3>();
        for (int i = 0; i < heightMap.Length; i++)
        {
            Vector3 p1 = GetPointLocalPlanetPosition(i);
            pointList.Add(p1);
        }
        Vector3 p = GetPointLocalPlanetPosition(0);
        pointList.Add(p);
        lineRenderer.loop = false;
        lineRenderer.useWorldSpace = false;
        lineRenderer.positionCount = pointList.Count;
        lineRenderer.widthMultiplier = topLayerThickness;
        lineRenderer.SetPositions(pointList.ToArray());
    }

    float GetPointLocalXPosition(int idx)
    {
        return distanceBetweenPoints * idx - (terrainWidth / 2);
    }

    Vector2 GetPointLocalPlanetPosition(int idx)
    {

        Vector2 position = GetPointOnCircle(idx) * (planetRadius + heightMap[idx]);
        return position;
    }

    Vector2 GetPointOnCircle(int idx)
    {
        return GetPointOnCircle(idx, angleBetweenPoints);
    }

    Vector2 GetPointOnCircle(int idx, float angleBetweenPoints)
    {
        float angle = angleBetweenPoints * idx;
        float x = Mathf.Sin(Mathf.Deg2Rad * angle);
        float y = Mathf.Cos(Mathf.Deg2Rad * angle);

        return new Vector2(x, y);
    }

    void GenerateTerrainInternal()
    {
        GenerateTerrain(true, false);
    }

    public void GenerateTerrain(bool generateForeground, bool doCallback)
    {
        GetRequiredComponents();
        DoPreCalculations();
        GenerateHeightMap();
        SmoothHeightMap();
        
        if (planetTerrain)
        {
            edgeCollider.points = GeneratePlanetEdge();
            meshFilter.mesh = GeneratePlanetMesh();
            GeneratePlanetTopLayer();
            if (generateForeground) GeneratePlanetForeground();
        }
        else
        {
            edgeCollider.points = GenerateTerrainEdge();
            meshFilter.mesh = GenerateMesh();
            GenerateTopLayer();
            if (generateForeground) GenerateForeground();
        }

        if(doCallback) onTerrainGenerated.Invoke(this, GetLocalTerrainPoints());
    }

    IEnumerator MorphTerrain()
    {
        
        while (true)
        {
            yield return new WaitForSeconds(morphInterval);
            float timer = morphDuration;
            while (timer > 0)
            {
                noiseSampleSeed += morphSpeed * Time.deltaTime;
                GenerateTerrain(false, false);
                timer -= Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }
        }
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        GenerateTerrain(false, false);
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
    public float yDepth;    
}