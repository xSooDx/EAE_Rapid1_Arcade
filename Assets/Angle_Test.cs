using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Angle_Test : MonoBehaviour
{
    public Transform obj;
    [SerializeField]
    private float _angle;

    private void Update()
    {
        Vector2 _dir = this.transform.position - obj.position;
        int sign = (_dir.x >= 0) ? -1 : 1;
        int offset = (sign >= 0) ? 0 : 360;
        _angle = Vector2.Angle(_dir, Vector2.up) * sign + offset;
    }
}
