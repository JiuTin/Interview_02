using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGG.Tool;
public class TP_CameraController : MonoBehaviour
{
    //������ƶ��ٶ�
    [SerializeField,Header("�����������")]private float _controlSpeed;
    [SerializeField]private Vector2 _cameraVerticalMaxAngle; //����������¿������Ƕ�
    [SerializeField] private float _smoothSpeed;
    private Transform _lookTarget;
    [SerializeField] private float _positionOffset;
    [SerializeField] private float _positionSmoothTime;
    private Vector3 _smoothDampVelocity = Vector3.zero;
    private Vector2 _input;
    private Vector3 _cameraRotation;

    //�����µ���ز���
    private Transform _currentTarget;
    private bool _isFinish;

    private void Awake()
    {
        //_gameInputManager = GetComponent<GameInputManager>();
        _lookTarget = GameObject.FindWithTag("CameraTarget").transform;
    }
    private void OnEnable()
    {
        GameEventManager.MainInstance.AddEventListening<Transform,float>("�������Ŀ��", SetFinishTarget);
    }
    private void OnDisable()
    {
        GameEventManager.MainInstance.RemoveEvent<Transform,float>("�������Ŀ��", SetFinishTarget);
    }

    private void Start()
    {
        TP_CameraController.LockMouse(true);
        _currentTarget = _lookTarget;
    }
    private void Update()
    {
        CameraInput();
    }
    private void LateUpdate()
    {
        UpdateCameraRotation();
        CameraPosition();
    }
    private void CameraInput()
    {
        _input.y += GameInputManager.MainInstance.CameraLook.x * _controlSpeed;
        _input.x -= GameInputManager.MainInstance.CameraLook.y * _controlSpeed;
        _input.x = Mathf.Clamp(_input.x,_cameraVerticalMaxAngle.x, _cameraVerticalMaxAngle.y);
    }
    private void UpdateCameraRotation()
    {
        _cameraRotation = Vector3.SmoothDamp(_cameraRotation, new Vector3(_input.x, _input.y, 0), ref _smoothDampVelocity, _smoothSpeed);
        transform.eulerAngles = _cameraRotation;
    }
    private void CameraPosition()
    {
        var newPosition = ((_isFinish)?_currentTarget.position+_currentTarget.up*0.7f:_currentTarget.position + (-transform.forward * _positionOffset));   //Ŀ���λ��+ƫ����
        transform.position = Vector3.Lerp(transform.position,newPosition,DevelopmentToos.UnTetheredLerp(_positionSmoothTime));
    }

    //
    private void SetFinishTarget(Transform target, float time)
    {
        _isFinish = true;
        _currentTarget = target;
        _positionOffset = 0.01f;
        TimeManager.MainInstance.TryGetOneTimer(time, ResetTarget);
    }

    private void ResetTarget()
    {
        _isFinish = false;
        _positionOffset = 0.5f;
        _currentTarget = _lookTarget;
    }

    //�ڽ�ɫ������״̬�£���ʾ���
    public static void LockMouse(bool flag)
    {
        if (flag)
        {
            Cursor.visible = false;                      //�α겻�ɼ�
            Cursor.lockState = CursorLockMode.Locked;  //�α������ģʽΪ����
        }
        else
        { 
            Cursor.visible = true;                      //�α겻�ɼ�
            Cursor.lockState = CursorLockMode.Confined;  //�α������ģʽΪ����
        
        }
    }
}
