using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatusEffectNet : MonoBehaviour
{
    public Player player;
    public Animator animator;
    
    public void OnStatusEffectNetFinished()
    {
        animator.SetBool("Netted", false);
        player.RestoreSlowedSpeed();
    }
}
