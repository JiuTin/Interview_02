using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGG.Tool;
namespace NB_FGT.Health
{
    public class EnemyHealthController : CharacterHealthBase
    {
        protected override void Awake()
        {
            base.Awake();
            _characterHealthInfo = ScriptableObject.Instantiate(_healthInfo);
            EnemyManager.MainInstance.AddEnemyUnit(gameObject);
        }

        protected override void TakeDamage(float damage,bool hasParry=false)
        {
            base.TakeDamage(damage, hasParry);
            if (_characterHealthInfo.CurrentHP <= 0)
            {
                GameEventManager.MainInstance.CallEvent("��������", transform);
                PlayDeadAnimation();
                //transform.gameObject.layer = _godLayer;   _godLayer=10
                _animator.SetBool(AnimationID.DeadID, true);
                EnemyManager.MainInstance.RemoveEnemyUnit(gameObject);
            }
        }
        protected override void PlayDeadAnimation()
        {
            base.PlayDeadAnimation();
        }

        protected override void CharacterHitAction(float damage,string hitName, string parryName)
        {
            //1.���жϽ�ɫ������ֵ��������ֵ������0�͸񵲶�����ֱ������
            //2.����˺�ֵ����30�����贫�������˺�����30����ôĬ������һ���Ʒ���������ô��۳�����������ֵ��������ֵ��
            if (_characterHealthInfo.StrengthFull && damage < 30f)
            {
                if (!_animator.AnimationAtTag("Attack"))
                {
                    //˵�������Ʒ���������ô����Ҫ���и񵲻�������
                    //Debug.Log(0);
                    _animator.Play(parryName, 0, 0);
                    //������Ч
                    GamePoolManager.MainInstance.TryGetPoolItem("BlockSound", transform.position, Quaternion.identity);
                    //����˺�
                    _characterHealthInfo.DamageToStrength(damage);
                    //����ֵ���ʱ��Ҫ֪ͨ��ҿ��Ա�������
                    if (!_characterHealthInfo.StrengthFull)
                    {
                        GameEventManager.MainInstance.CallEvent<bool>("�����", true);
                    }

                }
            }
            else
            {
                //˵��������ֵ��������==0f
                if(_characterHealthInfo.CurrentHP<=20f)
                    GameEventManager.MainInstance.CallEvent<bool>("�����", true);
                //ִ�����˶���
                _animator.Play(hitName, 0, 0);
                //������Ч
                GamePoolManager.MainInstance.TryGetPoolItem("HitSound", transform.position, Quaternion.identity);
                //������Ч
                _fx.Play();
                TakeDamage(damage);
                if (_characterHealthInfo.CurrentHP <= 0f)
                {
                    EnemyManager.MainInstance.RemoveEnemyUnit(this.gameObject);
                }
            }
        }



    }
}
