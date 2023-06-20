using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NB_FGT.Health;
using GGG.Tool;
public class PlayerHealthController : CharacterHealthBase
{
    protected override void Awake()
    {
        base.Awake();
        _characterHealthInfo = ScriptableObject.Instantiate(_healthInfo);
    }

    protected override void Update()
    {
        base.Update();
        PlayerParryInput();
    }
    protected override void TakeDamage(float damage, bool hasParry = false)
    {
        base.TakeDamage(damage, hasParry);
        GameEventManager.MainInstance.CallEvent("��������ֵUI", _characterHealthInfo);
        if (_characterHealthInfo.CurrentHP <= 0)
        {
            //TODO
            _animator.SetBool(AnimationID.DeadID, true);
        }

    }
    protected override void CharacterHitAction(float damage, string hitName, string parryName)
    {
        if (_animator.AnimationAtTag("Finish")) return;
        if (_animator.GetBool(AnimationID.ParryID) && damage < 30f)
        {
            //���Ÿ񵲶���
            _animator.Play(parryName, 0, 0);
            GamePoolManager.MainInstance.TryGetPoolItem("BlockSound", transform.position, Quaternion.identity);
        }
        else
        { 
            _animator.Play(hitName, 0, 0);
            _fx.Play();
            GamePoolManager.MainInstance.TryGetPoolItem("HitSound",transform.position,Quaternion.identity);
            
            
        }
        //��������������û������Ѫ��   TODO :  ��������ֵ
        TakeDamage(damage,_animator.GetBool(AnimationID.ParryID));
        //if (_characterHealthInfo.CurrentHP <= 0)
        //    Debug.Log("�������");  //TODO  �����������������������Ķ���
    }
    /// <summary>
    /// ��Ҹ�
    /// </summary>
    private void PlayerParryInput()
    {
        if (_animator.AnimationAtTag("Hit") && _animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 0.35f)
            return;
        if (_animator.AnimationAtTag("FinishHit")) return;
        _animator.SetBool(AnimationID.ParryID, GameInputManager.MainInstance.Parry);
    }
}
