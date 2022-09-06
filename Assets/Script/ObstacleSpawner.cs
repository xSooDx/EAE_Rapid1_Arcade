using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public float spawnVelocity = 1f;
    public Rect boundingSpace;
    public Obstacle obstacle;
    public int maxObstacleCount = 5;
    public float maxTimeBetwenSpawn = 8f;
    public float minTimeBetwenSpawn = 3f;

    Coroutine obstacleSpawnerCoroutine = null;

    List<Obstacle> obstacleList;

    private void Start()
    {
        obstacleList = new List<Obstacle>();
    }

    void Update()
    {
        if (obstacleSpawnerCoroutine == null)
        {
            float spawnTime = obstacleList.Count == 0? minTimeBetwenSpawn : Random.Range(minTimeBetwenSpawn, maxTimeBetwenSpawn);
            obstacleSpawnerCoroutine = StartCoroutine(ObstacleSpawnCountdownRoutine(spawnTime));
        }
    }

    void SpawnObstacle()
    {
        bool spawnOnRight = obstacleList.Count % 2 == 0; // 1 => left
        float spawnX = spawnOnRight ? boundingSpace.max.x: boundingSpace.min.x;
        float spawnY = Random.Range(boundingSpace.min.y, boundingSpace.max.y);

        Vector3 spawnPos = new Vector2(spawnX, spawnY);

        Obstacle obs = Instantiate(obstacle, transform.position + spawnPos, Quaternion.identity);
        obs.velocity = new Vector2(spawnOnRight ? -spawnVelocity : spawnVelocity, 0);
        obs.angularVelocity = spawnOnRight ? - 60f : 60f;
    }

    IEnumerator ObstacleSpawnCountdownRoutine(float timeToSpawn)
    {
        yield return new WaitForSeconds(timeToSpawn);
   
        SpawnObstacle();
        obstacleSpawnerCoroutine = null;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        obstacleSpawnerCoroutine = null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + (Vector3)boundingSpace.center, boundingSpace.size);
    }
#endif
}
