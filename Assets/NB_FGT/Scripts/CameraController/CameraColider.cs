using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGG.Tool;
public class CameraColider : MonoBehaviour
{
    [SerializeField, Header("最大最小偏移量")] private Vector2 _maxDistanceOffset;
    [SerializeField, Header("检测层级"), Space(10)] 
    private LayerMask _whatIsWall;
    [SerializeField, Header("射线长度"), Space(10)]
    private float _detectionDistance;
    [SerializeField, Header("平滑时间"), Space(10)]
    private float _coliderSmoothTime;
    private Transform _mainCamera;

    //开始的时候，保存一下起始点和起始的偏移量
    private Vector3 _originPosition;
    private float _originOffsetDistance;
    private void Awake()
    {
        _mainCamera = Camera.main.transform;
    }
    private void Start()
    {
        _originPosition = transform.localPosition.normalized;   //0-1,0归一化后是0.其它都是1.
        _originOffsetDistance = _maxDistanceOffset.y;
    }
    private void LateUpdate()
    {
        UpdateCameraCollider();
    }
    private void UpdateCameraCollider()
    {
        //从本地空间变换到世界空间(包括长度)
        var detectionDirection = transform.TransformPoint(_originPosition * _detectionDistance);
        if (Physics.Linecast(transform.position, detectionDirection, out var hit, _whatIsWall, QueryTriggerInteraction.Ignore))
        {
            //往前移动0.8的检测距离
            _originOffsetDistance = Mathf.Clamp(hit.distance * 0.8f, _maxDistanceOffset.x, _maxDistanceOffset.y);
        }
        else
        {
            _originOffsetDistance = _maxDistanceOffset.y;
        }
        _mainCamera.localPosition = Vector3.Lerp(_mainCamera.localPosition,
            _originPosition * (_originOffsetDistance - 0.1f), DevelopmentToos.UnTetheredLerp(_coliderSmoothTime));
    }
}
