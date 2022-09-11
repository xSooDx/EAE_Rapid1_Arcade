using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// script for place for putting prize
/// </summary>
public class Landing_Destination : LandingPointScript
{
    [SerializeField]
    private float Angle;
    // Start is called before the first frame update
    public override void TouchAction(Collision2D _col)
    {
        if (_col.gameObject.tag == "Prize")
        {
            Landing_Prize _prize = _col.gameObject.GetComponent<Landing_Prize>();
            ShipController _ship = _prize.GetShipController();
            if (_ship != null && _prize!= null)
            {
                Debug.Log("Touch");
                if (_ship.GetVerticalSpd() > this.Req_MAXVerticalSpeed)
                {
                    CrashFunction("Prize Crashed!!", "Too fast on vertical speed!", _prize.PrizeID);
                    return;
                }

                if (Mathf.Abs(_ship.GetHorizontalSpd()) > this.Req_MAXHorizonSpeed)
                {
                    CrashFunction("Prize Crashed!!", "Too fast on horizontal speed!", _prize.PrizeID);
                    return;
                }

                if (!RotationChk(_ship.GetRotateAngle()))
                {
                    CrashFunction("Prize Crashed!!", "Landing angle incorrect!", _prize.PrizeID);
                    return;
                }

                if (_col.gameObject.tag == "Prize" && _col.gameObject.GetComponent<Landing_Prize>() != null)
                {
                    LandingFunction(_prize);
                }
            }
        }

    }

    private void Update()
    {
        Angle = this.transform.rotation.eulerAngles.z;
    }

    void CrashFunction(string _Title, string _desc,string _PrizeID)
    {
        Debug.Log(_Title + " " + _desc);
        if (GameEventManager.gameEvent != null)
        {
            //GameEventManager.gameEvent.GameOver.Invoke(_Title, _desc, GameEndActionsLib.continue_ResetPos);
            GameEventManager.gameEvent.PrizeCrash.Invoke(_PrizeID);
        }
    }

    void LandingFunction(Landing_Prize _Prize)
    {
        _Prize.transform.SetParent(this.gameObject.transform);
        Debug.Log(_Prize.PrizeID + " Land");
        if (GameEventManager.gameEvent != null) GameEventManager.gameEvent.PrizeLand.Invoke(_Prize.PrizeID);
    }
}
