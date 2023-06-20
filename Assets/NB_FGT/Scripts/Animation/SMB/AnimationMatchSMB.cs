using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationMatchSMB : StateMachineBehaviour
{
    [SerializeField, Header("匹配信息")]
    private float _startTime;
    [SerializeField] private float _endTime;
    [SerializeField] private AvatarTarget _avatarTarget;
    [SerializeField,Header("激活重力")] private bool _isEnableGravity;
    [SerializeField] private float _enableTime;

    private Vector3 _matchPosition;
    private Quaternion _matchRotation;
    private void OnEnable()
    {
        GameEventManager.MainInstance.AddEventListening<Vector3, Quaternion>("SetAnimationMatchInfo", GetMatchInfo);
    }
    private void OnDisable()
    {
        GameEventManager.MainInstance.RemoveEvent<Vector3, Quaternion>("SetAnimationMatchInfo", GetMatchInfo);
    }
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //isMatchingTarget: 自动匹配是否处于激活状态
        if (!animator.isMatchingTarget)
        {
            animator.MatchTarget(_matchPosition, _matchRotation, _avatarTarget, new MatchTargetWeightMask(Vector3.one, 0f), _startTime, _endTime);
        }
        if (_isEnableGravity)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > _enableTime)
            {
                GameEventManager.MainInstance.CallEvent<bool>("EnableCharacterGravity", true);
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
    private void GetMatchInfo(Vector3 position, Quaternion rotation)
    {
        _matchPosition = position;
        _matchRotation = rotation;
    }
}
