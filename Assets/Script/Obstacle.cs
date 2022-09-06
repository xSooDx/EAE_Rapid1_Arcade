using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Obstacle : MonoBehaviour
{
    [SerializeField] Rigidbody2D myRigidbody;

    public Vector2 velocity
    {
        get { return myRigidbody.velocity; }
        set { myRigidbody.velocity = value; }
    }

    public float angularVelocity
    {
        get { return myRigidbody.angularVelocity; }
        set { myRigidbody.angularVelocity = value; }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Destroy(this.gameObject);
    }
}
