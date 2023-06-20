using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using NB_FGT.Combat;
using GGG.Tool;
public class AIAttackDistanceCondition : Conditional
{
    EnemyCombatControl _enemyCombatControl;
    [SerializeField]private float _attackDistance;
    public override void OnAwake()
    {
        _enemyCombatControl = GetComponent<EnemyCombatControl>();
    }
    public override TaskStatus OnUpdate()
    {
        return (DevelopmentToos.DistanceForTarget(
            EnemyManager.MainInstance.GetMainPlayer(), _enemyCombatControl.transform) > _attackDistance
            ? TaskStatus.Success : TaskStatus.Failure
            );
    }

}
