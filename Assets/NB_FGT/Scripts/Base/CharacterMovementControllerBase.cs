using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NB_FGT.Movement
{
    [RequireComponent(typeof(CharacterController))]
    public abstract class ChatacterMovementControllerBase : MonoBehaviour
    {
        protected CharacterController _control;
        protected Animator _animator;
        protected Vector3 _moveDirection;
        //地面检测
        [SerializeField, Header("地面检测")]protected bool _characterIsOnGround;
        [SerializeField]protected float _groundDetectionPositionOffset;  //检测偏移量
        [SerializeField]protected float _detectionRange;                 //检测范围
        [SerializeField]protected LayerMask _whatlsGround;               //检测层级

        //重力
        protected readonly float CharacterGravity = -9.8f;
        protected float _characterVerticalVelocity;// 更新角色y轴的速度，可以应用与重力和跳跃高度。
        protected float _fallOutDeltaTime;
        protected float _fallOutTime = 0.15f;   //防止角色下楼梯的时候鬼畜。
        protected float _characterVerticalMaxVelocity = 54f;   //角色在低于这个值的时候，才需要应用重力
        protected Vector3 _characterVerticalDirection; //角色的Y轴移动方向，因为是通过CC的Move函数来实现重力，所以把_characterVerticalVelocity,应用到这个向量的Y值里面去更新。
        protected bool _isEnableGravity;

        protected virtual void Awake()
        {
            _control = GetComponent<CharacterController>();
            _animator = GetComponent<Animator>();
            
        }
        private void OnEnable()
        {
            GameEventManager.MainInstance.AddEventListening<bool>("EnableCharacterGravity", EnableCharacterGravity);
        }
        private void OnDisable()
        {
            GameEventManager.MainInstance.RemoveEvent<bool>("EnableCharacterGravity", EnableCharacterGravity);
        }
        protected virtual void Start()
        {
            _fallOutDeltaTime = _fallOutTime;
            _isEnableGravity = true;
        }
        protected virtual void Update()
        {
            SetCharacterGravity();
            UpdateCharacterGravity();
        }
        protected virtual void OnAnimatorMove()
        {
            _animator.ApplyBuiltinRootMotion();
            UpdateCharacterMoveDirection(_animator.deltaPosition);
        }
        /// <summary>
        /// 地面检测
        /// </summary>
        /// <returns></returns>
        private bool GroundDetection()
        {
            var detectionPosition = new Vector3(transform.position.x, transform.position.y - _groundDetectionPositionOffset, transform.position.z); //检测中心点
            return Physics.CheckSphere(detectionPosition, _detectionRange, _whatlsGround, QueryTriggerInteraction.Ignore);
        }
        /// <summary>
        /// 重力
        /// </summary>
        private void SetCharacterGravity()
        {
            _characterIsOnGround = GroundDetection();
            if (_characterIsOnGround)
            {
                /*1.角色在地面上，需要重置FallOutTime
                 * 2.重置角色的垂直速度
                 * 
                 */
                _fallOutDeltaTime = _fallOutTime;
                if (_characterVerticalVelocity <= 0f)
                {
                    //固定-2，第二次跌落，垂直速度从-2开始计算
                    //非固定，在地面也会一直累计。
                    _characterVerticalVelocity = -2f;

                }
            }
            else
            {
                //不在地面
                if (_fallOutDeltaTime > 0)
                {
                    _fallOutDeltaTime -= Time.deltaTime; //等待0.15秒，这个0.15秒用来帮助角色从较低的高度下落

                }
                else
                {
                    //小于0，角色还没落地，播放下落动画。
                }
                if (_characterVerticalVelocity < _characterVerticalMaxVelocity && _isEnableGravity)
                {
                    _characterVerticalVelocity += CharacterGravity * Time.deltaTime;
                }
            }
        }
        /// <summary>
        /// 角色更新重力
        /// </summary>

        private void UpdateCharacterGravity()
        {
            if (!_isEnableGravity) return;
            if (_control.enabled == false) return;
            _characterVerticalDirection.Set(0, _characterVerticalVelocity, 0);
            _control.Move(_characterVerticalDirection * Time.deltaTime);
        }
        /// <summary>
        /// 坡道检测
        /// </summary>
        private Vector3 StopResetDirection(Vector3 moveDirection)
        {
            //检测角色现在是否在坡上移动，放置角色下下坡速度过快时导致变成弹力球,   --1.53米
            if (Physics.Raycast(transform.position + (transform.up * 0.5f), Vector3.down, out var hit,_control.height*0.85f,_whatlsGround ,QueryTriggerInteraction.Ignore))
            {
                //可以使用Vector3.Angle替换。
                if (Vector3.Dot(Vector3.up, hit.normal) != 0)
                {
                    return moveDirection = Vector3.ProjectOnPlane(moveDirection, hit.normal);
                }
            }
            return moveDirection;
        }

        protected void UpdateCharacterMoveDirection(Vector3 direction)
        {
            //沿着坡移动的方向
            if (_control.enabled == false) return;
            _moveDirection = StopResetDirection(direction);
            _control.Move(_moveDirection * Time.deltaTime);
        }
        private void OnDrawGizmos()
        {
            var detectionPosition = new Vector3(transform.position.x, transform.position.y - _groundDetectionPositionOffset, transform.position.z);
            Gizmos.DrawWireSphere(detectionPosition, _detectionRange);
        }

        //事件注册
        public void EnableCharacterGravity(bool enable)
        {
            _isEnableGravity = enable;
            _characterVerticalVelocity = (enable) ? -2f : 0f;
        }
    }
}
