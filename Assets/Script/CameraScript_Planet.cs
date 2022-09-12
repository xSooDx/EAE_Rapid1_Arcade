using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript_Planet : MonoBehaviour
{
    /// <summary>
    /// Is camera focusing on player
    /// </summary>
    private bool IsFocus;

    private bool ClosePlanet;

    private bool FocusOnPlayer;

    /// <summary>
    /// The focus target
    /// </summary>
    public Transform Target;

    public Transform Planet;

    /// <summary>
    /// camera on this gameobject
    /// </summary>
    private Camera m_Camera;

    [Tooltip("The size when zoom in")]
    public float ZoomoutSize;

    [Tooltip("Camera move speed")]
    public float MoveSpeed;

    [Tooltip("Camera zoom speed")]
    public float ZoomSpeed;

    [Tooltip("The offset position for following target")]
    public Vector3 offset;

    [Tooltip("The time can zoom")]
    public float CooldownTime;

    [Tooltip("The time can zoom on planet")]
    public float PlanetCooldownTime;

    /// <summary>
    /// the zoom in buffer
    /// </summary>
    private float CooldownCnt;

    private float PlanetCoolDownCnt;

    /// <summary>
    /// The original size of camera
    /// </summary>
    private float OriginSize;

    /// <summary>
    /// The original position of camera
    /// </summary>
    private Vector3 OriginPos;

    private Vector3 MovePos;

    [Range(0, 100)]
    public float FocusRatio;

    [Range(0, 100)]
    public float UnFocusRatio;

    [Range(0, 100)]
    public float minZoom;

    [Range(0, 500)]
    public float MaxZoom;

    [Range(0, 100)]
    public float ZoomLimit;

    private void Awake()
    {
        this.m_Camera = gameObject.GetComponent<Camera>();
    }

    private void Start()
    {
        if (this.m_Camera != null)
        {
            this.OriginSize = this.m_Camera.orthographicSize;//set the original size
        }
        this.OriginPos = this.MovePos = gameObject.transform.position;

        // add event that can be trigger when you need focus on something
        if (GameEventManager.gameEvent != null)
        {
            GameEventManager.gameEvent.StartFocus.AddListener(FocusObject);
            GameEventManager.gameEvent.CancelFocus.AddListener(CancelFocus);
            GameEventManager.gameEvent.ClosePlanet.AddListener(InPlanet);
            GameEventManager.gameEvent.LeavePlanet.AddListener(OutPlanet);
            GameEventManager.gameEvent.PlayerCrash.AddListener(Crash);
            GameEventManager.gameEvent.FocusPlayer.AddListener(FocusPlayerSet);
        }

    }

    private void FixedUpdate()
    {
        if (Target != null)
        {
            if (ClosePlanet && !FocusOnPlayer && this.Planet != null)
            {
                Vector3 diff = (Target.position - this.Planet.position);
                Vector3 _Fpos = this.Planet.position + diff / 100f * (IsFocus ? FocusRatio : UnFocusRatio);
                _Fpos.z = offset.z;
                _Fpos += diff / 8f;
                float _dis = Vector2.Distance(_Fpos, Target.position);
                float newZoom = Mathf.Lerp(minZoom, MaxZoom, _dis / ZoomLimit);
                this.m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, newZoom, Time.deltaTime * ZoomSpeed);
                this.transform.position = Vector3.Lerp(this.transform.position, _Fpos, MoveSpeed);

            }
            else
            {
                this.transform.position = Vector3.Lerp(this.transform.position, Target.position + offset, MoveSpeed);
                this.m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, FocusOnPlayer ? 50 : ZoomoutSize, Time.deltaTime * ZoomSpeed);
            }
            if (ClosePlanet && this.Planet != null)
            {
                RotateCamera(this.Planet.position);
            }

        }

    }

    /// <summary>
    /// Set the focus target and start focusing
    /// </summary>
    /// <param name="_target"></param>
    public void FocusObject()
    {
        if (!IsFocus)//if is not focusing
        {
            CooldownCnt += Time.deltaTime;
            if (CooldownCnt >= CooldownTime / 1.5f)
            {
                this.MovePos = gameObject.transform.position;//set the original position
                this.IsFocus = true;
                CooldownCnt = 0;
            }
        }
        else
        {
            CooldownCnt = 0;
        }
    }

    void Crash(Vector2 _dir)
    {
        FocusPlayerSet(true);
    }

    public void FocusPlayerSet(bool _set)
    {
        FocusOnPlayer = _set;
    }
    public void InPlanet(Transform _planet)
    {
        if (_planet != null)
        {
            if (!ClosePlanet)
            {
                PlanetCoolDownCnt += Time.deltaTime;
                if (PlanetCoolDownCnt >= PlanetCooldownTime / 1.5)
                {
                    this.Planet = _planet;
                    ClosePlanet = true;
                    PlanetCoolDownCnt = 0;
                }
            }
            else
            {
                PlanetCoolDownCnt = 0;
            }
        }
    }

    public void OutPlanet()
    {
        if (ClosePlanet)
        {
            PlanetCoolDownCnt += Time.deltaTime;
            if (PlanetCoolDownCnt >= PlanetCooldownTime)
            {
                this.Planet = null;
                ClosePlanet = false;
            }
        }
        else
        {
            PlanetCoolDownCnt = 0;
        }
    }

    /// <summary>
    /// Cancel focusing and return to camera's original codition
    /// </summary>
    public void CancelFocus(bool _Reset)
    {
        if (_Reset)
        {
            FocusOnPlayer = false;
            ResetCamera(_Reset);
            return;
        }
        if (IsFocus)//if is focusing
        {
            CooldownCnt += Time.deltaTime;
            if (CooldownCnt >= CooldownTime)
            {
                ResetCamera(_Reset);
            }
        }
        else
        {
            CooldownCnt = 0;
        }
    }

    void ResetCamera(bool _Reset)
    {
        this.IsFocus = false;
        if (_Reset)
        {
            this.MovePos = this.OriginPos;
        }
        //this.m_Camera.orthographicSize = OriginSize;
        CooldownCnt = 0;
    }

    void RotateCamera(Vector2 _target)
    {
        Vector2 _dir = (Vector2)this.transform.position - _target;
        int sign = (_dir.x >= 0) ? -1 : 1;
        int offset = (sign >= 0) ? 0 : 360;
        float angle = Vector2.Angle(_dir.normalized, Vector2.up) * sign + offset;
        gameObject.transform.DORotateQuaternion(Quaternion.Euler(0f, 0f, angle), 2f);
    }
}
