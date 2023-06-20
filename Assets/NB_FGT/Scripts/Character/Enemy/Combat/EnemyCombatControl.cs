using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGG.Tool;
namespace NB_FGT.Combat
{
    public class EnemyCombatControl : CharacterCombatBase
    {
        
        //AI的攻击指令是由AI管理器指派的，非AI自身的行为
        //AI在收到攻击指令，需要判断自身的情况，来决定是否接受这个指令
        //玩家不希望AI去接受
        [SerializeField] private bool _attackCommand;



        private void OnEnable()
        {
            GameEventManager.MainInstance.AddEventListening<Transform>("敌人死亡", OnEnemyDeadHandler);
        }
        private void OnDisable()
        {
            GameEventManager.MainInstance.RemoveEvent<Transform>("敌人死亡", OnEnemyDeadHandler);
        }
        protected void Start()
        {
            _canAttackInput = true;
            _currentEnemy = GameObject.FindGameObjectWithTag("Player").transform;
        }

        //当AI接受攻击指令时，受到玩家的攻击,放弃当前攻击指令。


        public void AIBaseAttackInput()
        {
            if (!_canAttackInput) return;
            ChangeComboData(_baseCombo);
            ExcuteAction();
        }

        protected override void UpdateComboInfo()
        {
            _currentComboIndex++;
            if (_currentComboIndex >= _currentCombo.TryGetComboMaxCount())
            {
                _currentComboIndex = 0;
                ResetAttackCommand();
            }
            _maxColdTime = 0f;
            _canAttackInput = true;
        }
        /// <summary>
        /// 检测AI状态是否允许接受攻击指令
        /// </summary>
        /// <returns></returns>
        private bool CheckAIState()
        {
            if (_animator.AnimationAtTag("Hit")) return false;
            if (_animator.AnimationAtTag("Parry")) return false;
            if (_animator.AnimationAtTag("Attack")) return false;
            if (_animator.AnimationAtTag("FinishHit")) return false;
            if (_attackCommand) return false;
            return true;
        }

        /// <summary>
        /// 重置攻击指令
        /// </summary>
        private void ResetAttackCommand()
        {
            _attackCommand = false;
        }
        //获取攻击指令
        public bool GetCombatCommand() => _attackCommand;


        //Event
        public void SetAttackCommand(bool command)
        {
            //判断自身情况
            if (!CheckAIState())
            {
                ResetAttackCommand();
                return;
            }
            _attackCommand = true;
        }

        public void StopAllAction()
        {
            if (_attackCommand)
                ResetAttackCommand();
            if (_animator.AnimationAtTag("Attack"))
            {
                _animator.Play("Lucy_Idle", 0, 0);
            }
        }

        private void OnEnemyDeadHandler(Transform enemy)
        {
            if (enemy == transform)
            {
                ResetAttackCommand();
                ResetComboInfo();
                _canAttackInput = true;
            }
        }
    }

}
