using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

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
    [Tooltip("How fast will ship speed up")]
    [Range(0, 10)]
    public float SpeedUpMultiplier;

    [Tooltip("Amount of fuel")]
    public float FuelAmount;

    private float _speed;

    private float PushInput;
    private float RotateInput;

    [Tooltip("Rotation Speed")]
    [Range(0, 20)]
    public float Rot_Speed = 5;

    [Tooltip("The height that need to be focus on")]
    [Range(0, 100)]
    public float FocusHeight;

    [Tooltip("The layer of ground")]
    public LayerMask GroundLayer;
    [Tooltip("The offset of height detection(for altitude)")]
    public Vector2 HeightOffest;

    public Vector2 CargoPosition;

    public CameraScript cameraControl;

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

    private void Awake()
    {
        Playerinput = new PlayerControl();
        this._rg = this.gameObject.GetComponent<Rigidbody2D>();
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
        if (MainGameController.gameController != null) MainGameController.gameController.GameOver.AddListener(GameOverAction);
    }

    void Update()
    {
        if (this.VerticalSpeedTxt != null)//set the value text
        {
            this.VerticalSpeedTxt.text = VerticalSpd.ToString("0");
        }

        if (this.HorizontalSpeedTxt != null)//set the value text
        {
            this.HorizontalSpeedTxt.text = HorizontalSpd.ToString("0");
        }

        if (this.AltitudeTxt != null)//set the value text
        {
            this.AltitudeTxt.text = Altitude.ToString("0");
        }

        if (this.FuelAmountTxt != null)//set the value text
        {
            this.FuelAmountTxt.text = FuelAmount.ToString("0");
        }
        if (FuelAmount <= 0 && MainGameController.gameController != null)
        {
            MainGameController.gameController.GameOver.Invoke("Game Over", "Out of fuel!!", false);
        }
        PushInput = Playerinput.PlayState.Push.ReadValue<float>();
        RotateInput = Playerinput.PlayState.Rotate.ReadValue<float>();

        if (PushInput > 0)
        {
            if (_speed < Speed)
            {
                _speed += Speed * Time.deltaTime * SpeedUpMultiplier;
            }
            FuelAmount -= Time.deltaTime * 3;
        }
        else
        {
            _speed -= Speed * Time.deltaTime * SpeedUpMultiplier * 2;
            if (_speed <= 0)
            {
                _speed = 0;
            }
        }

        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position, -Vector2.up, 1000f, GroundLayer);
        if (hit.collider != null)
        {
            Altitude = Vector2.Distance(hit.point, (Vector2)this.gameObject.transform.position + HeightOffest) * 20f;

            if (MainGameController.gameController != null)
            {
                if (Altitude < FocusHeight)
                {
                    MainGameController.gameController.StartFocus.Invoke();
                }
                else
                {
                    MainGameController.gameController.CancelFocus.Invoke(false);
                }
            }
            else if (cameraControl != null)
            {
                if (Altitude < FocusHeight)
                {
                    cameraControl.FocusObject();
                }
                else
                {
                    cameraControl.CancelFocus(false);
                }
            }

        }
    }

    /// <summary>
    /// Setting when game start
    /// </summary>
    /// <param name="_pos"></param>
    /// <param name="_force"></param>
    /// <param name="_rot"></param>
    public void InitialSetup(Vector2 _pos, Vector2 _force, float _rot)
    {
        Playerinput.Enable();
        //if(this._rg==null) this.gameObject.GetComponent<Rigidbody2D>();
        this._rg.isKinematic = false;
        this._rg.rotation = _rot;
        this._rg.position = _pos;
        this._rg.AddForce(_force);
    }

    private void FixedUpdate()
    {


        this._rg.AddTorque(RotateInput * Rot_Speed);//rotate the ship

        //set the maximum of angle
        if (this._rg.rotation < 0 && this._rg.rotation <= -90)
        {
            this._rg.rotation = -90;
        }
        if (this._rg.rotation > 0 && this._rg.rotation >= 90)
        {
            this._rg.rotation = 90;
        }
        RotateAngle = this._rg.rotation;//set the value(for viewing)

        _rg.AddRelativeForce(Vector2.up * PushInput * _speed);//push the ship



        // calculate the speed
        VerticalSpd = (this._rg.velocity.y * -20f);
        HorizontalSpd = (this._rg.velocity.x * 20f);
    }

    /// <summary>
    /// Something need to do when game's over
    /// </summary>
    /// <param name="_str1"></param>
    /// <param name="_str2"></param>
    void GameOverAction(string _str1, string _str2, bool _continue)
    {
        Playerinput.Disable();
        this._rg.velocity = Vector2.zero;
        this._rg.angularVelocity = 0;
        this._rg.isKinematic = true;
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

    public Vector2 GetCargoPos()
    {
        return (Vector2)this.transform.position + CargoPosition;
    }

    public Rigidbody2D GetRigidBody()
    {
        return this._rg;
    }
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(CargoPosition, 0.1f);
    }
#endif

}

