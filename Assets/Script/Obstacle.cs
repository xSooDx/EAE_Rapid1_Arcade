using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Obstacle : MonoBehaviour
{
    [SerializeField] Rigidbody2D myRigidbody;
    [SerializeField] SpriteRenderer mySprite;
    [SerializeField] ParticleSystem explosionParticles;
    [SerializeField] ParticleSystem trailParticles;
    public string description = "Knocked Out!";
    public float explosionRadius = 1.5f;
    public float explosionForce = 4f;

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

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);
        
        foreach (Collider2D col in hits)
        {
            if (!col || !col.attachedRigidbody) continue;
            Vector2 dir = col.attachedRigidbody.position - (Vector2)transform.position;
            dir = (dir.normalized + -myRigidbody.velocity.normalized * 0.25f).normalized;
            col.attachedRigidbody.AddForce(dir * explosionForce, ForceMode2D.Impulse);
        }
        // ToDo Explosion Prefab
        ParticleSystem explodeParticles = Instantiate(explosionParticles, transform.position, transform.rotation);
        ParticleSystem.MainModule main = explodeParticles.main;
        main.startColor = trailParticles.main.startColor;
        Destroy(this.gameObject);
    }
}
