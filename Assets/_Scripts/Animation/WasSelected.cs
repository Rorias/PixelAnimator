using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class WasSelected : StateMachineBehaviour
{
    public override void OnStateExit(Animator _anim, AnimatorStateInfo _stateInfo, int _layerIndex)
    {
        _anim.SetBool("WasSelected", false);
    }
}
