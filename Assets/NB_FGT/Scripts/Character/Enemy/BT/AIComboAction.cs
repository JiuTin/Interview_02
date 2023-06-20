using BehaviorDesigner.Runtime.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NB_FGT.Combat;
public class AIComboAction : Action
{
    EnemyCombatControl _enemyCombatControl;

    public override void OnAwake()
    {
        _enemyCombatControl = GetComponent<EnemyCombatControl>();
    }

    public override TaskStatus OnUpdate()
    {
        if (_enemyCombatControl.GetCombatCommand())
        {
            _enemyCombatControl.AIBaseAttackInput();
            return TaskStatus.Running;
        }

        return TaskStatus.Success;
    }
}
