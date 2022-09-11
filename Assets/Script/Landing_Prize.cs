using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// script for prize
/// </summary>
public class Landing_Prize : LandingPointScript
{
    private Rigidbody2D _rg;

    private ShipController _shipCtrl;

    public string PrizeID;

    public int PrizeScore;

    private bool _IsGrabbing;

    public bool FollowTarget;

    public PresentAnimationControl presentAnimation;


    private void Awake()
    {
        this._rg = GetComponent<Rigidbody2D>();
        this._IsGrabbing = false;
    }

    private void Start()
    {
        if (GameEventManager.gameEvent != null)
        {
            GameEventManager.gameEvent.PrizeLand.AddListener(DropPrize);
            GameEventManager.gameEvent.PrizeCrash.AddListener(CrashPrize);
        }
        this.PrizeID = gameObject.name;
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
    private void FixedUpdate()
    {
        if (this.FollowTarget)
        {
            this.transform.localPosition = Vector2.zero;
            this.transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
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

    void LandingFunction(ShipController _ship)
    {
        if (_ship != null && !_ship.IsGrabbing)
        {
            this._shipCtrl = _ship;
            this._IsGrabbing = true;
            this.gameObject.layer = LayerMask.NameToLayer("Prize_Grab");
            this.transform.SetParent(this._shipCtrl.GetGrabPoint());
            this.transform.localPosition = Vector2.zero;
            _ship.IsGrabbing = true;
            _ship.GrabbingPrizeID = this.PrizeID;
            //this.transform.position = this._shipCtrl.GetGrabPoint().position;
            this.transform.DORotateQuaternion(Quaternion.Euler(0f, 0f, 0f), 1f);
            this.FollowTarget = true;
            AudioManager.instance.PlayAudio("pickup");
            //if (this._Fjoint != null && _ship.GetRigidBody() != null) this._Fjoint.connectedBody = _ship.GetRigidBody();
            //this._Fjoint.enabled = true;
            foreach (Transform _child in gameObject.transform.GetComponentInChildren<Transform>())
            {
                _child.gameObject.layer = LayerMask.NameToLayer("Prize_Grab");
            }
        }

    }

    void DropPrize(string _prizeid)
    {
        if (!PrizeID.Equals(_prizeid) || !_IsGrabbing) return;
        //this.transform.SetParent(null);
        this._shipCtrl.IsGrabbing = false;
        this._shipCtrl.GrabbingPrizeID = "";
        this._shipCtrl = null;
        this.gameObject.layer = 6;
        this._rg.velocity = Vector2.zero;
        this._rg.freezeRotation = true;
        this._rg.isKinematic = true;
        this.FollowTarget = false;
        AudioManager.instance.PlayAudio("dropoff");
        if (GameEventManager.gameEvent != null)
        {
            GameEventManager.gameEvent.AddScore.Invoke("GOOD JOB!!",this.PrizeScore);
            //GameEventManager.gameEvent.GameOver.Invoke("Success!!", "The prize is delivered", GameEndActionsLib.continue_MaintainPos);
            GameEventManager.gameEvent.PrizeLand.RemoveListener(DropPrize);
        }
        if (presentAnimation != null) presentAnimation.OpenPresent();
        Destroy(this.gameObject,2f);
        //this._IsGrabbing = false;
    }

    void CrashPrize(string _prizeid)
    {
        if (!PrizeID.Equals(_prizeid) || !_IsGrabbing) return;

        this.transform.SetParent(null);
        this._shipCtrl.IsGrabbing = false;
        this._shipCtrl.GrabbingPrizeID = "";
        this._shipCtrl = null;
        this.FollowTarget = false;
        //AudioManager.instance.PlayAudio("dropoff");
        if (GameEventManager.gameEvent != null)
        {
            GameEventManager.gameEvent.PrizeLand.RemoveListener(DropPrize);
        }
        this._rg.AddForce(new Vector2(Random.Range(-5,5),1)*1000f);
        this._rg.AddTorque(Random.Range(-1, 2) * 500f);
        Destroy(this.gameObject, 2f);
    }


    public ShipController GetShipController()
    {
        return _shipCtrl;
    }
}
