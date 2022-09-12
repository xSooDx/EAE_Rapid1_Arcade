using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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



    [Space(5)]
    [Header("Ship Initial Setup:")]
    public bool SetPostion;
    [Tooltip("The position where player start")]
    public Vector2 InitialPos;
    public Transform InitialPos_Transform;
    [Tooltip("The velocity add when game start")]
    public Vector2 InitialForce;
    [Tooltip("The rotation of player when start")]
    [Range(-90, 90)]
    public float InitialRotate;

    [SerializeField]
    private List<Sprite> PrizeImgs;

    [Space(5)]
    [Header("UI Setup:")]
    [SerializeField]
    [Tooltip("Game Timer")]
    private float GameTime;

    private Coroutine timecoroutine;

    private Coroutine ResetTimeCoroutine;

    [Tooltip("Text for showing time")]
    public TextMeshProUGUI TimeTxt;

    [Tooltip("Text for game message title")]
    public TextMeshProUGUI GameMsgTitleTxt;

    [Tooltip("Text for game message description")]
    public TextMeshProUGUI GameMsgDESCTxt;

    [Tooltip("Text for score")]
    public TextMeshProUGUI ScoreTxt;

    private bool OutPlanet;

    public float ResetTime;
    private float _RstTimer;

    private void Awake()
    {
        gameController = this;
    }

    private void Start()
    {
        this.PlayerScore = 0;
        this.GameTime = 0;
        this.PrizeImgs = new List<Sprite>();
        iniSetup();
        if (GameEventManager.gameEvent != null) GameEventManager.gameEvent.GameOver.AddListener(GameOverFunc);//add function that will be trigger when game over
        if (GameEventManager.gameEvent != null) GameEventManager.gameEvent.AddScore.AddListener(AddScore);
        _RstTimer = this.ResetTime;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (ScoreTxt != null) ScoreTxt.text = PlayerScore.ToString();
    }

    /// <summary>
    /// thing do when game's over
    /// </summary>
    /// <param name="_ShowTxt"></param>
    /// <param name="_Desc"></param>
    //void GameOverFunc(string _ShowTxt, string _Desc, bool _continue, bool _resetPos)
    //{
    //    StopCoroutine(timecoroutine);
    //    if (GameMsgTitleTxt != null) GameMsgTitleTxt.text = _ShowTxt;
    //    if (GameMsgDESCTxt != null) GameMsgDESCTxt.text = _Desc;
    //    if (_continue)
    //    {
    //        //AddScore(LandingScore);

    //    }
    //    StartCoroutine(RestartGameCounter(_continue, _resetPos));
    //}

    void GameOverFunc(string _ShowTxt, string _Desc, GameEndAction _action)
    {
        SetGameTxt(_ShowTxt, _Desc);
        _action.StartAction();
        StartCoroutine(RestartGameCounter(_action));
        AudioManager.instance.PlayAudio("gameOver");
    }

    public void StartRestart(bool _start)
    {
        if (_start)
        {

                _RstTimer = this.ResetTime;
                ResetTimeCoroutine = StartCoroutine(ResetTimer());
        }
        else
        {
            if (ResetTimeCoroutine != null)
            {
                StopCoroutine(ResetTimeCoroutine);
            }
        }
    }

    IEnumerator ResetTimer()
    {
        while (true)
        {

            yield return new WaitForSeconds(1);
            _RstTimer -= 1f;
            if (_RstTimer <= 0)
            {
                GameEventManager.gameEvent.GameOver.Invoke("Ship Lost", "Too far from planet", GameEndActionsLib.continue_ResetPos);
                break;
            }
        }
    }

    public void AddScore(string _desc, int _score)
    {
        if (UIController.uiController != null)
        {
            UIController.uiController.GetScore(_desc, _score, playerShip.transform.position);
        }
        PlayerScore += _score;
    }

    void iniSetup(bool _rstPos = true)
    {
        if (SetPostion && _rstPos) SetPlayerPos();
        if (!_rstPos) playerShip.SetCanMove();
        SetGameTxt();
        if (ScoreTxt != null) ScoreTxt.text = PlayerScore.ToString();
        timecoroutine = StartCoroutine(TimeCounter());//start timer
        if (GameEventManager.gameEvent != null) GameEventManager.gameEvent.CancelFocus.Invoke(true);
    }

    public void SetScore()
    {
        if (ScoreManager.scoreManager != null)
        {
            ScoreManager.scoreManager.Score = gameController.PlayerScore;
            ScoreManager.scoreManager.GameTime = gameController.GameTime;
            ScoreManager.scoreManager.SetImg(PrizeImgs);
        }
    }

    public void SetPlayerPos()
    {
        if (playerShip != null) playerShip.InitialSetup(InitialPos_Transform == null ? InitialPos : InitialPos_Transform.position, InitialForce, InitialRotate);
    }

    public void SetGameTxt(string _title = "", string _msg = "")
    {
        if (GameMsgTitleTxt != null) GameMsgTitleTxt.text = _title;
        if (GameMsgDESCTxt != null) GameMsgDESCTxt.text = _msg;
    }

    public void AddPrizeImg(Sprite _img)
    {
        PrizeImgs.Add(_img);
    }

    /// <summary>
    /// the count down for reload scene
    /// </summary>
    /// <returns></returns>
    IEnumerator RestartGameCounter(bool _continue, bool _resetPos)
    {
        yield return new WaitForSeconds(3f);
        if (_continue)
        {
            iniSetup(_resetPos);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

    }



    IEnumerator RestartGameCounter(GameEndAction _action)
    {
        yield return new WaitForSeconds(3f);
        _action.EndAction();

    }

    public void TimerAction(bool _Start)
    {
        if (_Start)
        {
            timecoroutine = StartCoroutine(TimeCounter());
        }
        else
        {
            StopCoroutine(timecoroutine);
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
