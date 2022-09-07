using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ClawState
{
    public Sprite Emoji;
    public float ClawAngle;
}

[System.Serializable]
public class TransformState
{
    public GameObject part;
    [HideInInspector]
    public Vector3 originalPosition;
    [HideInInspector]
    public Quaternion originalRotation;

    public Collider2D itemCollider;
}

public class ClawAnimationControl : MonoBehaviour
{
    public ClawState NormalState;

    public ClawState GrabState;

    public ClawState CrashState;

    public List<TransformState> partState;

    private void Awake()
    {
        foreach (var item in partState)
        {
            if (item.part != null)
            {
                item.originalPosition = item.part.transform.position;
                item.originalRotation = item.part.transform.rotation;
            }
        }
    }

    public void Resetitem()
    {
        foreach (var item in partState)
        {
            if (item.part != null)
            {
                if (item.part.GetComponent<Rigidbody2D>() != null)
                {
                    Destroy(item.part.GetComponent<Rigidbody2D>());
                }
                item.part.transform.position = item.originalPosition;
                item.part.transform.rotation = item.originalRotation;
            }
        }
    }

    public void Explosion()
    {
#if UNITY_EDITOR 
        if (Application.isPlaying)
#endif
            foreach (var item in partState)
            {
                if (item.part != null)
                {
                    Rigidbody2D _rg = item.part.GetComponent<Rigidbody2D>() == null ? item.part.AddComponent<Rigidbody2D>() : item.part.GetComponent<Rigidbody2D>();
                    item.itemCollider.enabled = true;
                    //_rg.gravityScale = 0;
                    _rg.drag = 0.5f;
                    _rg.AddForce(new Vector2(Random.Range(-10, 10) * 70f, Random.Range(1, 10) * 70f));
                    _rg.AddTorque(Random.Range(-10, 10));
                }
            }
    }
}
