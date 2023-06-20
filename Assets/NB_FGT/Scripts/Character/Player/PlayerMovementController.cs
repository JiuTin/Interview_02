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

        //脚步声
        private float _nextFootTime;
        [SerializeField] private float _slowFootTime;
        [SerializeField] private float _fastFootTime;

        //角色目标朝向
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
                //获得键盘输入的旋转角度  ,
                //添加_mainCamera.eulerAngles.y后，在移动时，旋转镜头时，角色的旋转会加上摄像机的旋转。
                _rotationAngle =
                    Mathf.Atan2(GameInputManager.MainInstance.Movement.x, GameInputManager.MainInstance.Movement.y) *
                    Mathf.Rad2Deg + _mainCamera.eulerAngles.y;
            }
            if (_animator.GetBool(AnimationID.HasInputID) && _animator.AnimationAtTag("Motion"))
            {
                //自身世界坐标下的欧拉角 = (0,1,0)*(自身世界坐标下 绕Y轴的欧拉角 到 新的Y轴的欧拉角 的缓动值)
                transform.eulerAngles = Vector3.up * Mathf.SmoothDampAngle(transform.eulerAngles.y, _rotationAngle,
                    ref _angleVelocity, _rotationSmoothTime);

                //拿到我们要转向的那个方向
                _characterTargetDirection = Quaternion.Euler(0f, _rotationAngle, 0f) * Vector3.forward;
            }
            //角色在调整旋转角度时，可以调整旋转平滑时间，因为原先的平滑时间太短，导致turn动画还没播完，角色的实际方向就到了增量上，再加上动画的增量导致旋转出现偏差。
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
                //设置脚步声
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
        /// 脚步声
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
