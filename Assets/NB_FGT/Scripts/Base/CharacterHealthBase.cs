using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGG.Tool;
using NB_FGT.HealthData;
namespace NB_FGT.Health
{
    public abstract class CharacterHealthBase : MonoBehaviour,IHealth
    {
        //��ͬ�������˺���
        //ͬ�����д�����Ӧ����
        //��
        //������Ϣ
        protected Transform _currentAttacker;  //��ǰ�Ĺ�����
        protected Animator _animator;
        [SerializeField, Header("����ֵ��Ϣ")] protected CharacterHealthInfo _healthInfo;
        protected CharacterHealthInfo _characterHealthInfo;

        //��ЧFX
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
            GameEventManager.MainInstance.AddEventListening<float, string, string, Transform, Transform>("�����˺�", OnCharacterHitHandler);
            GameEventManager.MainInstance.AddEventListening<string, Transform, Transform>("��������", OnCharacterFinishHandler);
            GameEventManager.MainInstance.AddEventListening<float, Transform>("�����˺�",TriggerDamageEventHandler);
        }

        protected virtual void OnDisable()
        {
            GameEventManager.MainInstance.RemoveEvent<float, string, string, Transform, Transform>("�����˺�", OnCharacterHitHandler);
            GameEventManager.MainInstance.RemoveEvent<string, Transform, Transform>("��������", OnCharacterFinishHandler);
            GameEventManager.MainInstance.RemoveEvent<float, Transform>("�����˺�", TriggerDamageEventHandler);
        }
        protected virtual void Update()
        {
            OnHitLookTarget();
        }

        /// <summary>
        /// ��ɫ��������Ϊ
        /// </summary>
        /// <param name="hitName">���˶���</param>
        /// <param name="parrName">�񵲶���</param>
        protected virtual void CharacterHitAction(float damage,string hitName, string parryName)
        { 
            
        }

        protected virtual void PlayDeadAnimation()
        {
            if (!_animator.AnimationAtTag("FinishHit"))
            {
                _animator.Play("Dead", 0, 0);   //������������
            }
        }
        protected virtual void TakeDamage(float damage,bool hasParry=false)
        {
            //TODO �۳�����ֵ
            _characterHealthInfo.Damage(damage, hasParry);
        }
        /// <summary>
        /// ���õ�ǰ�Ĺ�����
        /// </summary>
        /// <param name="attacker">������</param>
        private void SetAttacker(Transform attacker)
        {
            if (_currentAttacker == null || _currentAttacker != attacker)
            {
                _currentAttacker = attacker;
            }
        }
        /// <summary>
        /// ���˿��򹥻���
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


        //�¼�=======================================================
        /// <summary>
        /// �����¼�
        /// </summary>
        /// <param name="damage">�˺�</param>
        /// <param name="hitName">���˶���</param>
        /// <param name="parryName">�񵲶���</param>
        /// <param name="attack">������</param>
        /// <param name="self">������</param>
        private void OnCharacterHitHandler(float damage, string hitName, string parryName, Transform attacker, Transform self)
        {
            if (self != transform) return;
            SetAttacker(attacker);
            CharacterHitAction(damage,hitName, parryName);
            //TakeDamage(damage);
        }
        /// <summary>
        /// �����¼�
        /// </summary>
        /// <param name="hitName">��������</param>
        /// <param name="attacker">������</param>
        /// <param name="self">��������</param>
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
            //������Ч
            GamePoolManager.MainInstance.TryGetPoolItem("HitSound",transform.position,Quaternion.identity);
            if (_characterHealthInfo.CurrentHP <= 0f)
            {
                EnemyManager.MainInstance.RemoveEnemyUnit(this.gameObject);
            }
        }



    }

}
