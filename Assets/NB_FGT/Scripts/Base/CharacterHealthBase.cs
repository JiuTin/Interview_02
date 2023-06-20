using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGG.Tool;
using NB_FGT.HealthData;
namespace NB_FGT.Health
{
    public abstract class CharacterHealthBase : MonoBehaviour,IHealth
    {
        //共同都有受伤函数
        //同样都有处决对应函数
        //格挡
        //生命信息
        protected Transform _currentAttacker;  //当前的攻击者
        protected Animator _animator;
        [SerializeField, Header("生命值信息")] protected CharacterHealthInfo _healthInfo;
        protected CharacterHealthInfo _characterHealthInfo;

        //特效FX
        protected IFX _fx;

        protected virtual void Awake()
        {
            _animator = GetComponent<Animator>();
            _fx = GetComponentInChildren<IFX>();
        }
        protected virtual void Start()
        {
            _characterHealthInfo.InitCharacterHealthInfo();
            
        }
        protected virtual void OnEnable()
        {
            GameEventManager.MainInstance.AddEventListening<float, string, string, Transform, Transform>("触发伤害", OnCharacterHitHandler);
            GameEventManager.MainInstance.AddEventListening<string, Transform, Transform>("触发处决", OnCharacterFinishHandler);
            GameEventManager.MainInstance.AddEventListening<float, Transform>("生成伤害",TriggerDamageEventHandler);
        }

        protected virtual void OnDisable()
        {
            GameEventManager.MainInstance.RemoveEvent<float, string, string, Transform, Transform>("触发伤害", OnCharacterHitHandler);
            GameEventManager.MainInstance.RemoveEvent<string, Transform, Transform>("触发处决", OnCharacterFinishHandler);
            GameEventManager.MainInstance.RemoveEvent<float, Transform>("生成伤害", TriggerDamageEventHandler);
        }
        protected virtual void Update()
        {
            OnHitLookTarget();
        }

        /// <summary>
        /// 角色的受伤行为
        /// </summary>
        /// <param name="hitName">受伤动画</param>
        /// <param name="parrName">格挡动画</param>
        protected virtual void CharacterHitAction(float damage,string hitName, string parryName)
        { 
            
        }

        protected virtual void PlayDeadAnimation()
        {
            if (!_animator.AnimationAtTag("FinishHit"))
            {
                _animator.Play("Dead", 0, 0);   //播放死亡动画
            }
        }
        protected virtual void TakeDamage(float damage,bool hasParry=false)
        {
            //TODO 扣除生命值
            _characterHealthInfo.Damage(damage, hasParry);
        }
        /// <summary>
        /// 设置当前的攻击者
        /// </summary>
        /// <param name="attacker">攻击者</param>
        private void SetAttacker(Transform attacker)
        {
            if (_currentAttacker == null || _currentAttacker != attacker)
            {
                _currentAttacker = attacker;
            }
        }
        /// <summary>
        /// 受伤看向攻击者
        /// </summary>
        private void OnHitLookTarget()
        {
            if (_currentAttacker == null) return;
            if (_animator.AnimationAtTag("Hit") || _animator.AnimationAtTag("Parry") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.5f)
            {
                transform.Look(_currentAttacker.position, 50f);
            }
        }

        public bool OnDie() => _characterHealthInfo.IsDie;


        //事件=======================================================
        /// <summary>
        /// 受伤事件
        /// </summary>
        /// <param name="damage">伤害</param>
        /// <param name="hitName">受伤动画</param>
        /// <param name="parryName">格挡动画</param>
        /// <param name="attack">攻击者</param>
        /// <param name="self">受伤者</param>
        private void OnCharacterHitHandler(float damage, string hitName, string parryName, Transform attacker, Transform self)
        {
            if (self != transform) return;
            SetAttacker(attacker);
            CharacterHitAction(damage,hitName, parryName);
            //TakeDamage(damage);
        }
        /// <summary>
        /// 处决事件
        /// </summary>
        /// <param name="hitName">处决动画</param>
        /// <param name="attacker">攻击者</param>
        /// <param name="self">被攻击者</param>
        private void OnCharacterFinishHandler(string hitName, Transform attacker, Transform self)
        {
            if (self != transform) return;
            SetAttacker(attacker);
            
            _animator.Play(hitName);
        }

        private void TriggerDamageEventHandler(float damage,Transform self)
        {
            if (self != transform) return;
            TakeDamage(damage);
            //触发音效
            GamePoolManager.MainInstance.TryGetPoolItem("HitSound",transform.position,Quaternion.identity);
            if (_characterHealthInfo.CurrentHP <= 0f)
            {
                EnemyManager.MainInstance.RemoveEnemyUnit(this.gameObject);
            }
        }



    }

}
