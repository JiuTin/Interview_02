using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGG.Tool;
namespace NB_FGT.Movement
{ 
    public class PlayerMovementController: ChatacterMovementControllerBase
    {
        private float _rotationAngle;
        private float _angleVelocity = 0f;
        [SerializeField] private float _rotationSmoothTime;

        private Transform _mainCamera;

        //�Ų���
        private float _nextFootTime;
        [SerializeField] private float _slowFootTime;
        [SerializeField] private float _fastFootTime;

        //��ɫĿ�곯��
        private Vector3 _characterTargetDirection;
        protected override void Awake()
        {
            base.Awake();
            _mainCamera = Camera.main.transform;
        }
        private void LateUpdate()
        {
            UpdateAnimation();
            CharacterRotationControl();
        }
        
        private void CharacterRotationControl()
        {
            if (!_characterIsOnGround) return;

            if (_animator.GetBool(AnimationID.HasInputID))
            {
                //��ü����������ת�Ƕ�  ,
                //���_mainCamera.eulerAngles.y�����ƶ�ʱ����ת��ͷʱ����ɫ����ת��������������ת��
                _rotationAngle =
                    Mathf.Atan2(GameInputManager.MainInstance.Movement.x, GameInputManager.MainInstance.Movement.y) *
                    Mathf.Rad2Deg + _mainCamera.eulerAngles.y;
            }
            if (_animator.GetBool(AnimationID.HasInputID) && _animator.AnimationAtTag("Motion"))
            {
                //�������������µ�ŷ���� = (0,1,0)*(�������������� ��Y���ŷ���� �� �µ�Y���ŷ���� �Ļ���ֵ)
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, _rotationAngle,
                    ref _angleVelocity, _rotationSmoothTime);

                //�õ�����Ҫת����Ǹ�����
                _characterTargetDirection = Quaternion.Euler(0f, _rotationAngle, 0f) * Vector3.forward;
            }
            //��ɫ�ڵ�����ת�Ƕ�ʱ�����Ե�����תƽ��ʱ�䣬��Ϊԭ�ȵ�ƽ��ʱ��̫�̣�����turn������û���꣬��ɫ��ʵ�ʷ���͵��������ϣ��ټ��϶���������������ת����ƫ�
            _animator.SetFloat(AnimationID.DeltaAngleID, DevelopmentToos.GetDeltaAngle(transform, _characterTargetDirection));
        }
        private void UpdateAnimation()
        {
            if (!_characterIsOnGround) return;
            _animator.SetBool(AnimationID.HasInputID, GameInputManager.MainInstance.Movement != Vector2.zero);
            if (_animator.GetBool(AnimationID.HasInputID))
            {
                if (GameInputManager.MainInstance.Run)
                { 
                    _animator.SetBool(AnimationID.RunID,true);
                }
                _animator.SetFloat(AnimationID.MovementID, ((_animator.GetBool(AnimationID.RunID)) ? 2f : GameInputManager.MainInstance.Movement.sqrMagnitude), 0.25f, Time.deltaTime);
                //���ýŲ���
                SetCharacterFootSound();
            }
            else
            {
                _animator.SetFloat(AnimationID.MovementID, 0f, 0.25f, Time.deltaTime);

                if (_animator.GetFloat(AnimationID.MovementID)< 0.2f)
                {
                    _animator.SetBool(AnimationID.RunID,false);
                }
            }
        }
        /// <summary>
        /// �Ų���
        /// </summary>
        private void SetCharacterFootSound()
        {
            if (_characterIsOnGround && _animator.GetFloat(AnimationID.MovementID) > 0.5f && _animator.AnimationAtTag("Motion"))
            {
                _nextFootTime -= Time.deltaTime;
                if (_nextFootTime < 0f)
                {
                    PlayChatacterFootSound();
                }
            }
            else
            {
                _nextFootTime = 0f;
            }
        }
        private void PlayChatacterFootSound()
        {
            GamePoolManager.MainInstance.TryGetPoolItem("FootSound", transform.position, Quaternion.identity);
            _nextFootTime = (_animator.GetFloat(AnimationID.MovementID) > 1.1f) ? _fastFootTime : _slowFootTime;
        }
    }

}
