using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LandingPointScript : MonoBehaviour
{
    [Tooltip("Vertical speed exceed this value, crash")]
    public float Req_MAXVerticalSpeed;

    [Tooltip("Horizontal speed exceed this value, crash")]
    public float Req_MAXHorizonSpeed;

    /// <summary>
    /// this is new
    /// </summary>
    [Tooltip("Rotation requirement")]
    public float Req_RotateAngle;
    [Tooltip("Rotation requirement tolerance")]
    public float Req_RotateAngleTor;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        TouchAction(collision);
    }

    public virtual void TouchAction(Collision2D _col)
    {
        Debug.Log("Touch "+ _col.gameObject.name);
    }

    
}
