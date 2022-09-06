using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ClawState
{
    public Sprite Emoji;
    public float ClawAngle;
}

[System.Serializable]
public class TransformState
{
    public Transform part;
    [HideInInspector]
    public Vector3 originalPosition;
    [HideInInspector]
    public Quaternion originalRotation;
}

public class ClawAnimationControl : MonoBehaviour
{
    public ClawState NormalState;

    public ClawState GrabState;

    public ClawState CrashState;

    public List<TransformState> partState;

    private void Awake()
    {
        foreach (var item in partState)
        {
            if (item.part != null)
            {
                item.originalPosition = item.part.position;
                item.originalRotation = item.part.rotation;
            }
        }
    }
}
