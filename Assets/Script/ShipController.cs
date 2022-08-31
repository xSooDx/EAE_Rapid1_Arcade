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

    [Header("Ship Movement Parameter:")]
    [Tooltip("Speed of the ship")]
    [Range(0, 50)]
    public float Speed;

    private float _speed;

    [Tooltip("Rotation Speed")]
    [Range(0, 20)]
    public float Rot_Speed = 5;

    [Range(0, 100)]
    public float FocusHeight;

    public LayerMask GroundLayer;

    public Vector2 HeightOffest;

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

    private void Awake()
    {
        Playerinput = new PlayerControl();
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
        this._rg = gameObject.GetComponent<Rigidbody2D>();
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

        RaycastHit2D hit = Physics2D.Raycast((Vector2)transform.position, -Vector2.up , 1000f, GroundLayer);
        if (hit.collider != null)
        {
            Altitude = Vector2.Distance(hit.point, (Vector2)this.gameObject.transform.position+ HeightOffest) * 20f;
            if (Altitude < FocusHeight && MainGameController.gameController != null)
            {
                MainGameController.gameController.StartFocus.Invoke(this.transform);
            }
            else
            {
                MainGameController.gameController.CancelFocus.Invoke();
            }
        }
    }

    public void InitialSetup(Vector2 _pos, Vector2 _force, float _rot)
    {
        this._rg.rotation = _rot;
        this._rg.position = _pos;
        this._rg.AddForce(_force);
    }

    private void FixedUpdate()
    {

        float _Rotate = Playerinput.PlayState.Rotate.ReadValue<float>();
        this._rg.AddTorque(_Rotate * Rot_Speed);//rotate the ship

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

        float _push = Playerinput.PlayState.Push.ReadValue<float>();
        if (_push > 0)
        {
            if (_speed < Speed)
            {
                _speed += Speed / 10;
            }
            _rg.AddRelativeForce(Vector2.up * _push * _speed);//push the ship
        }
        else
        {
            _speed = 0;
        }


        // calculate the speed
        VerticalSpd = (this._rg.velocity.y * -20f);
        HorizontalSpd = (this._rg.velocity.x * 20f);
    }

    void GameOverAction(string _str1, string _str2)
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

}

