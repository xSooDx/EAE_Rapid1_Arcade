using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PresentAnimationControl : MonoBehaviour
{
    private Animator animator;

    private SpriteRenderer spriteRenderer;

    public List<Sprite> prizeImgs;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        this.spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void OpenPresent()
    {
        if (animator != null)
        {

            animator.SetTrigger("PresentOpen");
        }
    }

    public void SetPrizeImg()
    {

        if (this.prizeImgs.Count > 0 && spriteRenderer != null)
        {
            animator.enabled = false;
            Debug.Log("SET");
            spriteRenderer.sprite = this.prizeImgs[Random.Range(0, prizeImgs.Count)];
            StartCoroutine(PrizeDisappear());
        }
    }

    IEnumerator PrizeDisappear()
    {
        yield return new WaitForSeconds(0.5f);
        this.transform.DOScale(this.transform.localScale * 1.3f, 0.5f);
        yield return new WaitForSeconds(0.5f);
        this.transform.DOScale(Vector2.zero, 0.3f);
    }

}
