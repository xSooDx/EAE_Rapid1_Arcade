using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresentAnimationControl : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void OpenPresent()
    {
        if (animator != null)
        {
            
            animator.SetTrigger("PresentOpen");
        }
    }
}
