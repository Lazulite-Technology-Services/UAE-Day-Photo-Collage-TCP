using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEndCallBack : StateMachineBehaviour
{
    // Called when the animation state finishes (after transition)
    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Call a method on the GameObject that owns the animator
        animator.gameObject.SendMessage("OnAnimationComplete", SendMessageOptions.DontRequireReceiver);
        
    }
}
