using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Landing_Prize : LandingPointScript
{
    private Rigidbody2D _rg;
    private DistanceJoint2D _joint;

    private FixedJoint2D _Fjoint;

    public float Weight;
    private void Awake()
    {
        this._rg= GetComponent<Rigidbody2D>();
        this._joint= GetComponent<DistanceJoint2D>();
        this._Fjoint = GetComponent<FixedJoint2D>();
        if (this._joint != null) this._joint.enabled = false;
        if (this._Fjoint != null) this._Fjoint.enabled = false;
    }
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

                LandingFunction(_ship);
            }
        }
    }

    void CrashFunction(string _desc)
    {
        if (MainGameController.gameController != null) MainGameController.gameController.GameOver.Invoke("Ship Crashed!!", _desc, false);
    }

    void LandingFunction(ShipController _ship)
    {
        //V1
        //this.transform.position = _ship.GetCargoPos();
        //this.transform.SetParent(_ship.gameObject.transform);
        //if (this._rg != null) this._rg.isKinematic = true;
        //this.gameObject.layer = 7;

        if (this._Fjoint != null && _ship.GetRigidBody() != null) this._Fjoint.connectedBody = _ship.GetRigidBody();
        this._Fjoint.enabled = true;
        this.gameObject.layer = 7;

        //V2
        //if (this._joint != null && _ship.GetRigidBody() != null) this._joint.connectedBody = _ship.GetRigidBody();
        //this._joint.enabled = true;
        //this.gameObject.layer = 7;
    }
}
