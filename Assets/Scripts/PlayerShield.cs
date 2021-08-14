using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    public Animator animator;

    public void OnShieldHitFinish()
    {
        animator.SetInteger("State", 0);
        animator.SetBool("Hit", false);
    }

    public void OnShieldAppearFinish()
    {
        animator.SetInteger("State", 0);
    }
}
