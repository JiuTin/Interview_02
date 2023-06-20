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
        //������
        [SerializeField, Header("������")]protected bool _characterIsOnGround;
        [SerializeField]protected float _groundDetectionPositionOffset;  //���ƫ����
        [SerializeField]protected float _detectionRange;                 //��ⷶΧ
        [SerializeField]protected LayerMask _whatlsGround;               //���㼶

        //����
        protected readonly float CharacterGravity = -9.8f;
        protected float _characterVerticalVelocity;// ���½�ɫy����ٶȣ�����Ӧ������������Ծ�߶ȡ�
        protected float _fallOutDeltaTime;
        protected float _fallOutTime = 0.15f;   //��ֹ��ɫ��¥�ݵ�ʱ�����
        protected float _characterVerticalMaxVelocity = 54f;   //��ɫ�ڵ������ֵ��ʱ�򣬲���ҪӦ������
        protected Vector3 _characterVerticalDirection; //��ɫ��Y���ƶ�������Ϊ��ͨ��CC��Move������ʵ�����������԰�_characterVerticalVelocity,Ӧ�õ����������Yֵ����ȥ���¡�
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
        /// ������
        /// </summary>
        /// <returns></returns>
        private bool GroundDetection()
        {
            var detectionPosition = new Vector3(transform.position.x, transform.position.y - _groundDetectionPositionOffset, transform.position.z); //������ĵ�
            return Physics.CheckSphere(detectionPosition, _detectionRange, _whatlsGround, QueryTriggerInteraction.Ignore);
        }
        /// <summary>
        /// ����
        /// </summary>
        private void SetCharacterGravity()
        {
            _characterIsOnGround = GroundDetection();
            if (_characterIsOnGround)
            {
                /*1.��ɫ�ڵ����ϣ���Ҫ����FallOutTime
                 * 2.���ý�ɫ�Ĵ�ֱ�ٶ�
                 * 
                 */
                _fallOutDeltaTime = _fallOutTime;
                if (_characterVerticalVelocity <= 0f)
                {
                    //�̶�-2���ڶ��ε��䣬��ֱ�ٶȴ�-2��ʼ����
                    //�ǹ̶����ڵ���Ҳ��һֱ�ۼơ�
                    _characterVerticalVelocity = -2f;

                }
            }
            else
            {
                //���ڵ���
                if (_fallOutDeltaTime > 0)
                {
                    _fallOutDeltaTime -= Time.deltaTime; //�ȴ�0.15�룬���0.15������������ɫ�ӽϵ͵ĸ߶�����

                }
                else
                {
                    //С��0����ɫ��û��أ��������䶯����
                }
                if (_characterVerticalVelocity < _characterVerticalMaxVelocity && _isEnableGravity)
                {
                    _characterVerticalVelocity += CharacterGravity * Time.deltaTime;
                }
            }
        }
        /// <summary>
        /// ��ɫ��������
        /// </summary>

        private void UpdateCharacterGravity()
        {
            if (!_isEnableGravity) return;
            if (_control.enabled == false) return;
            _characterVerticalDirection.Set(0, _characterVerticalVelocity, 0);
            _control.Move(_characterVerticalDirection * Time.deltaTime);
        }
        /// <summary>
        /// �µ����
        /// </summary>
        private Vector3 StopResetDirection(Vector3 moveDirection)
        {
            //����ɫ�����Ƿ��������ƶ������ý�ɫ�������ٶȹ���ʱ���±�ɵ�����,   --1.53��
            if (Physics.Raycast(transform.position + (transform.up * 0.5f), Vector3.down, out var hit,_control.height*0.85f,_whatlsGround ,QueryTriggerInteraction.Ignore))
            {
                //����ʹ��Vector3.Angle�滻��
                if (Vector3.Dot(Vector3.up, hit.normal) != 0)
                {
                    return moveDirection = Vector3.ProjectOnPlane(moveDirection, hit.normal);
                }
            }
            return moveDirection;
        }

        protected void UpdateCharacterMoveDirection(Vector3 direction)
        {
            //�������ƶ��ķ���
            if (_control.enabled == false) return;
            _moveDirection = StopResetDirection(direction);
            _control.Move(_moveDirection * Time.deltaTime);
        }
        private void OnDrawGizmos()
        {
            var detectionPosition = new Vector3(transform.position.x, transform.position.y - _groundDetectionPositionOffset, transform.position.z);
            Gizmos.DrawWireSphere(detectionPosition, _detectionRange);
        }

        //�¼�ע��
        public void EnableCharacterGravity(bool enable)
        {
            _isEnableGravity = enable;
            _characterVerticalVelocity = (enable) ? -2f : 0f;
        }
    }
}
