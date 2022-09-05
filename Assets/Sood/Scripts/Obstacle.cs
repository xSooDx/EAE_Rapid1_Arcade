using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public Vector2 velocity;

    // Update is called once per frame
    void Update()
    {
        transform.position += (Vector3) velocity * Time.deltaTime;
    }
}
