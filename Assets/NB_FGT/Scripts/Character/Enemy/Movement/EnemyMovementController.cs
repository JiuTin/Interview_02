using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGG.Tool;
namespace NB_FGT.Movement
{
    public class EnemyMovementController :ChatacterMovementControllerBase
    {
        //1.���˵Ķ�������
        //2.�ƶ���ʱ�򣬿������
        //3.���ƶ�״̬�£��ƶ��������Ĳ�������Ϊ0
        private bool _applyMovement;


        protected override void Start()
        {
            base.Start();
            SetApplyMovement(true);
        }
        protected override void Update()
        {
            base.Update();
            LockTargetDirection();
            DrawDirection();
        }
        private void LockTargetDirection()
        {
            if (_animator.AnimationAtTag("Motion"))
            {
                transform.Look(EnemyManager.MainInstance.GetMainPlayer().position, 500f);
            }
        }
        /// <summary>
        /// ���ö����ƶ�����
        /// </summary>
        /// <param name="horizontal"></param>
        /// <param name="vertical"></param>
        public void SetAnimatorMovementValue(float horizontal,float vertical)
        {
            if (_applyMovement)
            {
                _animator.SetBool(AnimationID.HasInputID, true);
                _animator.SetFloat(AnimationID.LockID, 1f);
                _animator.SetFloat(AnimationID.HorizontalID, horizontal, 0.2f, Time.deltaTime);
                _animator.SetFloat(AnimationID.VerticalID, vertical, 0.2f, Time.deltaTime);
            }
            else
            {
                _animator.SetBool(AnimationID.HasInputID, false);
                _animator.SetFloat(AnimationID.LockID, 0f);
                _animator.SetFloat(AnimationID.HorizontalID, 0f, 0.2f, Time.deltaTime);
                _animator.SetFloat(AnimationID.VerticalID, 0f, 0.2f, Time.deltaTime);
            }
        }

        private void DrawDirection()
        {
            Debug.DrawRay(transform.position + (transform.up * 0.7f), (EnemyManager.MainInstance.GetMainPlayer().position-transform.position), Color.yellow);
        }

        public void SetApplyMovement(bool apply)
        {
            _applyMovement = apply;
        }
        public void EnableCharacterController(bool enable)
        {
            _control.enabled = enable;
        }
    }
}
