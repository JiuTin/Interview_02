using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using NB_FGT.Combat;
public class AIAttackCommandCondition : Conditional
{
    EnemyCombatControl _enemyCombatControl;
    public override void OnAwake()
    {
        _enemyCombatControl = GetComponent<EnemyCombatControl>();
    }
    public override TaskStatus OnUpdate()
    {
        return (_enemyCombatControl.GetCombatCommand() ? TaskStatus.Success : TaskStatus.Failure);
    }
}
