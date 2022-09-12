using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[System.Serializable]
public class TargetPointer
{
    public GameObject TargetObj;

    public RectTransform Pointer_UI;

    private Image ShowImg;

    public Sprite In_Img;

    public Sprite Out_Img;

    public bool Show;

}
public class UIController : MonoBehaviour
{
    public static UIController uiController;

    public Camera m_Camera;

    public Canvas MainCanvas;

    private RectTransform CanvasRect;

    public GameObject AddScoreUI;

    public GameObject WarningSign;

    public GameObject PointerObj;

    [SerializeField]
    private bool ShowPointer;

    [SerializeField]
    private List<TargetPointer> targetPointers;

    private void Awake()
    {
        uiController = this;
        CanvasRect = MainCanvas.GetComponent<RectTransform>();
    }
    private void Start()
    {
        if (GameEventManager.gameEvent != null)
        {
            GameEventManager.gameEvent.SetWarning.AddListener(SetWarning);
        }
        WarningSign.SetActive(false);

       
    }

    private void FixedUpdate()
    {
        if (ShowPointer)
        {
            SetPointer();
        } else if (targetPointers.Count>0)
        {
            foreach (var item in targetPointers)
            {
                item.Pointer_UI.gameObject.SetActive(false);
            }
        }
    }

    private void SetPointer()
    {
        if (targetPointers.Count <= 0)
        {
            GetPlatform();
        }
        foreach (var item in targetPointers)
        {

            Vector2 ViewportPosition = m_Camera.WorldToViewportPoint(item.TargetObj.transform.position);
            if (CheckIsOut(ViewportPosition))
            {
                float Xpos = Mathf.Clamp((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f), CanvasRect.rect.xMin, CanvasRect.rect.xMax);
                float YPos = Mathf.Clamp((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f), CanvasRect.rect.yMin, CanvasRect.rect.yMax);
                Vector2 WorldObject_ScreenPosition = new Vector2(Xpos, YPos);

                Vector2 newDir = ViewportPosition - item.Pointer_UI.anchoredPosition;
                float angle = Mathf.Atan2(newDir.y, newDir.x) * Mathf.Rad2Deg + 90;
                item.Pointer_UI.gameObject.SetActive(true);
                item.Pointer_UI.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                item.Pointer_UI.anchoredPosition = WorldObject_ScreenPosition;
            }
            else
            {
                item.Pointer_UI.gameObject.SetActive(false);
            }
            
        }
    }

    void GetPlatform()
    {
        GameObject[] _Platform = GameObject.FindGameObjectsWithTag("Platform");
        if (_Platform.Length > 0)
        {
            foreach (var item in _Platform)
            {
                GameObject _obj = Instantiate(PointerObj);
                _obj.transform.SetParent(MainCanvas.transform);
                _obj.SetActive(false);
                TargetPointer newPointer = new TargetPointer();
                newPointer.TargetObj = item.gameObject;
                if (_obj.GetComponent<RectTransform>() != null)
                {
                    newPointer.Pointer_UI = _obj.GetComponent<RectTransform>();
                }
                targetPointers.Add(newPointer);
            }

        }
    }

    public void GetScore(string _desc, int _score, Vector2 _pos)
    {
        if (CanvasRect != null)
        {
            Vector2 ViewportPosition = m_Camera.WorldToViewportPoint(_pos);

            Vector2 WorldObject_ScreenPosition = WorldtoScreenPos(ViewportPosition);

            GameObject _obj = Instantiate(AddScoreUI);
            _obj.transform.SetParent(MainCanvas.transform);
            TextMeshProUGUI _txt = _obj.GetComponentInChildren<TextMeshProUGUI>();
            Debug.Log(_txt == null);
            RectTransform rect = _obj.GetComponent<RectTransform>();
            if (rect != null && _txt != null)
            {
                rect.localScale = Vector2.one;
                rect.anchoredPosition = WorldObject_ScreenPosition;
                _txt.text = _desc + "\n+" + _score.ToString();
            }
            Destroy(_obj, 2f);
        }

    }

    private Vector2 WorldtoScreenPos(Vector2 _VP)
    {
        float Xpos = (_VP.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f);
        float YPos = (_VP.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f);
        return new Vector2(Xpos, YPos);
    }

    void SetWarning(string _tag, bool _Warning)
    {
        if (WarningSign != null)
        {
            WarningSign.SetActive(_Warning);
        }
    }

    public void SetShowPointer(bool _setup)
    {
        this.ShowPointer = _setup;
    }

    private bool CheckIsOut(Vector2 _pos)
    {
        float Xpos = (_pos.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f);
        float YPos = (_pos.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f);
        return (Xpos < CanvasRect.rect.xMin || Xpos > CanvasRect.rect.xMax) || YPos < CanvasRect.rect.yMin || YPos > CanvasRect.rect.yMax;
    }
}
