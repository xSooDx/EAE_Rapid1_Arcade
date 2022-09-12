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

    private bool CheckIsOut(Vector2 _pos)
    {
        return false;
    }
}
