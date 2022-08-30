using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LandingPointScript : MonoBehaviour
{
    [Tooltip("Vertical speed exceed this value, crash")]
    public float Req_MAXVerticalSpeed;

    [Tooltip("Horizontal speed exceed this value, crash")]
    public float Req_MAXHorizonSpeed;

    [Tooltip("Rotation requirement")]
    public float Req_RotateAngle;
    [Tooltip("Rotation requirement tolerance")]
    public float Req_RotateAngleTor;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            ShipController _ship = collision.gameObject.GetComponent<ShipController>();
            if (_ship != null)
            {
                if (_ship.GetVerticalSpd() > this.Req_MAXVerticalSpeed)
                {
                    Debug.Log("Vertical Crash!!");
                }

                if (Mathf.Abs(_ship.GetHorizontalSpd()) > this.Req_MAXHorizonSpeed)
                {
                    Debug.Log("Horizon Crash!!");
                }

                if (Mathf.Abs(_ship.GetRotateAngle()) > this.Req_RotateAngle+this.Req_RotateAngleTor)
                {
                    Debug.Log("Rotate Crash!!");
                }
            }
        }
    }
}
