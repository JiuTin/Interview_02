using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGG.Tool;
public class CameraColider : MonoBehaviour
{
    [SerializeField, Header("�����Сƫ����")] private Vector2 _maxDistanceOffset;
    [SerializeField, Header("���㼶"), Space(10)] 
    private LayerMask _whatIsWall;
    [SerializeField, Header("���߳���"), Space(10)]
    private float _detectionDistance;
    [SerializeField, Header("ƽ��ʱ��"), Space(10)]
    private float _coliderSmoothTime;
    private Transform _mainCamera;

    //��ʼ��ʱ�򣬱���һ����ʼ�����ʼ��ƫ����
    private Vector3 _originPosition;
    private float _originOffsetDistance;
    private void Awake()
    {
        _mainCamera = Camera.main.transform;
    }
    private void Start()
    {
        _originPosition = transform.localPosition.normalized;   //0-1,0��һ������0.��������1.
        _originOffsetDistance = _maxDistanceOffset.y;
    }
    private void LateUpdate()
    {
        UpdateCameraCollider();
    }
    private void UpdateCameraCollider()
    {
        //�ӱ��ؿռ�任������ռ�(��������)
        var detectionDirection = transform.TransformPoint(_originPosition * _detectionDistance);
        if (Physics.Linecast(transform.position, detectionDirection, out var hit, _whatIsWall, QueryTriggerInteraction.Ignore))
        {
            //��ǰ�ƶ�0.8�ļ�����
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
