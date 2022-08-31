using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    /// <summary>
    /// Is camera focusing on player
    /// </summary>
    private bool IsFocus;

    /// <summary>
    /// The focus target
    /// </summary>
    private Transform Target;

    /// <summary>
    /// camera on this gameobject
    /// </summary>
    private Camera m_Camera;

    [Tooltip("The size when zoom in")]
    public float FocusSize;

    [Tooltip("Camera move speed")]
    public float MoveSpeed;

    [Tooltip("The offset position for following target")]
    public Vector3 offset;

    /// <summary>
    /// The original size of camera
    /// </summary>
    private float OriginSize;

    /// <summary>
    /// The original position of camera
    /// </summary>
    private Vector3 OriginPos;

  

    private void Start()
    {
        this.m_Camera = gameObject.GetComponent<Camera>();
        if (this.m_Camera != null)
        {
            this.OriginSize = this.m_Camera.orthographicSize;//set the original size
        }
        this.OriginPos = gameObject.transform.position;//set the original position
        // add event that can be trigger when you need focus on something
        if (MainGameController.gameController != null) MainGameController.gameController.StartFocus.AddListener(FocusObject);
        if (MainGameController.gameController != null) MainGameController.gameController.CancelFocus.AddListener(CancelFocus);
    }

    private void FixedUpdate()
    {
        if (IsFocus && Target != null)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, Target.position + offset, MoveSpeed);//move
        }
    }

    /// <summary>
    /// Set the focus target and start focusing
    /// </summary>
    /// <param name="_target"></param>
    void FocusObject(Transform _target)
    {
        if (!IsFocus && _target!=null)
        {
            this.Target = _target;
            this.IsFocus = true;
            this.m_Camera.orthographicSize = FocusSize;
            this.transform.position = Target.position + offset;
        }
    }

    /// <summary>
    /// Cancel focusing and return to camera's original codition
    /// </summary>
    void CancelFocus()
    {
        if (IsFocus)
        {
            this.Target = null;
            this.IsFocus = false;
            this.gameObject.transform.position = this.OriginPos;
            this.m_Camera.orthographicSize = OriginSize;
        }
    }
}
