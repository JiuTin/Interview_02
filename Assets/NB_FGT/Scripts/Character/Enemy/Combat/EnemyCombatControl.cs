using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGG.Tool;
namespace NB_FGT.Combat
{
    public class EnemyCombatControl : CharacterCombatBase
    {
        
        //AI�Ĺ���ָ������AI������ָ�ɵģ���AI�������Ϊ
        //AI���յ�����ָ���Ҫ�ж������������������Ƿ�������ָ��
        //��Ҳ�ϣ��AIȥ����
        [SerializeField] private bool _attackCommand;



        private void OnEnable()
        {
            GameEventManager.MainInstance.AddEventListening<Transform>("��������", OnEnemyDeadHandler);
        }
        private void OnDisable()
        {
            GameEventManager.MainInstance.RemoveEvent<Transform>("��������", OnEnemyDeadHandler);
        }
        protected void Start()
        {
            _canAttackInput = true;
            _currentEnemy = GameObject.FindGameObjectWithTag("Player").transform;
        }

        //��AI���ܹ���ָ��ʱ���ܵ���ҵĹ���,������ǰ����ָ�


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
        /// ���AI״̬�Ƿ�������ܹ���ָ��
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
        /// ���ù���ָ��
        /// </summary>
        private void ResetAttackCommand()
        {
            _attackCommand = false;
        }
        //��ȡ����ָ��
        public bool GetCombatCommand() => _attackCommand;


        //Event
        public void SetAttackCommand(bool command)
        {
            //�ж��������
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
