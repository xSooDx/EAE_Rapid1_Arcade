using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Obstacle : MonoBehaviour
{
    [SerializeField] Rigidbody2D myRigidbody;
    public string description = "Knocked Out!";

    public Vector2 velocity
    {
        get { return myRigidbody.velocity; }
        set { myRigidbody.velocity = value; }
    }
    public float mass
    {
        get { return myRigidbody.mass; }
        set { myRigidbody.mass = value; }
    }

    public float angularVelocity
    {
        get { return myRigidbody.angularVelocity; }
        set { myRigidbody.angularVelocity = value; }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            if (GameEventManager.gameEvent != null) GameEventManager.gameEvent.GameOver.Invoke("Ship Crashed!!", description, false, true);
        }
    }
}
