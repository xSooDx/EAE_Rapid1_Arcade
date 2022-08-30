using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShipController : MonoBehaviour
{
    private PlayerControl Playerinput;

    private Rigidbody2D _rg;

    public float Speed;

    public float Rot_Speed = 5;

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

    // Start is called before the first frame update
    void Start()
    {
        this._rg = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void FixedUpdate()
    {
        float _Rotate = Playerinput.PlayState.Rotate.ReadValue<float>();
        this._rg.rotation += (_Rotate * Rot_Speed);

        _rg.AddRelativeForce(Vector2.up* Playerinput.PlayState.Push.ReadValue<float>()*Speed);
    }
}

