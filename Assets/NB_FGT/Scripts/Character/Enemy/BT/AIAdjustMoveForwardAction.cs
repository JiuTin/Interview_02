using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using NB_FGT.Movement;
using GGG.Tool;
public class AIAdjustMoveForwardAction : Action
{
    private EnemyMovementController _enemyMovementController;
    public override void OnAwake()
    {
        _enemyMovementController = GetComponent<EnemyMovementController>();
    }
    public override TaskStatus OnUpdate()
    {
        if (DevelopmentToos.DistanceForTarget(EnemyManager.MainInstance.GetMainPlayer(), _enemyMovementController.transform) > 3f)
        {
            _enemyMovementController.SetApplyMovement(true);
            _enemyMovementController.SetAnimatorMovementValue(0f,1f);
            return TaskStatus.Running;
        }
        return TaskStatus.Success;
    }

}
