using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[System.Serializable]
public class PointerObj
{
    public GameObject TargetObj;

    public RectTransform Pointer_UI;

    public bool Show;

}
public class LocationPointer : MonoBehaviour
{
    public static LocationPointer navSystem;

    public Camera m_Camera;

    public Canvas MainCanvas;

    public List<PointerObj> Locations;

    private void Awake()
    {
        navSystem = this;
    }

    private void FixedUpdate()
    {

        RectTransform CanvasRect = MainCanvas.GetComponent<RectTransform>();

        foreach (var item in Locations)
        {
            if (!item.Show)
            {
                item.Pointer_UI.gameObject.SetActive(false);
                continue;
            }
            else
            {
                item.Pointer_UI.gameObject.SetActive(true);
            }
            Vector2 ViewportPosition = m_Camera.WorldToViewportPoint(item.TargetObj.transform.position);
            float Xpos = Mathf.Clamp((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f), CanvasRect.rect.xMin, CanvasRect.rect.xMax);
            float YPos = Mathf.Clamp((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f), CanvasRect.rect.yMin, CanvasRect.rect.yMax);
            Vector2 WorldObject_ScreenPosition = new Vector2(Xpos, YPos);

            Vector2 newDir = ViewportPosition - item.Pointer_UI.anchoredPosition;
            float angle = Mathf.Atan2(newDir.y, newDir.x) * Mathf.Rad2Deg + 90;
            item.Pointer_UI.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
            item.Pointer_UI.anchoredPosition = WorldObject_ScreenPosition;
        }

    }

    public void UpdatePointer(Collider2D[] _targets)
    {
        foreach (var item in Locations)
        {
            if (item.TargetObj != null)
            {
                if (_targets.Any(x => x.gameObject == item.TargetObj))
                {
                    item.Show = false;
                }
                else
                {
                    item.Show = true;
                }
            }
        }
    }

}
