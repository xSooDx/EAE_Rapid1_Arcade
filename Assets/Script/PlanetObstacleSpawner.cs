using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetObstacleSpawner : MonoBehaviour
{
    public PointEffector2D gravitySource;

    public Obstacle obstaclePrefab;

    public int obstacleCount;
    public float minHeight;
    public float maxHeight;


    private void Start()
    {
        SpawnObstacle();
    }


    Vector2 CalculateOrbitalVelocity(Vector2 position, float mass)
    {

        float orbitalSpeed = Mathf.Sqrt(Mathf.Abs(gravitySource.forceMagnitude) * ((Vector2)transform.position - position).magnitude / mass);
        //Debug.Log($"float {orbitalSpeed} = Mathf.Sqrt({gravitySource.forceMagnitude} * {((Vector2)transform.position - position).magnitude} / {mass})");
        Vector2 delta = ((Vector2)transform.position - position).normalized;
        Vector2 tangent = Quaternion.Euler(0, 0, 90) * delta;
        return tangent * orbitalSpeed;
    }

    void SpawnObstacle()
    {
        float angleBetweenSpawns = 360f / obstacleCount;
        for (int i = 1; i <= obstacleCount; i++)
        {
            float height = Random.Range(minHeight, maxHeight);
            Quaternion spawnAngle = Quaternion.Euler(0, 0, angleBetweenSpawns * i);
            Vector3 spawnPos = transform.position + spawnAngle * new Vector2(0, height * transform.localScale.y);

            Obstacle obs = Instantiate(obstaclePrefab, spawnPos, spawnAngle);

            int direction = Random.Range(0, 2) == 1 ? -1 : 1;
            obs.velocity = direction * CalculateOrbitalVelocity(spawnPos, obs.mass);
            obs.angularVelocity = direction * Random.Range(0f, 30f);
        }

        Instantiate(obstaclePrefab);
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
