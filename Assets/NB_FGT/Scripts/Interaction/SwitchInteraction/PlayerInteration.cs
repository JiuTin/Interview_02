using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteration : MonoBehaviour,IInteraction
{
    private Animator _animator;
    public bool _key;
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    private void Start()
    {
        _key = false;
    }
    private void OnEnable()
    {
        //GameEventManager.MainInstance.AddEventListening("�򿪿���", Interaction);
    }
    private void OnDisable()
    {
        //GameEventManager.MainInstance.RemoveEvent("�򿪿���", Interaction);
    }
    private void Interaction()
    {

        if (!_key) return;



        //TODO   ������Ҷ�Ӧ����   ---���Խ����������ַ����鵽һ�������Ϊֻ���ֶ�
        //_animator.Play("��Ӧ��������", 0, 0);
    }

    public void InteractionAction()
    {
        Interaction();
    }

    public bool CanInteraction()
    {
        return _key;
    }
}
