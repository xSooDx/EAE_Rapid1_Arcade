using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// script for prize
/// </summary>
public class Landing_Prize : LandingPointScript
{
    private Rigidbody2D _rg;

    private FixedJoint2D _Fjoint;

    private ShipController _shipCtrl;

    public string PrizeID;

    public int PrizeScore;

    private bool _IsGrabbing;

    private void Awake()
    {
        this._rg = GetComponent<Rigidbody2D>();
        this._Fjoint = GetComponent<FixedJoint2D>();
        if (this._Fjoint != null) this._Fjoint.enabled = false;
        this._IsGrabbing = false;
    }

    private void Start()
    {
        if (GameEventManager.gameEvent != null) GameEventManager.gameEvent.PrizeLand.AddListener(DropPrize);
    }

    public override void TouchAction(Collision2D _col)
    {
        if (_col.gameObject.tag == "Player" && !this._IsGrabbing)
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

                LandingFunction(_ship);
            }
        }
    }

    void CrashFunction(string _desc)
    {
        if (GameEventManager.gameEvent != null)
        {
            GameEventManager.gameEvent.GameOver.Invoke("Ship Crashed!!", _desc, false);
            GameEventManager.gameEvent.PlayerCrash.Invoke(this.Direction);
        }
    }

    void LandingFunction(ShipController _ship)
    {
        if (this._Fjoint != null && _ship.GetRigidBody() != null) this._Fjoint.connectedBody = _ship.GetRigidBody();
        this._Fjoint.enabled = true;
        this._Fjoint.connectedAnchor = new Vector2(0, 0.1f);
        this._shipCtrl = _ship;
        this._IsGrabbing = true;
        this.gameObject.layer = LayerMask.NameToLayer("Prize_Grab");
        foreach (Transform _child in gameObject.transform.GetComponentInChildren<Transform>())
        {
            _child.gameObject.layer= LayerMask.NameToLayer("Prize_Grab");
        }
    }

    void DropPrize(string _prizeid)
    {
        if (!PrizeID.Equals(_prizeid) || !_IsGrabbing) return;

        this._Fjoint.connectedBody = null;
        this._Fjoint.enabled = false;
        this._shipCtrl = null;
        this.gameObject.layer = 6;

        if (GameEventManager.gameEvent != null)
        {
            GameEventManager.gameEvent.AddScore.Invoke(this.PrizeScore);
            GameEventManager.gameEvent.GameOver.Invoke("Success!!", "The prize is delivered", true);
            GameEventManager.gameEvent.PrizeLand.RemoveListener(DropPrize);
        }
        Destroy(this.gameObject);
        //this._IsGrabbing = false;
    }

    public ShipController GetShipController()
    {
        return _shipCtrl;
    }
}
