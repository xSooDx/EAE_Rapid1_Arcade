using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Test : MonoBehaviour
{
    /// <summary>
    /// The focus target
    /// </summary>
    public Transform Target;

    public Transform Planet;

    [Range(0,100)]
    public float FocusRatio;

    [Tooltip("Camera move speed")]
    public float MoveSpeed;

    [Tooltip("The offset position for following target")]
    public Vector3 offset;

    public float FocusBuffer;


    /// <summary>
    /// camera on this gameobject
    /// </summary>
    private Camera m_Camera;

    [Tooltip("Camera zoom speed")]
    public float ZoomSpeed;

    private void Awake()
    {
        this.m_Camera = gameObject.GetComponent<Camera>();
    }

    private void FixedUpdate()
    {
        Vector3 _Fpos = this.Planet.position + (Target.position - this.Planet.position) / 100f * FocusRatio;
        _Fpos.z = offset.z;
        float _dis = Vector2.Distance(_Fpos, this.Planet.position)+ FocusBuffer;
        this.m_Camera.orthographicSize = Mathf.Lerp(this.m_Camera.orthographicSize, _dis/2, ZoomSpeed * Time.deltaTime * 2);
        this.transform.position = Vector3.Lerp(this.transform.position, _Fpos, MoveSpeed);
        RotateCamera(this.Planet.position);
    }

    void RotateCamera(Vector2 _target)
    {
        Vector2 _dir = (Vector2)this.transform.position - _target;
        int sign = (_dir.x >= 0) ? -1 : 1;
        int offset = (sign >= 0) ? 0 : 360;
        float angle = Vector2.Angle(_dir.normalized, Vector2.up) * sign + offset;
        gameObject.transform.DORotateQuaternion(Quaternion.Euler(0f, 0f, angle), 0.01f);
    }
}
