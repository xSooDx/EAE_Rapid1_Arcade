using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Script for normal landing point
/// </summary>
public class Landing_normal : LandingPointScript
{
    public override void TouchAction(Collision2D _col)
    {
        if (_col.gameObject.tag == "Player")
        {
            ShipController _ship = _col.gameObject.GetComponent<ShipController>();
            if (_ship != null)
            {
                this.Direction = _col.transform.position - transform.position;
                if (_ship.GetVerticalSpd() > this.Req_MAXVerticalSpeed)
                {
                    Debug.Log("Vertical Crash!!");
                    CrashFunction("Too fast on vertical speed!");
                    return;
                }

                if (Mathf.Abs(_ship.GetHorizontalSpd()) > this.Req_MAXHorizonSpeed)
                {
                    Debug.Log("Horizon Crash!!");
                    CrashFunction("Too fast on horizontal speed!");
                    return;
                }

                if (!RotationChk(_col.transform.position - transform.position, _ship.GetRotateAngle()))
                {
                    Debug.Log("Rotate Crash!!");
                    CrashFunction("Landing angle incorrect!");
                    return;
                }

                //LandingFunction("Perfect Landing");
            }
        }
    }

    void CrashFunction(string _desc)
    {
        if (GameEventManager.gameEvent != null)
        {
            GameEventManager.gameEvent.GameOver.Invoke("Ship Crashed!!", _desc, GameEndActionsLib.continue_ResetPos);
            GameEventManager.gameEvent.PlayerCrash.Invoke(this.Direction);
        }
    }



    void LandingFunction(string _desc)
    {
        if (GameEventManager.gameEvent != null)
        {
            GameEventManager.gameEvent.GameOver.Invoke("Success!!", _desc, GameEndActionsLib.continue_MaintainPos);
            GameEventManager.gameEvent.AddScore.Invoke("Perfect Landing!!",100);
        }
    }
}
