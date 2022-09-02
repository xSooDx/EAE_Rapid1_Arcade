using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    /// <summary>
    /// Is camera focusing on player
    /// </summary>
    private bool IsFocus;

    [Tooltip("Camera will follow it's target when x out of this value")]
    public float FollowBuffer;

    /// <summary>
    /// The focus target
    /// </summary>
    public Transform Target;

    /// <summary>
    /// camera on this gameobject
    /// </summary>
    private Camera m_Camera;

    [Tooltip("The size when zoom in")]
    public float FocusSize;

    [Tooltip("Camera move speed")]
    public float MoveSpeed;

    [Tooltip("Camera zoom speed")]
    public float ZoomSpeed;

    [Tooltip("The offset position for following target")]
    public Vector3 offset;

    [Tooltip("The size when zoom in")]
    public float CooldownTime;

    /// <summary>
    /// the zoom in buffer
    /// </summary>
    private float CooldownCnt;

    /// <summary>
    /// The original size of camera
    /// </summary>
    private float OriginSize;

    /// <summary>
    /// The original position of camera
    /// </summary>
    private Vector3 OriginPos;

    private Vector3 MovePos;

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
        if (MainGameController.gameController != null) MainGameController.gameController.StartFocus.AddListener(FocusObject);
        if (MainGameController.gameController != null) MainGameController.gameController.CancelFocus.AddListener(CancelFocus);
    }

    private void FixedUpdate()
    {
        if (IsFocus && Target != null)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, Target.position + offset, MoveSpeed);//move
            this.m_Camera.orthographicSize = Mathf.Lerp(this.m_Camera.orthographicSize, FocusSize, ZoomSpeed * Time.deltaTime);
        }
        if (Mathf.Abs(Target.position.x - this.MovePos.x) > this.FollowBuffer)
        {
            Vector3 des = new Vector3(Target.position.x, this.MovePos.y, this.MovePos.z);
            this.MovePos = Vector3.Lerp(this.MovePos, des, MoveSpeed * Time.deltaTime * Mathf.Abs(Target.position.x - this.MovePos.x) * 1.5f);
            if (!IsFocus)
            {
                this.transform.position = this.MovePos;
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
                this.transform.position = Target.position + offset;
                CooldownCnt = 0;
            }
        }
        else
        {
            CooldownCnt = 0;
        }
    }

    /// <summary>
    /// Cancel focusing and return to camera's original codition
    /// </summary>
    public void CancelFocus(bool _Reset)
    {
        if (_Reset)
        {
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
        this.gameObject.transform.position = _Reset ? this.OriginPos : this.MovePos;
        if (_Reset)
        {
            this.MovePos = this.OriginPos;
        }
        this.m_Camera.orthographicSize = OriginSize;
        CooldownCnt = 0;
    }
}
