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

    [Tooltip("The camera which you want to show on the UI")]
    public Camera m_Camera;
    [Tooltip("The canvas of UI")]
    public Canvas MainCanvas;
    [Tooltip("The canvas RectTransform of UI")]
    private RectTransform CanvasRect;
    [Tooltip("The adding score popup UI")]
    public GameObject AddScoreUI;

    public GameObject WarningSign;
    public GameObject AngleSign;
    public GameObject SpeedSign;

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
            //GameEventManager.gameEvent.PlayerCrash.AddListener(ResetWarning);
        }
        if (WarningSign != null)
            WarningSign.SetActive(false);
        if (AngleSign != null)
            AngleSign.SetActive(false);
        if (SpeedSign != null)
            SpeedSign.SetActive(false);


    }

    private void FixedUpdate()
    {
        if (ShowPointer)
        {
            SetPointer();
        }
        else if (targetPointers.Count > 0)
        {
            foreach (var item in targetPointers)
            {
                item.Pointer_UI.gameObject.SetActive(false);
            }
        }
    }
    /// <summary>
    /// set the pointer position and rotation
    /// </summary>
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

    /// <summary>
    /// get the platform in scene and spawn pointer for them
    /// </summary>
    void GetPlatform()
    {
        if (PointerObj == null) return;
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

    /// <summary>
    /// show score text function
    /// </summary>
    /// <param name="_desc"></param>
    /// <param name="_score"></param>
    /// <param name="_pos"></param>
    public void GetScore(string _desc, int _score, Vector2 _pos)
    {
        if (CanvasRect != null)
        {

            StartCoroutine(AddScore(_desc + "\n+" + _score.ToString(), _pos));
        }

    }

    /// <summary>
    /// spawn text gameobject
    /// </summary>
    /// <param name="_desc"></param>
    /// <param name="_pos"></param>
    void SpawnTxt(string _desc, Vector2 _pos)
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
            _txt.text = _desc;
        }
        Destroy(_obj, 2f);
    }

    IEnumerator AddScore(string _desc, Vector2 _pos)
    {
        SpawnTxt(_desc,_pos);
        yield return new WaitForSeconds(1.5f);
        if (MainGameController.gameController != null && MainGameController.gameController.FuelAdd > 0)
        {
            SpawnTxt("Fuel add!!\n+"+ MainGameController.gameController.FuelAdd, _pos);
        }
    }

    /// <summary>
    /// calculate the position on canvas
    /// </summary>
    /// <param name="_VP"></param>
    /// <returns></returns>
    private Vector2 WorldtoScreenPos(Vector2 _VP)
    {
        float Xpos = (_VP.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f);
        float YPos = (_VP.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f);
        return new Vector2(Xpos, YPos);
    }

    void SetWarning(string _tag, bool _Warning)
    {
        if (WarningSign != null && (_tag.Equals("SIGN") || _tag.Equals("ALL")))
        {
            WarningSign.SetActive(_Warning);
        }

        if (AngleSign != null && (_tag.Equals("AG") || _tag.Equals("ALL")))
        {
            AngleSign.SetActive(_Warning);
        }

        if (SpeedSign != null && (_tag.Equals("SPD") || _tag.Equals("ALL")))
        {
            SpeedSign.SetActive(_Warning);
        }
    }

    void ResetWarning(Vector2 _pos)
    {
        WarningSign.SetActive(false);

        AngleSign.SetActive(false);

        SpeedSign.SetActive(false);

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
