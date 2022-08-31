using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MainGameController : MonoBehaviour
{
    public static MainGameController gameController;

    public ShipController playerShip;

    public UnityEvent<string,string> GameOver;

    public UnityEvent<Transform> StartFocus;

    public UnityEvent CancelFocus;

    [Header("Ship Initial Setup:")]
    public Vector2 InitialPos;

    public Vector2 InitialForce;

    [Range(-90, 90)]
    public float InitialRotate;

    

    [SerializeField]
    private float GameTime;

    [Tooltip("Text for showing time")]
    public TextMeshProUGUI TimeTxt;

    [Tooltip("Text for showing time")]
    public TextMeshProUGUI GameMsgTitleTxt;

    [Tooltip("Text for showing time")]
    public TextMeshProUGUI GameMsgDESCTxt;

    private void Awake()
    {
        gameController = this;
    }

    private void Start()
    {
        if (playerShip != null) playerShip.InitialSetup(InitialPos, InitialForce, InitialRotate);
        if (GameMsgTitleTxt != null) GameMsgTitleTxt.text = "";
        if (GameMsgDESCTxt != null) GameMsgDESCTxt.text = "";
        this.GameTime = 0;
        StartCoroutine(TimeCounter());
        GameOver.AddListener(GameOverFunc);
    }

    void GameOverFunc(string _ShowTxt,string _Desc)
    {
        if (GameMsgTitleTxt != null) GameMsgTitleTxt.text = _ShowTxt;
        if (GameMsgDESCTxt != null) GameMsgDESCTxt.text = _Desc;
        StartCoroutine(RestartGameCounter());
    }

    IEnumerator RestartGameCounter()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    IEnumerator TimeCounter()
    {
        if (TimeTxt != null)
        {
            TimeTxt.text = (GameTime / 60).ToString("00") + ":" + (GameTime % 60).ToString("00");
        }
        yield return new WaitForSeconds(1f);
        GameTime++;
        StartCoroutine(TimeCounter());
    }
}
