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
    public GameObject part;
    public Vector3 originalPosition;
    public Quaternion originalRotation;

    public Collider2D itemCollider;


}

public class ClawAnimationControl : MonoBehaviour
{
    public Sprite NormalStateEmoji;

    public Sprite HappyStateEmoji;

    public Sprite CrashStateEmoji;

    public float ClawOpenRotation;

    public Transform LeftClaw;
    public Transform RightClaw;

    private bool OpenClaw;

    public float explosionForce;

    public SpriteRenderer FaceSprite;

    public List<TransformState> partState;

    private IEnumerator EmojiPlayer;

    private void Awake()
    {
        EmojiPlayer = ChangeEmoji(NormalStateEmoji);
        foreach (var item in partState)
        {
            if (item.part != null)
            {
                item.originalPosition = item.part.transform.localPosition;
                item.originalRotation = item.part.transform.localRotation;
            }
        }
    }
    private void Start()
    {

    }

    public void Resetitem()
    {
        ResetEmoji();
        SetClawOpen(false);
        foreach (var item in partState)
        {
            if (item.part != null)
            {
                if (item.part.GetComponent<Rigidbody2D>() != null)
                {
                    Destroy(item.part.GetComponent<Rigidbody2D>());
                }
                item.part.transform.localPosition = item.originalPosition;
                item.part.transform.localRotation = item.originalRotation;
            }
        }
    }

    private void FixedUpdate()
    {
        float TargetAngle = 0;
        if (OpenClaw)
        {
            TargetAngle = ClawOpenRotation;
        }
        LeftClaw.localRotation = Quaternion.Slerp(LeftClaw.localRotation, Quaternion.Euler(0f, 0f, -1f * TargetAngle), 3f * Time.deltaTime);
        RightClaw.localRotation = Quaternion.Slerp(RightClaw.localRotation, Quaternion.Euler(0f, 0f, TargetAngle), 3f * Time.deltaTime);
    }

    public void Explosion(Vector2 _dir)
    {
        ShockEmoji();
#if UNITY_EDITOR
        if (Application.isPlaying)
#endif
            foreach (var item in partState)
            {
                if (item.part != null)
                {
                    Rigidbody2D _rg = item.part.GetComponent<Rigidbody2D>() == null ? item.part.AddComponent<Rigidbody2D>() : item.part.GetComponent<Rigidbody2D>();
                    item.itemCollider.enabled = true;
                    _rg.gravityScale = 0;
                    _rg.mass = 25;
                    //_rg.drag = 0.5f;
                    _rg.AddForce(new Vector2(Random.Range(_dir.x - 10, _dir.x + 10) * explosionForce, Random.Range(_dir.y, _dir.y + 10) * explosionForce));
                    _rg.AddTorque(Random.Range(-1, 2) * 800);
                }
            }
    }

    public void SetClawOpen(bool _open)
    {
        this.OpenClaw = _open;
    }

    public void ResetEmoji()
    {
        if (FaceSprite != null && NormalStateEmoji != null)
        {
            StopCoroutine(EmojiPlayer);
            FaceSprite.sprite = NormalStateEmoji;
        }
    }

    public void ShockEmoji()
    {
        if (FaceSprite != null && CrashStateEmoji != null)
        {
            StopCoroutine(EmojiPlayer);
            EmojiPlayer = ChangeEmoji(CrashStateEmoji);
            StartCoroutine(EmojiPlayer);
        }
    }

    public void HappyEmoji()
    {
        if (FaceSprite != null && HappyStateEmoji != null)
        {
            StopCoroutine(EmojiPlayer);
            EmojiPlayer = ChangeEmoji(HappyStateEmoji);
            StartCoroutine(ChangeEmoji(HappyStateEmoji));
        }
    }

    IEnumerator ChangeEmoji(Sprite _emoji)
    {
        FaceSprite.sprite = _emoji;
        yield return new WaitForSeconds(3f);
        FaceSprite.sprite = NormalStateEmoji;
    }
}
