using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// the basic function of a landing position
/// </summary>
public abstract class LandingPointScript : MonoBehaviour
{
    [Tooltip("Vertical speed exceed this value, crash")]
    public float Req_MAXVerticalSpeed;

    [Tooltip("Horizontal speed exceed this value, crash")]
    public float Req_MAXHorizonSpeed;

    [Tooltip("Rotation requirement")]
    public float Req_RotateAngle;
    [Tooltip("Rotation requirement tolerance")]
    public float Req_RotateAngleTor;

    public Vector2 Direction;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        TouchAction(collision);
    }

    /// <summary>
    /// check if the rotation is correct(base on position of the landing spot)
    /// </summary>
    /// <param name="_dir"></param>
    /// <param name="_rotAngle"></param>
    /// <param name="_localrot"></param>
    /// <returns></returns>
    public bool RotationChk(Vector2 _dir, float _rotAngle, float _localrot = 0)
    {
        int sign = (_dir.x >= 0) ? -1 : 1;
        int offset = (sign >= 0) ? 0 : 360;
        float angle = Vector2.Angle(_dir, Vector2.up) * sign + offset + _localrot;
        Debug.Log(angle);
        float _min = angle - Req_RotateAngleTor;
        float _MAX = angle + Req_RotateAngleTor;
        if (_min < 0)
        {
            if (_rotAngle >= _min + 360 || _rotAngle <= _MAX)
            {
                return true;
            }
        }
        else if (_MAX > 360)
        {
            if (_rotAngle >= _min || _rotAngle <= _MAX - 360)
            {
                return true;
            }
        }
        else
        {
            if (_rotAngle >= _min && _rotAngle <= _MAX)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// check the rotation is correct
    /// </summary>
    /// <param name="_rotAngle"></param>
    /// <returns></returns>
    public bool RotationChk(float _rotAngle)
    {
        float angle = this.transform.rotation.eulerAngles.z;

        float _min = angle - Req_RotateAngleTor;
        float _MAX = angle + Req_RotateAngleTor;
        //Debug.Log(_min + "~" + _MAX);
        if (_min < 0)
        {
            if (_rotAngle >= _min + 360 || _rotAngle <= _MAX)
            {
                return true;
            }
        }
        else if (_MAX > 360)
        {
            if (_rotAngle >= _min || _rotAngle <= _MAX - 360)
            {
                return true;
            }
        }
        else
        {
            if (_rotAngle >= _min && _rotAngle <= _MAX)
            {
                return true;
            }
        }

        return false;
    }


    /// <summary>
    /// function when something land on this object
    /// </summary>
    /// <param name="_col"></param>
    public virtual void TouchAction(Collision2D _col)
    {
        Debug.Log("Touch " + _col.gameObject.name);
    }


}
