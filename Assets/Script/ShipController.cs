using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

[RequireComponent(typeof(AudioSource))]
public class ShipController : MonoBehaviour
{
    /// <summary>
    /// input system
    /// </summary>
    private PlayerControl Playerinput;

    /// <summary>
    /// rigidbody of the object
    /// </summary>
    private Rigidbody2D _rg;

    [Header("Ship Setting:")]
    [Tooltip("Speed of the ship")]
    [Range(0, 50)]
    public float Speed;

    [Range(0, 100)]
    public float MaxSpeed;
    [Tooltip("How fast will ship speed up")]
    [Range(0, 200)]
    public float SpeedUpMultiplier;

    [Tooltip("Amount of fuel")]
    public float FuelAmount;

    private float _speed;

    private float PushInput;

    private float RotateInput;

    public bool IsGrabbing;
    public string GrabbingPrizeID;

    private bool ShowInfo;

    private bool GameEnd;

    [Tooltip("Rotation Speed")]
    [Range(0, 20)]
    public float Rot_Speed = 5;

    [Tooltip("How fast will ship rotate speed up")]
    [Range(0, 10)]
    public float RotSpeedUpMultiplier;

    public bool RotateLock;

    [SerializeField]
    private float _RotateSpd;

    [Tooltip("The height that need to be focus on")]
    [Range(0, 100)]
    public float FocusHeight;

    [Range(1, 20)]
    public float SearchField;

    [Tooltip("The layer of ground")]
    public LayerMask GroundLayer;
    public LayerMask PlatformLayer;
    [Tooltip("The offset of height detection(for altitude)")]
    public Vector2 HeightOffest;

    public Transform GrabPoint;

    public CameraScript cameraControl;

    public ParticleSystem emitParticle;

    public ClawAnimationControl animationControl;

    [Space(5)]
    [Header("Movement Info:")]
    [SerializeField]
    private float VerticalSpd;
    [SerializeField]
    private float HorizontalSpd;
    [SerializeField]
    private float RotateAngle;
    [SerializeField]
    private float Altitude;

    [Space(5)]
    [Header("UI setup:")]
    [Tooltip("Text for showing vertical speed.")]
    public TextMeshProUGUI VerticalSpeedTxt;
    [Tooltip("Text for showing Horizontal speed.")]
    public TextMeshProUGUI HorizontalSpeedTxt;
    [Tooltip("Text for showing Altitude.")]
    public TextMeshProUGUI AltitudeTxt;
    [Tooltip("Text for showing fuel amount")]
    public TextMeshProUGUI FuelAmountTxt;

    AudioSource thrustAudioSource;

    private void Awake()
    {
        Playerinput = new PlayerControl();
        this._rg = this.gameObject.GetComponent<Rigidbody2D>();
        if (emitParticle != null) emitParticle.Stop();
        thrustAudioSource = this.gameObject.GetComponent<AudioSource>();
        thrustAudioSource.loop = true;
    }

    private void OnEnable()
    {
        Playerinput.Enable();
       

    }
    private void OnDisable()
    {
        Playerinput.Disable();
    }

    void Start()
    {
        //this._rg = this.gameObject.GetComponent<Rigidbody2D>();
        if (GameEventManager.gameEvent != null)
        {
            GameEventManager.gameEvent.GameOver.AddListener(GameOverAction);
            GameEventManager.gameEvent.PlayerCrash.AddListener(Crash);
        }
        thrustAudioSource.clip = AudioManager.instance.GetAudioClip("shipThrusters");
    }

    void Update()
    {
        if (this.VerticalSpeedTxt != null && ShowInfo)//set the value text
        {
            this.VerticalSpeedTxt.text = VerticalSpd.ToString("0");
        }

        if (this.HorizontalSpeedTxt != null && ShowInfo)//set the value text
        {
            this.HorizontalSpeedTxt.text = HorizontalSpd.ToString("0");
        }

        if (this.AltitudeTxt != null && ShowInfo)//set the value text
        {
            if (Altitude >= 0)
            {
                this.AltitudeTxt.text = Altitude.ToString("0");
            }
            else
            {
                this.AltitudeTxt.text = "N/A";
            }

        }

        if (this.FuelAmountTxt != null)//set the value text
        {
            this.FuelAmountTxt.text = FuelAmount.ToString("0");
        }
        if (FuelAmount <= 0 && GameEventManager.gameEvent != null && !GameEnd)
        {
            GameEventManager.gameEvent.GameOver.Invoke("Game Over", "Out of fuel!!", GameEndActionsLib.gameEnd);
            GameEnd = true;
            return;
        }
        PushInput = Playerinput.PlayState.Push.ReadValue<float>();
        RotateInput = Playerinput.PlayState.Rotate.ReadValue<float>();

        if (PushInput > 0)
        {
            if (_speed < Speed)
            {
                _speed += (_speed + SpeedUpMultiplier) * Time.deltaTime;
            }
            FuelAmount -= Time.deltaTime * 3;
            if (emitParticle != null)
            {
                if (!emitParticle.isEmitting)
                {
                    emitParticle.Play();
                    thrustAudioSource.Play();
                }
            }
        }
        else
        {
            _speed -= (_speed + SpeedUpMultiplier) * Time.deltaTime * 2;
            if (_speed <= 0)
            {
                _speed = 0;
            }
            if (emitParticle != null)
            {
                if (emitParticle.isEmitting)
                {
                    emitParticle.Stop();
                    thrustAudioSource.Stop();
                }

            }
        }

        if (RotateInput != 0)
        {
            if (_RotateSpd < Rot_Speed)
            {
                _RotateSpd += (_RotateSpd + RotSpeedUpMultiplier) * Time.deltaTime;
            }
        }
        else
        {
            _RotateSpd -= (Rot_Speed + RotSpeedUpMultiplier) * Time.deltaTime * 2;
            if (_RotateSpd <= 0)
            {
                _RotateSpd = 0;
            }
        }
        //this._rg.angularVelocity = 0;
        Collider2D[] GroundChk = Physics2D.OverlapCircleAll(this.gameObject.transform.position, SearchField, GroundLayer);
        if (LocationPointer.navSystem != null) LocationPointer.navSystem.UpdatePointer(GroundChk);
        if (GroundChk.Length > 0)
        {
            Vector2 _pos = this.transform.position;
            Transform _planet = null;
            float _distance = -99f;
            foreach (var item in GroundChk)
            {
                _pos = item.gameObject.transform.position;

                float _dis = Vector2.Distance(_pos, (Vector2)this.gameObject.transform.position);

                if (_distance < 0 || _distance >= _dis)
                {
                    _distance = _dis;
                    _planet = item.gameObject.transform;
                }

            }
            if (GameEventManager.gameEvent != null)
            {
                GameEventManager.gameEvent.ClosePlanet.Invoke(_planet);
            }

            //Altitude = _distance * 20f;
            if (_distance >= 0)
            {
                RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position, _pos - (Vector2)this.gameObject.transform.position, 1000f, GroundLayer);
                if (hit.collider != null)
                {
                    Altitude = Vector2.Distance(hit.point, (Vector2)this.gameObject.transform.position + HeightOffest) * 10f;

                    if (GameEventManager.gameEvent != null)
                    {
                        if (Altitude < FocusHeight)
                        {
                            GameEventManager.gameEvent.StartFocus.Invoke();
                        }
                        else
                        {
                            GameEventManager.gameEvent.CancelFocus.Invoke(false);
                        }
                    }

                }

                Collider2D[] PlatformChk = Physics2D.OverlapCircleAll(this.gameObject.transform.position, 3f, PlatformLayer);
                GameEventManager.gameEvent.FocusPlayer.Invoke(PlatformChk.Length > 0);
            }

        }
        else
        {
            Altitude = -1f;
            if (GameEventManager.gameEvent != null)
            {
                GameEventManager.gameEvent.LeavePlanet.Invoke();
            }
        }

        RotateAngle = this.transform.rotation.eulerAngles.z;

        // calculate the speed
        VerticalSpd = Mathf.Abs(this._rg.velocity.y * -20f);
        HorizontalSpd = (this._rg.velocity.x * 20f);


    }

    /// <summary>
    /// Setting when game start
    /// </summary>
    /// <param name="_pos"></param>
    /// <param name="_force"></param>
    /// <param name="_rot"></param>
    public void InitialSetup(Vector2 _pos, Vector2 _force, float _rot)
    {
        SetCanMove();
        //if(this._rg==null) this.gameObject.GetComponent<Rigidbody2D>();
        this._rg.rotation = _rot;
        this._rg.position = _pos;
        this._rg.AddForce(_force);
        if (animationControl != null) animationControl.Resetitem();
    }

    public void SetCanMove()
    {
        ShowInfo = true;
        Playerinput.Enable();
        this.transform.parent = null;
        this._rg.isKinematic = false;
    }

    private void FixedUpdate()
    {


        //this._rg.AddTorque(RotateInput * _RotateSpd);//rotate the ship
        this._rg.rotation += RotateInput * _RotateSpd;

        //set the maximum of angle
        if (RotateLock)
        {
            if (this._rg.rotation < 0 && this._rg.rotation <= -90)
            {
                this._rg.rotation = -90;
            }
            if (this._rg.rotation > 0 && this._rg.rotation >= 90)
            {
                this._rg.rotation = 90;
            }
        }

        // RotateAngle = this._rg.rotation;//set the value(for viewing)


        _rg.AddRelativeForce(Vector2.up * PushInput * _speed);//push the ship
        if (_rg.velocity.magnitude > MaxSpeed)
        {
            _rg.velocity = _rg.velocity.normalized * MaxSpeed;
        }



    }

    /// <summary>
    /// Something need to do when game's over
    /// </summary>
    /// <param name="_str1"></param>
    /// <param name="_str2"></param>
    void GameOverAction(string _str1, string _str2, GameEndAction gameState)
    {
        Playerinput.Disable();
        this._rg.velocity = Vector2.zero;
        this._rg.angularVelocity = 0;
        this._rg.isKinematic = true;
        ShowInfo = false;
    }

    void Crash(Vector2 _dir)
    {
        if (animationControl != null) animationControl.Explosion(_dir);
        AudioManager.instance.PlayAudio("shipDestroy");
        if(GameEventManager.gameEvent!=null)
        {
            GameEventManager.gameEvent.PrizeCrash.Invoke(this.GrabbingPrizeID);
                }
    }


    public float GetVerticalSpd()
    {
        return this.VerticalSpd;
    }

    public float GetHorizontalSpd()
    {
        return this.HorizontalSpd;
    }

    public float GetRotateAngle()
    {
        return this.RotateAngle;
    }

    public Rigidbody2D GetRigidBody()
    {
        return this._rg;
    }

    public Transform GetGrabPoint()
    {
        return GrabPoint;
    }
#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.gameObject.transform.position, SearchField);
    }
#endif
}

