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
                GameEventManager.MainInstance.CallEvent("敌人死亡", transform);
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
            //1.先判断角色的躯干值或者体力值，大于0就格挡而不是直接受伤
            //2.如果伤害值大于30（假设传进来的伤害大于30，那么默认这是一个破防攻击，那么会扣除大量的体力值或者躯干值）
            if (_characterHealthInfo.StrengthFull && damage < 30f)
            {
                if (!_animator.AnimationAtTag("Attack"))
                {
                    //说明不是破防动作，那么我们要进行格挡或者闪避
                    //Debug.Log(0);
                    _animator.Play(parryName, 0, 0);
                    //播放音效
                    GamePoolManager.MainInstance.TryGetPoolItem("BlockSound", transform.position, Quaternion.identity);
                    //造成伤害
                    _characterHealthInfo.DamageToStrength(damage);
                    //体力值清空时，要通知玩家可以被处决了
                    if (!_characterHealthInfo.StrengthFull)
                    {
                        GameEventManager.MainInstance.CallEvent<bool>("激活处决", true);
                    }

                }
            }
            else
            {
                //说明，体力值不够或者==0f
                if(_characterHealthInfo.CurrentHP<=20f)
                    GameEventManager.MainInstance.CallEvent<bool>("激活处决", true);
                //执行受伤动画
                _animator.Play(hitName, 0, 0);
                //播放音效
                GamePoolManager.MainInstance.TryGetPoolItem("HitSound", transform.position, Quaternion.identity);
                //播放特效
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
