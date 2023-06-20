using NB_FGT.ComboData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGG.Tool;
namespace NB_FGT.Combat
{ 
    public class PlayerComboController : CharacterCombatBase
    {
        //1.���ȣ���Ҫ������ϼ�����ͨ��ϼ����ػ���ϼ�
        //2.��ǰ��ϼ�
        //private Animator _animator;
        //[SerializeField, Header("ȭ��")] private CharacterComboSO _baseCombo;
        [SerializeField, Header("����")] private CharacterComboSO _changeHeavyCombo;  //���ṥ���ı���
       
        [SerializeField, Header("��ɱ")] private CharacterComboSO _assassinationCombo;  //��ɱ

        
        
        //private Transform _cameraGameObject;   ���ڵ�һ�ּ��
       
       
        
        


        //�������
        private Vector3 _detectionDirection;
        [SerializeField, Header("�������")] private float _detectionRange;
        [SerializeField] private float _detectionDistance;
        
        [SerializeField] private LayerMask _enemyLayer;
        private Collider[] _units;

        private bool _activeEnemyDetection;    //���˼��

        protected override void Awake()
        {
            base.Awake();
            //if (Camera.main != null) _cameraGameObject = Camera.main.transform;
        }
        private void Start()
        {
            _canAttackInput = true;
            //_currentCombo = _baseCombo;
            _canFinish = false;
            _currentComboIndex = 0;
            _activeEnemyDetection = true;
        }
        private void OnEnable()
        {
            GameEventManager.MainInstance.AddEventListening<bool>("�����", EnableFinishEventHandler);
            GameEventManager.MainInstance.AddEventListening<Transform>("��������", CheckAndRemoveEnemyHandler);
        }
        private void OnDisable()
        {
            GameEventManager.MainInstance.RemoveEvent<bool>("�����", EnableFinishEventHandler);
            GameEventManager.MainInstance.RemoveEvent<Transform>("��������", CheckAndRemoveEnemyHandler);
        }
        protected override void Update()
        {
            base.Update();
            //UpdateDetectionDirection();
            GetOneUnit();
            // ClearEnemy();
            CharacterBaseAttackInput();
            //���⹥��
            //MatchPosition();
            CharacterFinishAttackInput();
            CharacterAssassinationInput();
            ClearEnemy();
            CheckEnemyIsDie();
        }
        private void FixedUpdate()
        {
            //DetectionTarget();   
            GetNearUnit();
            
        }

        protected override void MatchPosition()
        {
            base.MatchPosition();
            if (_animator.AnimationAtTag("Finish") && _currentEnemy!=null) //��ǰ����ƥ��
            {
                transform.rotation = Quaternion.LookRotation(-_currentEnemy.forward);
                RunningMatch(_finishCombo, _finishComboIndex);
            }
            else if (_animator.AnimationAtTag("Assassination"))
            {
                transform.rotation = Quaternion.LookRotation(_currentEnemy.forward);
                RunningMatch(_assassinationCombo, _finishComboIndex);
            }
        }

        #region ���
        
        //private void DetectionTarget()
        //{
        //    if (Physics.SphereCast(transform.position+(transform.up*0.7f),_detectionRange,_detectionDirection,out var hit,_detectionDistance,1<<9,QueryTriggerInteraction.Ignore))
        //    {
        //        //���������˵����⵽��
        //        _currentEnemy = hit.collider.transform;
        //    }
        //}
        //private void UpdateDetectionDirection()
        //{
        //    _detectionDirection = (_cameraGameObject.forward * GameInputManager.MainInstance.Movement.y) +
        //        (_cameraGameObject.right * GameInputManager.MainInstance.Movement.x);
        //    _detectionDirection.Set(_detectionDirection.x, 0f, _detectionDirection.z);
        //    _detectionDirection=_detectionDirection.normalized;
        //}
        //private void OnDrawGizmos()
        //{
        //    Gizmos.DrawWireSphere(transform.position + (transform.up * 0.7f) + _detectionDirection * _detectionDistance, _detectionRange);
        //}
        #endregion

        #region ��Χ���
        //1.�����Ϊ���ģ�ȡ�Զ����һ���뾶԰�ķ�Χ�ڻ�ȡ���еĵ���
        //2.�ڵ�ǰ���û��Ŀ���ʱ��ȡ�������������һ������Ϊ��ǰĿ��
        //3.��ѡ��
        //      (1)��ǰ��Ŀ�꣬���ٸ���Ŀ�ֱ꣬����ǰĿ����ʧ�����̫Զ
        private void GetNearUnit()
        {
            if (!_activeEnemyDetection) return;
            if (_currentEnemy != null && DevelopmentToos.DistanceForTarget(_currentEnemy,transform)<3f) return;     //�����������������������, ���ʱ�����
            _units=Physics.OverlapSphere(transform.position + (transform.up * 0.7f),_detectionRange,_enemyLayer,QueryTriggerInteraction.Ignore);
        }


        private void GetOneUnit()
        {
            if (_units.Length == 0) return;
            //if (_animator.GetFloat(AnimationID.MovementID) > 0.7f) return;
            if (_currentEnemy != null && DevelopmentToos.DistanceForTarget(_currentEnemy, transform) < 3f) return;
            //if (!_animator.AnimationAtTag("Attack")) return;
            if (_animator.AnimationAtTag("Finish")) return;
            if(!_activeEnemyDetection) return;
            Transform temp_Enemy = null;
            var distance = Mathf.Infinity;
            foreach (var e in _units)
            {
                //1.����ȥ���������ĵ��ˣ����Ƚϣ�ѡȡ���������ĵ�һ��������Ϊ��ǰ��Ŀ��
                var dis = DevelopmentToos.DistanceForTarget(e.transform, transform);
                if (dis < distance)
                {
                    temp_Enemy = e.transform;
                    distance = dis;
                }
            }
            _currentEnemy = temp_Enemy != null ? temp_Enemy : _currentEnemy;
            //����ѡ����˺����ô�����
            _canFinish = false;
        }
        //��յ���
        private void ClearEnemy()
        {
            if (_currentEnemy == null) return;
            if (_animator.GetFloat(AnimationID.MovementID) > 0.7f && DevelopmentToos.DistanceForTarget(_currentEnemy,transform)>3f)
            {
                _currentEnemy = null;
                _canFinish = false;
            }
        }

        #endregion



        #region ��ɫ�Ļ�������

        /// <summary>
        /// �Ƿ���Թ���
        /// </summary>
        /// <returns></returns>
        private bool CanBaseAttackInput()
        {
            //_canAttackInput == false�����ܹ���
            //��ɫ�ڰ��ᣬ���ܹ���
            //��ɫ�ڴ��������ܹ���
            //��ɫ�ڸ񵲣����ܹ���

            if (!_canAttackInput) return false;
            if (_animator.AnimationAtTag("Hit")) return false;
            if (_animator.AnimationAtTag("Parry")) return false;
            if (_animator.AnimationAtTag("Finish")) return false;
            return true;

        }

        #region ��������
        protected override void CharacterBaseAttackInput()
        {
            if (!CanBaseAttackInput()) return;
            if (GameInputManager.MainInstance.LAttack)
            {
                //1.�жϵ�ǰ����ϼ��Ƿ�Ϊ�ջ��߲�Ϊ������ϼ�(���ڱ���֮��Ҫ�ص����������������������Ҫ����Ϊ������ϼ�)
                if (_currentCombo == null || _currentCombo != _baseCombo)
                {
                    ChangeComboData(_baseCombo);
                }
                ExcuteAction();
            }
            else if (GameInputManager.MainInstance.RAttack)
            {
                //�жϵ�ǰCombo�ǲ��Ǳ���Combo
                ChangeComboData(_changeHeavyCombo);
                ExcuteAction();          
            }

        }
        #endregion

       
      

        
        #endregion


        #region ����(���⹥��)

        /// <summary>
        /// �Ƿ�������⹥��
        /// </summary>
        private bool CanSpecialAttack()
        {
            if (_animator.AnimationAtTag("Finish")) return false;
            if (_currentEnemy == null) return false;
            if (!_canFinish) return false;
            return true;
        }

        private void CharacterFinishAttackInput()
        {
            if (!CanSpecialAttack()) return;
            if (GameInputManager.MainInstance.Grab)
            {
                //1.���Ŵ�������
                _finishComboIndex = Random.Range(0, _finishCombo.TryGetComboMaxCount());
                _animator.Play(_finishCombo.TryGetOneComboAction(_finishComboIndex));
                //2.�����¼����ģ����õ���ע��Ĵ����¼�
                GameEventManager.MainInstance.CallEvent("��������", _finishCombo.TryGetOneHitName(_finishComboIndex, 0), transform, _currentEnemy);
                TimeManager.MainInstance.TryGetOneTimer(0.5f, UpdateComboInfo);
                EnemyManager.MainInstance.StopAllActiveUnit();
                //���������
                GameEventManager.MainInstance.CallEvent("�������Ŀ��", _currentEnemy, _animator.GetCurrentAnimatorStateInfo(0).length + 1.5f);


                //������˼��
                _activeEnemyDetection = true;
                ResetComboInfo();
            }
        }
        #endregion
        #region  ��ɱ
        private bool CanAssassination()
        {
            //1.û��Ŀ�꣬������
            if (_currentEnemy == null) return false;
            //2.����̫Զ��������
            if (DevelopmentToos.DistanceForTarget(_currentEnemy, transform) > 2) return false;
            //3.�Ƕ�̫�󣬲�����
            if (Vector3.Angle(transform.forward, _currentEnemy.forward) > 30) return false;
            //4.���ڰ�ɱ��������
            if (_animator.AnimationAtTag("Assassination")) return false;
             
            return true;
        }

        private void CheckEnemyIsDie()
        { 
            if(_currentEnemy == null) return;
            if (_currentEnemy.TryGetComponent(out IHealth health))
            {
                //�����ҵĶ���ִ������ˣ�ͬʱ�������Ѿ������ˣ����Ҷ������ڹ�����
                //���������û��ִ����ϣ�Ȼ������ĳЩ����������Ҫȥ�жϻ���������ǰĿ��
                //��ô�����ڵ���������ֱ�Ӿ��Ƴ���ǰĿ�꣬��ô��Ҿ�û�취��ȡ��ǰĿ��
                //�ͻᱨ�����á�
                if (_animator.AnimationAtTag("Motion") && health.OnDie() && !_animator.IsInTransition(0))
                {
                    Debug.Log("����Ѫ��");
                    _currentEnemy = null;
                }
            }
        }
        private void CharacterAssassinationInput()
        {
            if (!CanAssassination()) return;
            if (GameInputManager.MainInstance.TakeOut)
            {
                //1.ִ�а�ɱ
                //2.���е��˵İ�ɱ�¼�
                _finishComboIndex = Random.Range(0, _assassinationCombo.TryGetComboMaxCount());
                _animator.Play(_assassinationCombo.TryGetOneComboAction(_finishComboIndex),0,0);
                GameEventManager.MainInstance.CallEvent("��������", _assassinationCombo.TryGetOneHitName(_finishComboIndex, 0), transform, _currentEnemy);
                ResetComboInfo();
            }
        }
        #endregion

        //�¼�
        private void EnableFinishEventHandler(bool apply)
        {
            if (_canFinish) return;
            _canFinish = apply;
            //ȡ�����˼��
            _activeEnemyDetection = false;
        }

        private void CheckAndRemoveEnemyHandler(Transform enemy)
        {
            if (enemy == _currentEnemy)
            {
                _canFinish = false;
                _currentEnemy = null;
            }
        }
    }

}
