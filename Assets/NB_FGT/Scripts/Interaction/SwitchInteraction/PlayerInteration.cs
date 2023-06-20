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
        //GameEventManager.MainInstance.AddEventListening("打开开关", Interaction);
    }
    private void OnDisable()
    {
        //GameEventManager.MainInstance.RemoveEvent("打开开关", Interaction);
    }
    private void Interaction()
    {

        if (!_key) return;



        //TODO   播放玩家对应动画   ---可以将动画名称字符串归到一个类里，作为只读字段
        //_animator.Play("对应动画名称", 0, 0);
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
