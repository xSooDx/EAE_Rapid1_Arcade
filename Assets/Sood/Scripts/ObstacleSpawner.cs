using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public float spawnVelocity = 5f;
    public Rect boundingSpace;
    public Obstacle obstacle;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SpawnObstacle();
        }
    }

    void SpawnObstacle()
    {
        bool spawnOnRight = Random.Range(0, 2) == 1; // false => left
        float spawnX = spawnOnRight ? boundingSpace.max.x: boundingSpace.min.x;
        float spawnY = Random.Range(boundingSpace.min.y, boundingSpace.max.y);
        //Debug.Log($"{boundingSpace.min}, {boundingSpace.max}");
        Vector3 spawnPos = new Vector2(spawnX, spawnY) - boundingSpace.size/2;

        Obstacle obs = Instantiate(obstacle, transform.position + spawnPos, Quaternion.identity);
        obs.velocity = new Vector2(spawnOnRight ? -spawnVelocity : spawnVelocity, 0);
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + (Vector3)boundingSpace.position, boundingSpace.size);
    }
#endif
}
