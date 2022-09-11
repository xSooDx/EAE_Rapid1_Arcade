using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    public float maxHeightDelta = 0.05f;
    public float dropOffZoneHeight = 10f;
    public float dropOffZoneCount = 10f;
    public float spawnOffsetAngle = 30f;
    public PickupSpawnSettings[] pickupSettings;

    public DropOffZoneMovementScript dropOffPrefab;

    public int minPickups = 3;
    public int maxPickups = 6;

    float totalWeight = 0;

    public bool off;

    void Awake()
    {

    }

    public void OnTerrainGeneratedCallback(TerrainGenerator1D terrainGenerator, Vector2[] terrainPointsLocal)
    {
        if (off) return;
        List<Vector2> potentialSpawnPoints = new List<Vector2>();
        float sqMaxHeightDelta = maxHeightDelta * maxHeightDelta;
        for (int i = 0; i < terrainPointsLocal.Length-1; i++)
        {
            //if (Mathf.Abs(terrainPointsLocal[i].sqrMagnitude - terrainPointsLocal[i+1].sqrMagnitude) < maxHeightDelta)
            //{
                Vector2 newPos = (terrainPointsLocal[i] + terrainPointsLocal[i + 1]) / 2;
                potentialSpawnPoints.Add(newPos);
                //Debug.DrawRay(newPos, Vector2.up, Color.yellow);
            //}
        }

        potentialSpawnPoints.Add((terrainPointsLocal[terrainPointsLocal.Length - 1] + terrainPointsLocal[0]) / 2);

        int dropOffPointIdx = potentialSpawnPoints.Count / 2;

        potentialSpawnPoints.RemoveAt(dropOffPointIdx);

        int pickupCount = Mathf.Min(Random.Range(minPickups, maxPickups), potentialSpawnPoints.Count);

        int randRange = potentialSpawnPoints.Count/ pickupCount;
        int currStart = 0;

        CalculateTotalWeight();
        float currWeightVal = 0f;
        //Debug.Log($"Pickup spawner: pickupCount: {pickupCount}, potentialSpawnPoints: {potentialSpawnPoints.Count}, Range: {randRange}");
        for (int i = 0; i< pickupCount; i++)
        {
            int idx = Random.Range(currStart, currStart + randRange);
            Vector2 dir = terrainGenerator.planetTerrain? (potentialSpawnPoints[idx]).normalized : transform.up;

            Vector2 spawnPoint = ((potentialSpawnPoints[idx] + dir * 0.2f) * transform.localScale) + (Vector2)transform.position;
            Debug.DrawRay(spawnPoint, dir, Color.red, 10f);
            float randRoll = Random.Range(0, totalWeight);
            //Debug.Log($"SOOD: 4 {spawnPoint} {randRoll}");
            foreach (PickupSpawnSettings pickup in pickupSettings)
            {
                if(randRoll < currWeightVal + pickup.spawnWeight)
                {
                    // ToDo - Store and destroy
                    Instantiate(pickup.spawnPrefab, spawnPoint, Quaternion.identity);
                    break;
                }

                currWeightVal += pickup.spawnWeight;
            }
            currWeightVal = 0f;
            currStart += randRange;
        }

        SpawnDropOffZone();
    }

    void SpawnDropOffZone()
    {
        float angleBetweenSpawns = 360f / dropOffZoneCount;
        for(int i = 0; i< dropOffZoneCount; i++)
        {
            Quaternion spawnAngle = Quaternion.Euler(0, 0, spawnOffsetAngle + angleBetweenSpawns * i);
            Vector3 spawnPos = transform.position + spawnAngle * new Vector2(0, dropOffZoneHeight * transform.localScale.y);

            DropOffZoneMovementScript dropoff = Instantiate(dropOffPrefab, spawnPos, spawnAngle);

            dropoff.orbitTarget = transform;
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

#if UNITY_EDITOR
    private void OnValidate()
    {
        CalculateTotalWeight();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, dropOffZoneHeight * transform.localScale.y);
    }
#endif
}

[System.Serializable]
public class PickupSpawnSettings
{
    public float spawnWeight = 1;
    public int scoreMultiplier = 1;
    public GameObject spawnPrefab;
}
