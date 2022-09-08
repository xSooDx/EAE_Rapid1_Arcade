using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle_State : PrizeState
{
    public Idle_State(Landing_Prize ObjectData) : base(ObjectData)
    {

    }

    //public override void TouchFunction(Collision2D _col)
    //{
    //    if (_col.gameObject.tag == "Player")
    //    {
    //        ShipController _ship = _col.gameObject.GetComponent<ShipController>();
    //        if (_ship != null)
    //        {
    //            if (_ship.GetVerticalSpd() > ObjectData.Req_MAXVerticalSpeed)
    //            {
    //                Debug.Log("Vertical Crash!!");
    //                CrashFunction("Too fast on vertical speed!");
    //                return;
    //            }

    //            if (Mathf.Abs(_ship.GetHorizontalSpd()) > ObjectData.Req_MAXHorizonSpeed)
    //            {
    //                Debug.Log("Horizon Crash!!");
    //                CrashFunction("Too fast on horizontal speed!");
    //                return;
    //            }

    //            if (!(_ship.GetRotateAngle() >= ObjectData.Req_RotateAngle - ObjectData.Req_RotateAngleTor && _ship.GetRotateAngle() <= ObjectData.Req_RotateAngle + ObjectData.Req_RotateAngleTor))
    //            {
    //                Debug.Log("Rotate Crash!!");
    //                CrashFunction("Landing angle incorrect!");
    //                return;
    //            }

    //            LandingFunction(_ship);
    //        }
    //    }
    //}
    //void CrashFunction(string _desc)
    //{
    //    if (MainGameController.gameController != null) MainGameController.gameController.GameOver.Invoke("Ship Crashed!!", _desc, false);
    //}

    //void LandingFunction(ShipController _ship)
    //{
    //    if (ObjectData._Fjoint != null && _ship.GetRigidBody() != null) ObjectData._Fjoint.connectedBody = _ship.GetRigidBody();
    //    ObjectData._Fjoint.enabled = true;
    //    ObjectData._shipCtrl = _ship;
    //}
}
