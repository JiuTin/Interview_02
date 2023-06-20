using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGG.Tool.Singleton;
public class GameInputManager :Singleton<GameInputManager>
{
    private GameInputAction _gameInputAction;
    // => : Lambda���ʽ
    public Vector2 Movement => _gameInputAction.GameInput.Movement.ReadValue<Vector2>();
    public Vector2 CameraLook => _gameInputAction.GameInput.CameraLook.ReadValue<Vector2>();
    public bool Run => _gameInputAction.GameInput.Run.triggered;
    public bool Climb => _gameInputAction.GameInput.Climb.triggered;
    public bool LAttack => _gameInputAction.GameInput.LAttack.triggered;
    public bool RAttack => _gameInputAction.GameInput.RAttack.triggered;
    public bool TakeOut => _gameInputAction.GameInput.TakeOut.triggered;
    public bool Grab => _gameInputAction.GameInput.Grab.triggered;
    public bool Parry => _gameInputAction.GameInput.Parry.phase == UnityEngine.InputSystem.InputActionPhase.Performed;     //
    public bool Equip => _gameInputAction.GameInput.Equip.triggered;                                                       //
    public bool Interaction => _gameInputAction.GameInput.Interaction.triggered;                                            //space
    protected override void Awake()
    {
        base.Awake();
        //??=  : ���Ϊ��ʱ�������ұߵ�ֵ����ֵ�� ��߲�Ϊ��ʱ���������ұߵ�ֵ
        _gameInputAction ??= new GameInputAction();
    }
    private void OnEnable()
    {
        _gameInputAction.Enable();
    }
    private void OnDisable()
    {
        _gameInputAction.Disable();
    }
}
