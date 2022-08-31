using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    private bool IsFocus;

    private Transform Target;

    private Camera m_Camera;

    public float FocusSize;

    public float MoveSpeed;

    public Vector3 offset;

    private float OriginSize;

    private Vector3 OriginPos;

  

    private void Start()
    {
        this.m_Camera = gameObject.GetComponent<Camera>();
        if (this.m_Camera != null)
        {
            this.OriginSize = this.m_Camera.orthographicSize;
        }
        this.OriginPos = gameObject.transform.position;
        if (MainGameController.gameController != null) MainGameController.gameController.StartFocus.AddListener(FocusObject);
        if (MainGameController.gameController != null) MainGameController.gameController.CancelFocus.AddListener(CancelFocus);
    }

    private void FixedUpdate()
    {
        if (IsFocus && Target != null)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, Target.position + offset, MoveSpeed);
        }
    }

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
