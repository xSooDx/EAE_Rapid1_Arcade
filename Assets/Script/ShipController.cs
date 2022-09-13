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
    /// <summary>
    /// speed add to player
    /// </summary>
    private float _speed;
    /// <summary>
    /// fire thruster input value
    /// </summary>
    private float PushInput;
    /// <summary>
    /// rotate input value
    /// </summary>
    private float RotateInput;

    /// <summary>
    /// whether showing the info of player(for UI)
    /// </summary>
    private bool ShowInfo;
    /// <summary>
    /// is game over
    /// </summary>
    private bool GameEnd;

    [Tooltip("Rotation Speed")]
    [Range(0, 20)]
    public float Rot_Speed = 5;

    [Tooltip("How fast will ship rotate speed up")]
    [Range(0, 10)]
    public float RotSpeedUpMultiplier;
    [Tooltip("Set whether limit the rotate angle")]
    public bool RotateLock;



    [Tooltip("The height that need to be focus on")]
    [Range(0, 100)]
    public float FocusHeight;
    [Tooltip("The field range for detecting planet")]
    [Range(1, 20)]
    public float SearchField;

    [Tooltip("The layer of ground")]
    public LayerMask GroundLayer;
    [Tooltip("The layer of platform")]
    public LayerMask PlatformLayer;
    [Tooltip("The layer setting for checking whether player is in safe speed or rotation and so on")]
    public LayerMask StatusLayerCheck;
    [Tooltip("The offset of height detection(for altitude)")]
    public Vector2 HeightOffest;

    [Tooltip("The position of present going to be")]
    public Transform GrabPoint;
    [Tooltip("Particle which emit when fire thruster")]
    public ParticleSystem emitParticle;
    [Tooltip("Animation of player ship")]
    public ClawAnimationControl animationControl;

    [Space(5)]
    [Header("Ship Info:")]
    public bool IsGrabbing;
    public string GrabbingPrizeID;

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
    [SerializeField]
    private float _RotateSpd;

    [Space(5)]
    [Header("UI setup:")]
    [Tooltip("Text for showing vertical speed.")]
    public TextMeshProUGUI VerticalSpeedTxt;
    [Tooltip("Text for showing Horizontal speed.")]
    public TextMeshProUGUI HorizontalSpeedTxt;
    [Tooltip("Text for showing Altitude.")]
    public TextMeshProUGUI RotationTxt;
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

        if (this.RotationTxt != null && ShowInfo)//set the value text
        {
            this.RotationTxt.text = RotateAngle.ToString("0");
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
        if (FuelAmount <= 0 && GameEventManager.gameEvent != null && !GameEnd)//if fuel's out, end game
        {
            GameEventManager.gameEvent.GameOver.Invoke("Game Over", "Out of fuel!!", GameEndActionsLib.gameEnd);
            GameEnd = true;
            return;
        }
        PushInput = Playerinput.PlayState.Push.ReadValue<float>();//get whether press UP(W)
        RotateInput = Playerinput.PlayState.Rotate.ReadValue<float>();//get whether press Left Right A D

        if (PushInput > 0)
        {
            if (_speed < Speed)//if current add speed is less than speed setup
            {
                _speed += (_speed + SpeedUpMultiplier) * Time.deltaTime;
            }
            FuelAmount -= Time.deltaTime * 3;
            if (emitParticle != null)//emit the particle
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
            //decrease the add speed
            _speed -= (_speed + SpeedUpMultiplier) * Time.deltaTime * 2;
            if (_speed <= 0)
            {
                _speed = 0;
            }
            if (emitParticle != null)//stop emit the particle
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
            if (_RotateSpd < Rot_Speed)// add rotate speed
            {
                _RotateSpd += (_RotateSpd + RotSpeedUpMultiplier) * Time.deltaTime;
            }
        }
        else
        {
            //decrease rotate speed
            _RotateSpd -= (Rot_Speed + RotSpeedUpMultiplier) * Time.deltaTime * 2;
            if (_RotateSpd <= 0)
            {
                _RotateSpd = 0;
            }
        }
        //check whether touch any planet
        Collider2D[] GroundChk = Physics2D.OverlapCircleAll(this.gameObject.transform.position, SearchField, GroundLayer);
        if (LocationPointer.navSystem != null) LocationPointer.navSystem.UpdatePointer(GroundChk);
        if (GroundChk.Length > 0)
        {
            //add and calculate which planet most close to player
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
            //trigger the event of closing to planet
            if (GameEventManager.gameEvent != null)
            {
                GameEventManager.gameEvent.ClosePlanet.Invoke(_planet);
            }


            if (_distance >= 0)
            {
                //check the height player to ground, set the animation or zoom the camera
                RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position, _pos - (Vector2)this.gameObject.transform.position, 1000f, GroundLayer);
                if (hit.collider != null)
                {
                    Altitude = Vector2.Distance(hit.point, (Vector2)this.gameObject.transform.position + HeightOffest) * 10f;

                    if (GameEventManager.gameEvent != null)
                    {
                        if (Altitude < FocusHeight)
                        {
                            if (!IsGrabbing)
                            {
                                if (animationControl != null) animationControl.SetClawOpen(true);
                            }
                            else
                            {
                                if (animationControl != null) animationControl.SetClawOpen(false);
                            }
                            GameEventManager.gameEvent.StartFocus.Invoke();
                        }
                        else
                        {
                            if (animationControl != null) animationControl.SetClawOpen(false);
                            GameEventManager.gameEvent.CancelFocus.Invoke(false);
                        }
                    }

                }

                //check whether player is safe when hit the ground, if it's danger, show warning
                RaycastHit2D _WarningHit = Physics2D.Raycast((Vector2)transform.position, _pos - (Vector2)this.gameObject.transform.position, 1000f, StatusLayerCheck);
                float _alt = Vector2.Distance(_WarningHit.point, (Vector2)this.gameObject.transform.position + HeightOffest) * 10f;
                if (_WarningHit.collider != null && _alt < 50 && GameEventManager.gameEvent != null && (!IsGrabbing || _WarningHit.collider.gameObject.layer == LayerMask.NameToLayer("Platform")))
                {
                    LandingPointScript landingPoint = _WarningHit.collider.gameObject.GetComponent<LandingPointScript>();
                    if (landingPoint != null)
                    {
                        bool VS = GetVerticalSpd() > landingPoint.Req_MAXVerticalSpeed;
                        GameEventManager.gameEvent.SetWarning.Invoke("VS", VS);

                        bool HS = Mathf.Abs(GetHorizontalSpd()) > landingPoint.Req_MAXHorizonSpeed;
                        GameEventManager.gameEvent.SetWarning.Invoke("HS", HS);

                        bool AG = _WarningHit.collider.gameObject.layer == LayerMask.NameToLayer("Platform") ? !landingPoint.RotationChk(GetRotateAngle()) : !landingPoint.RotationChk(transform.position - landingPoint.transform.position, GetRotateAngle());
                        GameEventManager.gameEvent.SetWarning.Invoke("AG", AG);

                        GameEventManager.gameEvent.SetWarning.Invoke("SIGN", VS || HS || AG);

                        GameEventManager.gameEvent.SetWarning.Invoke("SPD", VS || HS);
                    }
                }
                else
                {
                    GameEventManager.gameEvent.SetWarning.Invoke("ALL", false);
                }

                Collider2D[] PlatformChk = Physics2D.OverlapCircleAll(this.gameObject.transform.position, 3f, PlatformLayer);
                GameEventManager.gameEvent.FocusPlayer.Invoke(PlatformChk.Length > 0);
            }

        }
        else
        {
            //player leave planet
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
        this._rg.rotation = _rot;
        this._rg.position = _pos;
        this._rg.AddForce(_force);
        if (animationControl != null) animationControl.Resetitem();
    }

    /// <summary>
    /// set can charater move
    /// </summary>
    public void SetCanMove()
    {
        ShowInfo = true;
        Playerinput.Enable();
        this.transform.parent = null;
        this._rg.isKinematic = false;
    }

    private void FixedUpdate()
    {


        //this._rg.AddTorque(RotateInput * _RotateSpd);
        this._rg.rotation += RotateInput * _RotateSpd;//rotate the ship

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

    /// <summary>
    /// function when player crush
    /// </summary>
    /// <param name="_dir">crush position</param>
    void Crash(Vector2 _dir)
    {
        if (animationControl != null) animationControl.Explosion(_dir);
        AudioManager.instance.PlayAudio("shipDestroy");
        if (GameEventManager.gameEvent != null)
        {
            GameEventManager.gameEvent.PrizeCrash.Invoke(this.GrabbingPrizeID);//release the item player grabbing
        }
        if (MainGameController.gameController != null)
        {
            FuelAmount -= MainGameController.gameController.FuelLoss;
            if (FuelAmount < 0)
            {
                FuelAmount = 0;
            }
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

