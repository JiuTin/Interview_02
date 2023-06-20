using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BehaviorDesigner.Runtime.Tasks;
using NB_FGT.Movement;
using NB_FGT.Combat;
using GGG.Tool;
public class AIFreeMovementAction : Action
{
    //1.��û�б�ָ�ɹ���ָ��ʱ���������������ƶ�
    //2.Ҫô�����ƶ���Ҫôǰ���ƶ�
    //3.Ҫô����ĳЩ���������磺Idle�򳰷�
    //4.������ҹ���ʱ����AI����
    private EnemyMovementController _enemyMovementController;
    private EnemyCombatControl _enemyCombatController;


    private int _actionIndex;   //������������ͬ��ֵ����ִ�в�ͬ����
    private int _lastActionIndex;   //��һ���ڵ�
    private float _actionTimer;   //����ִ��ʱ��

    public override void OnAwake()
    {
        _enemyMovementController = GetComponent<EnemyMovementController>();
        _enemyCombatController = GetComponent<EnemyCombatControl>();
        _lastActionIndex = _actionIndex;
        _actionTimer = 1f;
    }
    public override TaskStatus OnUpdate()
    {
        //1.�ж�һ�£������Ƿ��й���ָ��
        if (!_enemyCombatController.GetCombatCommand())
        {
            //�ж��Ƿ��ھ�����
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

            //���ڵ�ǰ�ڵ���߼�
            return TaskStatus.Running;
        }
        else
        { 
            //��һЩ�˳��߼�
        }
        //�ڵ����
        return TaskStatus.Success;
    }

    private float DistanceForTarget() => DevelopmentToos.DistanceForTarget(EnemyManager.MainInstance.GetMainPlayer(),transform);

    //1.AI�������ȼ�⹥��ָ��
    //2.����ָ��û�б������ô�ж���Ŀ��ľ��룬��Զ�ͳ���Ŀ���ƶ�
    //3.���ִﰲȫ���룬����ָ�û�б�����
    //4.��ô��AIִ�и���ϡ��ŹֵĶ���
    //5.������ָ�����˳��ڵ�

    /// <summary>
    /// ִ�������ƶ�����
    /// </summary>
    private void FreeMovement()
    {
        switch (_actionIndex)
        {
            case 0://�����ƶ�
                _enemyMovementController.SetAnimatorMovementValue(-1f, 0f);
                break;
            case 1://�����ƶ�
                _enemyMovementController.SetAnimatorMovementValue(1f, 0f);
                break;
            case 2://վ��
                _enemyMovementController.SetAnimatorMovementValue(0f, 0f);
                break;
            case 3://�����
                _enemyMovementController.SetAnimatorMovementValue(-1f, -1f);
                break;
            case 4://�Һ���
                _enemyMovementController.SetAnimatorMovementValue(1f, -1f);
                break;
        }
    }
    /// <summary>
    /// ���¶�������ʱ��
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
    /// ���¶�������
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
