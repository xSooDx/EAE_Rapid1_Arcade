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


    [Tooltip("Camera move speed")]
    public float MoveSpeed;



    /// <summary>
    /// camera on this gameobject
    /// </summary>
    private Camera m_Camera;

    public Canvas MainCanvas;

    public RectTransform UI_Element;

    [SerializeField]
    private Vector2 Pos;

    private void Awake()
    {
        this.m_Camera = gameObject.GetComponent<Camera>();
    }

    private void FixedUpdate()
    {
        //this.m_Camera.wi

        RectTransform CanvasRect = MainCanvas.GetComponent<RectTransform>();

        Vector2 ViewportPosition = m_Camera.WorldToViewportPoint(Target.position);
        Pos = ViewportPosition;
        float Xpos = Mathf.Clamp((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f), CanvasRect.rect.xMin, CanvasRect.rect.xMax);
        float YPos = Mathf.Clamp((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f), CanvasRect.rect.yMin, CanvasRect.rect.yMax);
        Vector2 WorldObject_ScreenPosition = new Vector2(Xpos, YPos);

        //now you can set the position of the ui element
        UI_Element.anchoredPosition = WorldObject_ScreenPosition;
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
