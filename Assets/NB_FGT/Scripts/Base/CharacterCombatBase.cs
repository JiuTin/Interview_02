using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NB_FGT.ComboData;
using GGG.Tool;

namespace NB_FGT.Combat
{
    public abstract class CharacterCombatBase : MonoBehaviour
    {
        //��Һ�AI���ǵĹ����¼�������ͬ
        //�˺��Ĵ���Ҳ��ͬ
        //������ϼ�Ҳ��
        //��ϼ�����Ϣ����
        protected Animator _animator;
        [SerializeField,Header("��ɫ��ϼ�")]protected CharacterComboSO _baseCombo;//�ṥ��
        [SerializeField, Header("����")] protected CharacterComboSO _finishCombo;  //����

        protected CharacterComboSO _currentCombo;

        //��Ҫһ����ǰ��ϼ��Ķ����������൱����������ʹ����һ��
        //���������������ʱ��
        //�Ƿ��������빥���ź�
        protected int _currentComboIndex;
        protected int _hitIndex;
        protected float _maxColdTime;
        protected bool _canAttackInput;
        protected Transform _currentEnemy;

        //��ֹ����_currentComboIndex����Խ�������(�ڸ��¶�����Ϣʱ���ӳٵ�Խ��)
        //����ʹ���µ��±���������ֹԽ��(���ڸ���Ŀ�Ǵ�-1��ʼ��ÿ�������������ã���ʱû�����������)
        protected int _finishComboIndex;
        protected bool _canFinish;


        //�������
        [SerializeField] protected float _attackRange;


        protected virtual void Awake()
        {
            _animator = GetComponent<Animator>();
            
        }

        protected virtual void Update()
        {
            MatchPosition();
            LookTargetOnAttack();
            
            OnEndAttack();
        }



        #region ����Ŀ��
        private void LookTargetOnAttack()
        {
            if (_currentEnemy == null) return;
            if (_animator.AnimationAtTag("Attack") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
            {
                transform.Look(_currentEnemy.position, 50f);
            }
        }
        #endregion


        #region λ��ͬ��
        protected virtual void MatchPosition()
        {
            if (_currentEnemy == null) return;
            if (!_animator) return;
            //ƥ�乥����λ��(ȷ���Լ��ǲ�����Ҫ)
            if (_animator.AnimationAtTag("Attack"))
            {
                var timer = _animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
                if (timer > 0.35f) return;
                if (DevelopmentToos.DistanceForTarget(_currentEnemy, transform) > 2f) return;
                //RunningMatch();
            }
        }
        protected void RunningMatch(CharacterComboSO combo, int index, float startTime = 0f, float endTime = 0.1f)
        {
            if (!_animator.isMatchingTarget && !_animator.IsInTransition(0))   //��ǰ����ƥ��ͬʱ���ڹ���״̬
            {
                //���Ըĳ���ҵ�λ��
                _animator.MatchTarget(_currentEnemy.position + (-transform.forward * combo.TryGetComboPositionOffset(index)), Quaternion.identity, AvatarTarget.Body,
                    new MatchTargetWeightMask(Vector3.one, 0f), startTime, endTime);
            }
        }
        #endregion

        #region �����¼�
        /// <summary>
        /// �����˺��¼�
        /// </summary>
        protected void ATK()
        {
            TriggerDamage();
            UpdateHitIndex();

            GamePoolManager.MainInstance.TryGetPoolItem("ATKSound", transform.position, Quaternion.identity);
        }
        #endregion

        #region �˺�����

        protected void TriggerDamage()
        {
            //1.ȷ��Ŀ��
            //2.ȷ�����˴��ڿɴ����˺��ľ���ͽǶ�
            //3.�����¼����ģ����ô����˺��������
            if (_currentEnemy == null) return;
            //�������ֱΪ0��ͶӰ�ĳ��ȳ��Ա�ͶӰ���������ȡ�   --�����ﶼ��һ�ˡ�--
            if (Vector3.Dot(transform.forward, DevelopmentToos.DirectionForTarget(transform, _currentEnemy)) < 0.85f) return;
            if (DevelopmentToos.DistanceForTarget(_currentEnemy, transform) > _attackRange) return;
            if (_animator.AnimationAtTag("Attack"))
            {
                //��������˶����ǵ���Ƭ�Σ�
                //���ܴ��ڵ����⣬����ʱ���˺���û����ʱ��_currentEnemy���ó������ĵ��ˣ��п��ܱ�������
                GameEventManager.MainInstance.CallEvent("�����˺�", _currentCombo.TryGetDamage(_currentComboIndex),
                    _currentCombo.TryGetOneHitName(_currentComboIndex, _hitIndex),
                    _currentCombo.TryGetOneParryName(_currentComboIndex, _hitIndex),
                    transform, _currentEnemy);
            }
            else
            {
                //���ڴ�����ɱ
                //������һ�������Ĵ���������ͬһ�������ڼ�ᴥ������˺�
                GameEventManager.MainInstance.CallEvent("�����˺�", _finishCombo.TryGetDamage(_finishComboIndex), _currentEnemy);
            }

        }

        #endregion

        #region ����������Ϣ
        protected virtual void UpdateComboInfo()
        {
            _currentComboIndex++;
            if (_currentComboIndex == _currentCombo.TryGetComboMaxCount())
            {
                _currentComboIndex = 0;
            }
            _maxColdTime = 0f;
            _canAttackInput = true;
        }
        protected void UpdateHitIndex()
        {
            
            //����ǹ���ʱ���ۼ�
            if (_animator.AnimationAtTag("Attack"))
            {
                _hitIndex++;
                
                if (_hitIndex == _currentCombo.TryGetHitOrParryMaxCount(_currentComboIndex))
                    _hitIndex = 0;
            }
            //TODO,����ʱ
        }
        #endregion

        #region ����������Ϣ
        //  
        protected void ResetComboInfo()
        {
            _currentComboIndex = 0;
            _maxColdTime = 0f;
            _hitIndex = 0;
        }

        protected void OnEndAttack()
        {
            if (_animator.AnimationAtTag("Motion") && _canAttackInput)
            {
                ResetComboInfo();
            }
        }
        #endregion

        #region  ��������
      
        protected virtual void CharacterBaseAttackInput() { }
        /// <summary>
        /// ���Ĺ�������
        /// </summary>
        /// <param name="comboData"></param>
        protected void ChangeComboData(CharacterComboSO comboData)
        {
            if (_currentCombo != comboData)
            {
                _currentCombo = comboData;
                ResetComboInfo();
            }
        }

        #endregion

        #region ����ִ��
        protected void ExcuteAction()
        {
            //_currentComboCount += (_currentCombo == _baseCombo) ? 1 : 0;
            //���µ�ǰ������HitIndex����ֵ
            _hitIndex = 0;
            //_currentComboIndex++;
            //if (_currentComboIndex >= _currentCombo.TryGetComboMaxCount())
            //{
            //    //��ǰ�����Ѿ�ִ�е����һ��������
            //    _currentComboIndex = 0;
            //}
            _maxColdTime = _currentCombo.TryGetColdTime(_currentComboIndex);
            _animator.CrossFadeInFixedTime(_currentCombo.TryGetOneComboAction(_currentComboIndex), 0.15555f, 0, 0);
            //ʹ�ü�ʱ�����»����Խ������⡣
            TimeManager.MainInstance.TryGetOneTimer(_maxColdTime, UpdateComboInfo);
            _canAttackInput = false;

        }
        #endregion

    }
}
