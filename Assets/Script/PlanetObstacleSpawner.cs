using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetObstacleSpawner : MonoBehaviour
{
    public PointEffector2D gravitySource;

    public Obstacle[] obstaclePrefabs;

    public int obstacleCount;
    public float minHeight;
    public float maxHeight;
    public float obstacleVelocity = 2f;
    public float timeBetweenSpawns = 3.5f;
    public float randomVelAngle = 15f;


    private void Start()
    {
        StartCoroutine(SpawnObstacleRoutine());
    }


    //Vector2 CalculateOrbitalVelocity(Vector2 position, float mass)
    //{

    //    float orbitalSpeed = Mathf.Sqrt(Mathf.Abs(gravitySource.forceMagnitude) * ((Vector2)transform.position - position).magnitude / mass);
    //    //Debug.Log($"float {orbitalSpeed} = Mathf.Sqrt({gravitySource.forceMagnitude} * {((Vector2)transform.position - position).magnitude} / {mass})");
    //    Vector2 delta = ((Vector2)transform.position - position).normalized;
    //    Vector2 tangent = Quaternion.Euler(0, 0, 90) * delta;
    //    return tangent * orbitalSpeed;
    //}

    //void SpawnObstacles()
    //{
    //    float angleBetweenSpawns = 360f / obstacleCount;
    //    for (int i = 1; i <= obstacleCount; i++)
    //    {
    //        float height = Random.Range(minHeight, maxHeight);
    //        Quaternion spawnAngle = Quaternion.Euler(0, 0, angleBetweenSpawns * i);
    //        Vector3 spawnPos = transform.position + spawnAngle * new Vector2(0, height * transform.localScale.y);

    //        Obstacle obs = Instantiate(obstaclePrefab, spawnPos, spawnAngle);

    //        int direction = Random.Range(0, 2) == 1 ? -1 : 1;
    //        //obs.velocity = direction * CalculateOrbitalVelocity(spawnPos, obs.mass);
    //        obs.velocity = (transform.position - spawnPos).normalized * obstacleVelocity;
    //        obs.angularVelocity = direction * Random.Range(0f, 30f);
    //    }
    //}

    void SpawnObstacles(float angle)
    {
        float height = Random.Range(minHeight, maxHeight);
        Quaternion spawnAngle = Quaternion.Euler(0, 0, angle);
        Vector3 spawnPos = transform.position + spawnAngle * new Vector2(0, height * transform.localScale.y);

        Obstacle obstaclePrefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];

        Obstacle obs = Instantiate(obstaclePrefab, spawnPos, spawnAngle);

        int direction = Random.Range(0, 2) == 1 ? -1 : 1;
        //obs.velocity = direction * CalculateOrbitalVelocity(spawnPos, obs.mass);
        obs.velocity = Quaternion.Euler(0,0,Random.Range(-randomVelAngle, randomVelAngle)) * (transform.position - spawnPos).normalized * obstacleVelocity;
        obs.angularVelocity = direction * Random.Range(0f, 30f);
    }


    IEnumerator SpawnObstacleRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeBetweenSpawns);
            SpawnObstacles(Random.Range(0, 360f));
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, minHeight * transform.localScale.y);
        Gizmos.DrawWireSphere(transform.position, maxHeight * transform.localScale.y);
    }
#endif

    // f = mv2/r
    // v2 = fr/m
}
