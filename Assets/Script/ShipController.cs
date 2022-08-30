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
    public float Speed;

    [Tooltip("Rotation Speed")]
    public float Rot_Speed = 5;

    [Space(5)]
    [Header("Movement Info:")]
    [SerializeField]
    private float VerticalSpd;
    [SerializeField]
    private float HorizontalSpd;
    [SerializeField]
    private float RotateAngle;

    [Space(5)]
    [Header("UI setup:")]
    [Tooltip("Text for showing vertical speed.")]
    public TextMeshProUGUI VerticalSpeedTxt;
    [Tooltip("Text for showing Horizontal speed.")]
    public TextMeshProUGUI HorizontalSpeedTxt;

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

        _rg.AddRelativeForce(Vector2.up * Playerinput.PlayState.Push.ReadValue<float>() * Speed);//push the ship

        // calculate the speed
        VerticalSpd = (this._rg.velocity.y * -20f);
        HorizontalSpd = (this._rg.velocity.x * 20f);
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

