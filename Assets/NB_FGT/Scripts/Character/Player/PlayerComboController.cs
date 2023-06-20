using NB_FGT.ComboData;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGG.Tool;
namespace NB_FGT.Combat
{ 
    public class PlayerComboController : CharacterCombatBase
    {
        //1.首先，需要传入组合技，普通组合技，重击组合技
        //2.当前组合技
        //private Animator _animator;
        //[SerializeField, Header("拳击")] private CharacterComboSO _baseCombo;
        [SerializeField, Header("踢腿")] private CharacterComboSO _changeHeavyCombo;  //重轻攻击的变招
       
        [SerializeField, Header("暗杀")] private CharacterComboSO _assassinationCombo;  //暗杀

        
        
        //private Transform _cameraGameObject;   用于第一种检测
       
       
        
        


        //攻击检测
        private Vector3 _detectionDirection;
        [SerializeField, Header("攻击检测")] private float _detectionRange;
        [SerializeField] private float _detectionDistance;
        
        [SerializeField] private LayerMask _enemyLayer;
        private Collider[] _units;

        private bool _activeEnemyDetection;    //敌人检测

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
            GameEventManager.MainInstance.AddEventListening<bool>("激活处决", EnableFinishEventHandler);
            GameEventManager.MainInstance.AddEventListening<Transform>("敌人死亡", CheckAndRemoveEnemyHandler);
        }
        private void OnDisable()
        {
            GameEventManager.MainInstance.RemoveEvent<bool>("激活处决", EnableFinishEventHandler);
            GameEventManager.MainInstance.RemoveEvent<Transform>("敌人死亡", CheckAndRemoveEnemyHandler);
        }
        protected override void Update()
        {
            base.Update();
            //UpdateDetectionDirection();
            GetOneUnit();
            // ClearEnemy();
            CharacterBaseAttackInput();
            //特殊攻击
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
            if (_animator.AnimationAtTag("Finish") && _currentEnemy!=null) //当前不在匹配
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

        #region 检测
        
        //private void DetectionTarget()
        //{
        //    if (Physics.SphereCast(transform.position+(transform.up*0.7f),_detectionRange,_detectionDirection,out var hit,_detectionDistance,1<<9,QueryTriggerInteraction.Ignore))
        //    {
        //        //进入大括号说明检测到了
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

        #region 范围检测
        //1.以玩家为中心，取自定义的一个半径园的范围内获取其中的敌人
        //2.在当前玩家没有目标的时候，取距离自身最近的一个敌人为当前目标
        //3.自选：
        //      (1)当前有目标，不再更新目标，直到当前目标消失或距离太远
        private void GetNearUnit()
        {
            if (!_activeEnemyDetection) return;
            if (_currentEnemy != null && DevelopmentToos.DistanceForTarget(_currentEnemy,transform)<3f) return;     //可以添加其它条件触发更新, 清空时会更新
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
                //1.依次去遍历附近的敌人，作比较，选取与玩家最近的第一个敌人作为当前的目标
                var dis = DevelopmentToos.DistanceForTarget(e.transform, transform);
                if (dis < distance)
                {
                    temp_Enemy = e.transform;
                    distance = dis;
                }
            }
            _currentEnemy = temp_Enemy != null ? temp_Enemy : _currentEnemy;
            //重新选择敌人后，重置处决。
            _canFinish = false;
        }
        //清空敌人
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



        #region 角色的基础攻击

        /// <summary>
        /// 是否可以攻击
        /// </summary>
        /// <returns></returns>
        private bool CanBaseAttackInput()
        {
            //_canAttackInput == false，不能攻击
            //角色在挨揍，不能攻击
            //角色在处决，不能攻击
            //角色在格挡，不能攻击

            if (!_canAttackInput) return false;
            if (_animator.AnimationAtTag("Hit")) return false;
            if (_animator.AnimationAtTag("Parry")) return false;
            if (_animator.AnimationAtTag("Finish")) return false;
            return true;

        }

        #region 攻击输入
        protected override void CharacterBaseAttackInput()
        {
            if (!CanBaseAttackInput()) return;
            if (GameInputManager.MainInstance.LAttack)
            {
                //1.判断当前的组合技是否为空或者不为基础组合技(存在变招之后要回到基础攻击的情况，所以需要重置为基础组合技)
                if (_currentCombo == null || _currentCombo != _baseCombo)
                {
                    ChangeComboData(_baseCombo);
                }
                ExcuteAction();
            }
            else if (GameInputManager.MainInstance.RAttack)
            {
                //判断当前Combo是不是变招Combo
                ChangeComboData(_changeHeavyCombo);
                ExcuteAction();          
            }

        }
        #endregion

       
      

        
        #endregion


        #region 处决(特殊攻击)

        /// <summary>
        /// 是否可以特殊攻击
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
                //1.播放处决动画
                _finishComboIndex = Random.Range(0, _finishCombo.TryGetComboMaxCount());
                _animator.Play(_finishCombo.TryGetOneComboAction(_finishComboIndex));
                //2.呼叫事件中心，调用敌人注册的处决事件
                GameEventManager.MainInstance.CallEvent("触发处决", _finishCombo.TryGetOneHitName(_finishComboIndex, 0), transform, _currentEnemy);
                TimeManager.MainInstance.TryGetOneTimer(0.5f, UpdateComboInfo);
                EnemyManager.MainInstance.StopAllActiveUnit();
                //激活摄像机
                GameEventManager.MainInstance.CallEvent("设置相机目标", _currentEnemy, _animator.GetCurrentAnimatorStateInfo(0).length + 1.5f);


                //激活敌人检测
                _activeEnemyDetection = true;
                ResetComboInfo();
            }
        }
        #endregion
        #region  暗杀
        private bool CanAssassination()
        {
            //1.没有目标，不可以
            if (_currentEnemy == null) return false;
            //2.距离太远，不可以
            if (DevelopmentToos.DistanceForTarget(_currentEnemy, transform) > 2) return false;
            //3.角度太大，不可以
            if (Vector3.Angle(transform.forward, _currentEnemy.forward) > 30) return false;
            //4.正在暗杀，不可以
            if (_animator.AnimationAtTag("Assassination")) return false;
             
            return true;
        }

        private void CheckEnemyIsDie()
        { 
            if(_currentEnemy == null) return;
            if (_currentEnemy.TryGetComponent(out IHealth health))
            {
                //如果玩家的动作执行完毕了，同时，敌人已经死亡了，而且动画不在过渡中
                //如果动画还没有执行完毕，然后我们某些动作可能需要去判断或者依赖当前目标
                //那么我们在敌人死亡后直接就移除当前目标，那么玩家就没办法获取当前目标
                //就会报空引用。
                if (_animator.AnimationAtTag("Motion") && health.OnDie() && !_animator.IsInTransition(0))
                {
                    Debug.Log("敌人血空");
                    _currentEnemy = null;
                }
            }
        }
        private void CharacterAssassinationInput()
        {
            if (!CanAssassination()) return;
            if (GameInputManager.MainInstance.TakeOut)
            {
                //1.执行暗杀
                //2.呼叫敌人的暗杀事件
                _finishComboIndex = Random.Range(0, _assassinationCombo.TryGetComboMaxCount());
                _animator.Play(_assassinationCombo.TryGetOneComboAction(_finishComboIndex),0,0);
                GameEventManager.MainInstance.CallEvent("触发处决", _assassinationCombo.TryGetOneHitName(_finishComboIndex, 0), transform, _currentEnemy);
                ResetComboInfo();
            }
        }
        #endregion

        //事件
        private void EnableFinishEventHandler(bool apply)
        {
            if (_canFinish) return;
            _canFinish = apply;
            //取消敌人检测
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
