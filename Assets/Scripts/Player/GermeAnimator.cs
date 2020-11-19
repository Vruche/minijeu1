using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GermeAnimator : PlayerAnimator
{
    public override void UpdateAnimation()
    {
        animator.SetFloat("Direction", Mathf.Sign(playerController.velocity.x));
        animator.SetFloat("xVelocity", playerController.velocity.x);
        animator.SetFloat("yVelocity", playerController.velocity.y);
        animator.SetBool("isGrounded", playerController.isGrounded);
        animator.SetBool("LookRight", playerController.lookRight);
    }

    public void OnEvolAnimDone()
    {
        Player.instance.NextEvolution();
    }
}
