using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraineAnimator : PlayerAnimator
{
    public override void UpdateAnimation()
    {
        animator.SetFloat("xInput", playerController.input.direction.x);
    }

    public void OnEvolAnimDone()
    {
        Player.instance.NextEvolution();
    }
}
