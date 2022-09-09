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

    public float angularVelocity
    {
        get { return myRigidbody.angularVelocity; }
        set { myRigidbody.angularVelocity = value; }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Destroy(this.gameObject);
        Continue_ResetPos _action = new Continue_ResetPos();
        if (GameEventManager.gameEvent != null) GameEventManager.gameEvent.GameOver.Invoke("Ship Crashed!!", description, _action);
    }
}
