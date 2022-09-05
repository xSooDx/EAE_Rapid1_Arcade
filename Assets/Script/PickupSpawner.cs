using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    public float maxHeightDelta = 0.05f;
    public PickupSpawnSettings[] pickupSettings;

    public int minPickups = 3;
    public int maxPickups = 6;

    float totalWeight = 0;

    void Awake()
    {

    }

    public void OnTerrainGeneratedCallback(TerrainGenerator1D terrainGenerator, Vector2[] terrainPointsLocal)
    {
        List<Vector2> potentialSpawnPoints = new List<Vector2>();
        for(int i = 0; i < terrainPointsLocal.Length-1; i++)
        {
            if (Mathf.Abs(terrainPointsLocal[i].y - terrainPointsLocal[i+1].y) < maxHeightDelta)
            {
                Vector2 newPos = (terrainPointsLocal[i] + terrainPointsLocal[i + 1]) / 2;
                potentialSpawnPoints.Add(newPos);
                Debug.DrawRay(newPos, Vector2.up, Color.yellow);
            }
        }

        int pickupCount = Mathf.Min(Random.Range(minPickups, maxPickups), potentialSpawnPoints.Count);

        int randRange = pickupCount / potentialSpawnPoints.Count;
        int currStart = 0;

        CalculateTotalWeight();
        float currWeightVal = 0f;
        for (int i = 0; i< pickupCount; i++)
        {
            int idx = Random.Range(currStart, currStart + randRange);

            Vector2 spawnPoint = potentialSpawnPoints[idx];
            float randRoll = Random.Range(0, totalWeight);
            foreach(PickupSpawnSettings pickup in pickupSettings)
            {
                if(randRoll < currWeightVal + pickup.spawnWeight)
                {
                    // ToDo - Store and destroy
                    Instantiate(pickup.spawnPrefab, spawnPoint, Quaternion.identity);
                    break;
                }
                currWeightVal += pickup.spawnWeight;
            }

            currStart += randRange;
        }
    }

    void CalculateTotalWeight()
    {
        totalWeight = 0;
        foreach (PickupSpawnSettings pickup in pickupSettings)
        {
            totalWeight += pickup.spawnWeight;
        }
    }

    private void OnValidate()
    {
        CalculateTotalWeight();
    }
}

[System.Serializable]
public class PickupSpawnSettings
{
    public float spawnWeight = 1;
    public int scoreMultiplier = 1;
    public GameObject spawnPrefab;
}
