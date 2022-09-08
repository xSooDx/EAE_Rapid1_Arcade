using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// script for place for putting prize
/// </summary>
public class Landing_Destination : LandingPointScript
{
    // Start is called before the first frame update
    public override void TouchAction(Collision2D _col)
    {
        if (_col.gameObject.tag == "Player" || _col.gameObject.tag == "Prize")
        {
            ShipController _ship = _col.gameObject.tag == "Player"? _col.gameObject.GetComponent<ShipController>(): _col.gameObject.GetComponent<Landing_Prize>().GetShipController();
            if (_ship != null)
            {
                Debug.Log("Touch");
                if (_ship.GetVerticalSpd() > this.Req_MAXVerticalSpeed)
                {
                    CrashFunction(_col.gameObject.tag == "Player" ? "Ship" : "Prize" + " Crashed!!", "Too fast on vertical speed!");
                    return;
                }

                if (Mathf.Abs(_ship.GetHorizontalSpd()) > this.Req_MAXHorizonSpeed)
                {
                    CrashFunction(_col.gameObject.tag == "Player" ? "Ship" : "Prize" + " Crashed!!", "Too fast on horizontal speed!");
                    return;
                }

                if (!(_ship.GetRotateAngle() >= this.Req_RotateAngle - this.Req_RotateAngleTor && _ship.GetRotateAngle() <= this.Req_RotateAngle + this.Req_RotateAngleTor))
                {
                    CrashFunction(_col.gameObject.tag == "Player" ? "Ship" : "Prize" + " Crashed!!", "Landing angle incorrect!");
                    return;
                }

                if (_col.gameObject.tag == "Prize" && _col.gameObject.GetComponent<Landing_Prize>()!=null)
                {
                    LandingFunction(_col.gameObject.GetComponent<Landing_Prize>());
                }
            }
        }

    }

    void CrashFunction(string _Title, string _desc)
    {
        Debug.Log(_Title+" "+ _desc);
        if (GameEventManager.gameEvent != null) GameEventManager.gameEvent.GameOver.Invoke(_Title, _desc, false);
    }

    void LandingFunction(Landing_Prize _Prize)
    {
        Debug.Log(_Prize.PrizeID+" Land");
        //if (GameEventManager.gameEvent != null) GameEventManager.gameEvent.GameOver.Invoke("Success!!", _desc, true);
        if (GameEventManager.gameEvent != null) GameEventManager.gameEvent.PrizeLand.Invoke(_Prize.PrizeID);
    }
}
