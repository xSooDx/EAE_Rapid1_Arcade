using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class MainGameController : MonoBehaviour
{
    public static MainGameController gameController;

    [Header("Game Setting:")]
    [Tooltip("The player charater")]
    public ShipController playerShip;

    /// <summary>
    /// Event of game over
    /// </summary>
    [HideInInspector]
    public UnityEvent<string,string> GameOver;

    /// <summary>
    /// Event of camera focus on ship when it is low altitude
    /// </summary>
    [HideInInspector]
    public UnityEvent<Transform> StartFocus;

    /// <summary>
    /// Event of camera cancel focus on ship
    /// </summary>
    [HideInInspector]
    public UnityEvent CancelFocus;

    [Space(5)]
    [Header("Ship Initial Setup:")]
    [Tooltip("The position where player start")]
    public Vector2 InitialPos;
    [Tooltip("The velocity add when game start")]
    public Vector2 InitialForce;
    [Tooltip("The rotation of player when start")]
    [Range(-90, 90)]
    public float InitialRotate;

    [Space(5)]
    [Header("UI Setup:")]
    [SerializeField]
    [Tooltip("Game Timer")]
    private float GameTime;

    [Tooltip("Text for showing time")]
    public TextMeshProUGUI TimeTxt;

    [Tooltip("Text for game message title")]
    public TextMeshProUGUI GameMsgTitleTxt;

    [Tooltip("Text for game message description")]
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
        StartCoroutine(TimeCounter());//start timer
        GameOver.AddListener(GameOverFunc);//add function that will be trigger when game over
    }

    /// <summary>
    /// thing do when game's over
    /// </summary>
    /// <param name="_ShowTxt"></param>
    /// <param name="_Desc"></param>
    void GameOverFunc(string _ShowTxt,string _Desc)
    {
        if (GameMsgTitleTxt != null) GameMsgTitleTxt.text = _ShowTxt;
        if (GameMsgDESCTxt != null) GameMsgDESCTxt.text = _Desc;
        StartCoroutine(RestartGameCounter());
    }

    /// <summary>
    /// the count down for reload scene
    /// </summary>
    /// <returns></returns>
    IEnumerator RestartGameCounter()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /// <summary>
    /// time counter
    /// </summary>
    /// <returns></returns>
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
