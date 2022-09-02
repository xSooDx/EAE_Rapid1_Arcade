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

    [Tooltip("The score that landing will get")]
    public int LandingScore;

    [Header("Game Info:")]
    [SerializeField]
    private int PlayerScore;

    /// <summary>
    /// Event of game over
    /// </summary>
    [HideInInspector]
    public UnityEvent<string,string,bool> GameOver;

    /// <summary>
    /// Event of camera focus on ship when it is low altitude
    /// </summary>
    [HideInInspector]
    public UnityEvent StartFocus;

    /// <summary>
    /// Event of camera cancel focus on ship
    /// </summary>
    [HideInInspector]
    public UnityEvent<bool> CancelFocus;

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

    private Coroutine timecoroutine;

    [Tooltip("Text for showing time")]
    public TextMeshProUGUI TimeTxt;

    [Tooltip("Text for game message title")]
    public TextMeshProUGUI GameMsgTitleTxt;

    [Tooltip("Text for game message description")]
    public TextMeshProUGUI GameMsgDESCTxt;

    [Tooltip("Text for score")]
    public TextMeshProUGUI ScoreTxt;

    private void Awake()
    {
        gameController = this;
    }

    private void Start()
    {
        this.PlayerScore = 0;
        this.GameTime = 0;
        iniSetup();
        GameOver.AddListener(GameOverFunc);//add function that will be trigger when game over
        //gameController.GameOver.Invoke("CALL!!", "TEST", true);
    }

    /// <summary>
    /// thing do when game's over
    /// </summary>
    /// <param name="_ShowTxt"></param>
    /// <param name="_Desc"></param>
    void GameOverFunc(string _ShowTxt,string _Desc,bool _continue)
    {
        StopCoroutine(timecoroutine);
        if (GameMsgTitleTxt != null) GameMsgTitleTxt.text = _ShowTxt;
        if (GameMsgDESCTxt != null) GameMsgDESCTxt.text = _Desc;
        if (_continue)
        {
            PlayerScore += LandingScore;
            if (ScoreTxt != null) ScoreTxt.text = PlayerScore.ToString();
        }
        StartCoroutine(RestartGameCounter(_continue));
    }

    void NewRound()
    {
        iniSetup();
    }

    void iniSetup()
    {
        if (playerShip != null) playerShip.InitialSetup(InitialPos, InitialForce, InitialRotate);
        if (GameMsgTitleTxt != null) GameMsgTitleTxt.text = "";
        if (GameMsgDESCTxt != null) GameMsgDESCTxt.text = "";
        if (ScoreTxt != null) ScoreTxt.text = PlayerScore.ToString();
        if (playerShip != null) playerShip.InitialSetup(InitialPos, InitialForce, InitialRotate);
        timecoroutine = StartCoroutine(TimeCounter());//start timer
        gameController.CancelFocus.Invoke(true);
    }

    /// <summary>
    /// the count down for reload scene
    /// </summary>
    /// <returns></returns>
    IEnumerator RestartGameCounter(bool _continue)
    {
        yield return new WaitForSeconds(3f);
        if (_continue)
        {
            NewRound();
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        
    }

    /// <summary>
    /// time counter
    /// </summary>
    /// <returns></returns>
    IEnumerator TimeCounter()
    {
        while (true)
        {
            if (TimeTxt != null)
            {
                TimeTxt.text = (GameTime / 60).ToString("00") + ":" + (GameTime % 60).ToString("00");
            }
            yield return new WaitForSeconds(1f);
            GameTime++;
        }
        
    }
}
