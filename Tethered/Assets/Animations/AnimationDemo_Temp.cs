using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationDemo_Temp : MonoBehaviour
{
    public Animator animator;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        animator.SetTrigger("land");
    }
}
