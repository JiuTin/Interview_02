using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NB_FGT.ComboData;
using GGG.Tool;

namespace NB_FGT.Combat
{
    public abstract class CharacterCombatBase : MonoBehaviour
    {
        //玩家和AI他们的攻击事件触发相同
        //伤害的触发也相同
        //基础组合技也是
        //组合技的信息更新
        protected Animator _animator;
        [SerializeField,Header("角色组合技")]protected CharacterComboSO _baseCombo;//轻攻击
        [SerializeField, Header("处决")] protected CharacterComboSO _finishCombo;  //处决

        protected CharacterComboSO _currentCombo;

        //需要一个当前组合技的动作索引，相当于现在正在使用哪一招
        //攻击的最大问题间隔时间
        //是否允许输入攻击信号
        protected int _currentComboIndex;
        protected int _hitIndex;
        protected float _maxColdTime;
        protected bool _canAttackInput;
        protected Transform _currentEnemy;

        //防止出现_currentComboIndex出现越界的问题(在更新动作信息时，延迟的越界)
        //可以使用新的下标索引来防止越界(由于该项目是从-1开始且每个动作都会重置，暂时没发现这个错误)
        protected int _finishComboIndex;
        protected bool _canFinish;


        //攻击检测
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



        #region 看着目标
        private void LookTargetOnAttack()
        {
            if (_currentEnemy == null) return;
            if (_animator.AnimationAtTag("Attack") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
            {
                transform.Look(_currentEnemy.position, 50f);
            }
        }
        #endregion


        #region 位置同步
        protected virtual void MatchPosition()
        {
            if (_currentEnemy == null) return;
            if (!_animator) return;
            //匹配攻击的位置(确认自己是不是需要)
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
            if (!_animator.isMatchingTarget && !_animator.IsInTransition(0))   //当前不在匹配同时不在过渡状态
            {
                //可以改成玩家的位置
                _animator.MatchTarget(_currentEnemy.position + (-transform.forward * combo.TryGetComboPositionOffset(index)), Quaternion.identity, AvatarTarget.Body,
                    new MatchTargetWeightMask(Vector3.one, 0f), startTime, endTime);
            }
        }
        #endregion

        #region 攻击事件
        /// <summary>
        /// 触发伤害事件
        /// </summary>
        protected void ATK()
        {
            TriggerDamage();
            UpdateHitIndex();

            GamePoolManager.MainInstance.TryGetPoolItem("ATKSound", transform.position, Quaternion.identity);
        }
        #endregion

        #region 伤害触发

        protected void TriggerDamage()
        {
            //1.确保目标
            //2.确保敌人处于可触发伤害的距离和角度
            //3.呼叫事件中心，调用触发伤害这个函数
            if (_currentEnemy == null) return;
            //点积：垂直为0，投影的长度乘以被投影的向量长度。   --》这里都归一了《--
            if (Vector3.Dot(transform.forward, DevelopmentToos.DirectionForTarget(transform, _currentEnemy)) < 0.85f) return;
            if (DevelopmentToos.DistanceForTarget(_currentEnemy, transform) > _attackRange) return;
            if (_animator.AnimationAtTag("Attack"))
            {
                //这里的受伤动画是单个片段，
                //可能存在的问题，攻击时，伤害还没触发时，_currentEnemy设置成其它的敌人，有可能报空引用
                GameEventManager.MainInstance.CallEvent("触发伤害", _currentCombo.TryGetDamage(_currentComboIndex),
                    _currentCombo.TryGetOneHitName(_currentComboIndex, _hitIndex),
                    _currentCombo.TryGetOneParryName(_currentComboIndex, _hitIndex),
                    transform, _currentEnemy);
            }
            else
            {
                //处于处决或暗杀
                //处决是一个完整的处决动作，同一个动画期间会触发多次伤害
                GameEventManager.MainInstance.CallEvent("生成伤害", _finishCombo.TryGetDamage(_finishComboIndex), _currentEnemy);
            }

        }

        #endregion

        #region 更新连招信息
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
            
            //如果是攻击时，累加
            if (_animator.AnimationAtTag("Attack"))
            {
                _hitIndex++;
                
                if (_hitIndex == _currentCombo.TryGetHitOrParryMaxCount(_currentComboIndex))
                    _hitIndex = 0;
            }
            //TODO,处决时
        }
        #endregion

        #region 重置连招信息
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

        #region  基础攻击
      
        protected virtual void CharacterBaseAttackInput() { }
        /// <summary>
        /// 更改攻击数据
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

        #region 动作执行
        protected void ExcuteAction()
        {
            //_currentComboCount += (_currentCombo == _baseCombo) ? 1 : 0;
            //更新当前动作的HitIndex索引值
            _hitIndex = 0;
            //_currentComboIndex++;
            //if (_currentComboIndex >= _currentCombo.TryGetComboMaxCount())
            //{
            //    //当前动作已经执行到最后一个动作了
            //    _currentComboIndex = 0;
            //}
            _maxColdTime = _currentCombo.TryGetColdTime(_currentComboIndex);
            _animator.CrossFadeInFixedTime(_currentCombo.TryGetOneComboAction(_currentComboIndex), 0.15555f, 0, 0);
            //使用计时器更新会存在越界的问题。
            TimeManager.MainInstance.TryGetOneTimer(_maxColdTime, UpdateComboInfo);
            _canAttackInput = false;

        }
        #endregion

    }
}
