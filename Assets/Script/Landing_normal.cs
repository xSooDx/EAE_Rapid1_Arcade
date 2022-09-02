using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landing_normal : LandingPointScript
{
    public override void TouchAction(Collision2D _col)
    {
        if (_col.gameObject.tag == "Player")
        {
            ShipController _ship = _col.gameObject.GetComponent<ShipController>();
            if (_ship != null)
            {
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

                if (!(_ship.GetRotateAngle() >= this.Req_RotateAngle - this.Req_RotateAngleTor && _ship.GetRotateAngle() <= this.Req_RotateAngle + this.Req_RotateAngleTor))
                {
                    Debug.Log("Rotate Crash!!");
                    CrashFunction("Landing angle incorrect!");
                    return;
                }

                LandingFunction("Perfect Landing");
            }
        }
    }

    void CrashFunction(string _desc)
    {
        if (MainGameController.gameController != null) MainGameController.gameController.GameOver.Invoke("Ship Crashed!!", _desc, false);
    }

    void LandingFunction(string _desc)
    {
        if (MainGameController.gameController != null) MainGameController.gameController.GameOver.Invoke("Success!!", _desc, true);
    }
}
