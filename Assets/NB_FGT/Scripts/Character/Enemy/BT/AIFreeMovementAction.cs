using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using NB_FGT.Movement;
using NB_FGT.Combat;
using GGG.Tool;
public class AIFreeMovementAction : Action
{
    //1.在没有被指派攻击指令时，处于限制自由移动
    //2.要么左右移动，要么前后移动
    //3.要么播放某些动画，比如：Idle或嘲讽
    //4.在离玩家过近时，让AI后退
    private EnemyMovementController _enemyMovementController;
    private EnemyCombatControl _enemyCombatController;


    private int _actionIndex;   //动作索引，不同的值代表执行不同动作
    private int _lastActionIndex;   //上一个节点
    private float _actionTimer;   //动作执行时间

    public override void OnAwake()
    {
        _enemyMovementController = GetComponent<EnemyMovementController>();
        _enemyCombatController = GetComponent<EnemyCombatControl>();
        _lastActionIndex = _actionIndex;
        _actionTimer = 1f;
    }
    public override TaskStatus OnUpdate()
    {
        //1.判断一下，自身是否有攻击指令
        if (!_enemyCombatController.GetCombatCommand())
        {
            //判断是否在距离内
            if (DistanceForTarget() > 8.0f)
            {
                _enemyMovementController.SetAnimatorMovementValue(0f, 1f);
            }
            else if (DistanceForTarget() < 8.0f + 0.1f && DistanceForTarget() > 3.0f + 0.1f)
            {
                FreeMovement();
                UpdateFreeActioin();
            }
            else
            {
                _enemyMovementController.SetAnimatorMovementValue(1f, -1f);
            }

            //处于当前节点的逻辑
            return TaskStatus.Running;
        }
        else
        { 
            //做一些退出逻辑
        }
        //节点结束
        return TaskStatus.Success;
    }

    private float DistanceForTarget() => DevelopmentToos.DistanceForTarget(EnemyManager.MainInstance.GetMainPlayer(),transform);

    //1.AI启动，先检测攻击指令
    //2.攻击指令没有被激活，那么判断与目标的距离，过远就朝着目标移动
    //3.当抵达安全距离，攻击指令还没有被激活
    //4.那么让AI执行各种稀奇古怪的动作
    //5.当攻击指令被激活，退出节点

    /// <summary>
    /// 执行左右移动动作
    /// </summary>
    private void FreeMovement()
    {
        switch (_actionIndex)
        {
            case 0://往左移动
                _enemyMovementController.SetAnimatorMovementValue(-1f, 0f);
                break;
            case 1://往右移动
                _enemyMovementController.SetAnimatorMovementValue(1f, 0f);
                break;
            case 2://站立
                _enemyMovementController.SetAnimatorMovementValue(0f, 0f);
                break;
            case 3://左后退
                _enemyMovementController.SetAnimatorMovementValue(-1f, -1f);
                break;
            case 4://右后退
                _enemyMovementController.SetAnimatorMovementValue(1f, -1f);
                break;
        }
    }
    /// <summary>
    /// 更新动作持续时间
    /// </summary>
    private void UpdateFreeActioin()
    {
        if (_actionTimer > 0)
        {
            _actionTimer -= Time.deltaTime;
            if (_actionTimer <= 0f)
            {
                UpdateActionIndex();
            }
        }
    }
    /// <summary>
    /// 更新动作索引
    /// </summary>
    private void UpdateActionIndex()
    {
        _actionTimer = Random.Range(2f,3f);
        _lastActionIndex = _actionIndex;
        _actionIndex = Random.Range(0, 5);
        if (_lastActionIndex == _actionIndex)
        {
            _actionIndex = Random.Range(0, 2);
        }
        if (_actionIndex == 3 || _actionIndex == 5)
        {
            _actionTimer = 1f;
        }
    }

}
