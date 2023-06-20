using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbController : MonoBehaviour
{
    [SerializeField, Header("检测")] private float _detectionDistance;
    [SerializeField] private LayerMask _detectionLayer;
    private RaycastHit _hit;
    private Animator _animator;


    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    private void Update()
    {
        CharacterClimbInput();
    }
    private bool CanClimb()
    {
        return Physics.Raycast(transform.position + (transform.up * 0.5f), transform.forward, out _hit, _detectionDistance, _detectionLayer, QueryTriggerInteraction.Ignore);
    }
    private void CharacterClimbInput()
    {
        if (!CanClimb()) return;
        if (GameInputManager.MainInstance.Climb )
        {
            /*
             * 先去获取检测到的墙体信息
             */
            var i = _hit.collider.transform.position.y - transform.position.y;
            var j = i + _hit.collider.bounds.extents.y;
            var position = Vector3.zero;
            var rotation = Quaternion.LookRotation(-_hit.normal);
            position.Set(_hit.point.x, j, _hit.point.z);
            switch (_hit.transform.tag)
            {
                case "高墙":
                    ToCallEvent(position, rotation);
                    _animator.CrossFade("爬高墙", 0, 0, 0);
                    break;
                case "中等墙":
                    ToCallEvent(position, rotation);
                    _animator.CrossFade("爬中高墙", 0, 0, 0);
                    break;
            }
        }
       
    }
    private void ToCallEvent(Vector3 position, Quaternion rotation)
    {
        GameEventManager.MainInstance.CallEvent<Vector3, Quaternion>("SetAnimationMatchInfo",position,rotation);
        GameEventManager.MainInstance.CallEvent<bool>("EnableCharacterGravity", false);
    }

}
