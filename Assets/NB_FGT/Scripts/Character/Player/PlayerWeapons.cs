using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GGG.Tool;
public class PlayerWeapons : MonoBehaviour
{
    [SerializeField] private Transform[] handsWeapon;     //���ϵ�����
    [SerializeField] private Transform hipWeapon;         //���������
    private Animator _animator;
    private bool _isShow;

    private void Awake()
    {
        _animator=GetComponent<Animator>();
    }
    private void Update()
    {
        ControlShowWP();
    }
    private void ControlShowWP()
    {
        if (_animator.AnimationAtTag("Equip")) return;
        if (!_isShow)
        {
            if (GameInputManager.MainInstance.Equip)
            {
                _animator.Play("EquipWP");
            }
        }
        else
        {
            if (GameInputManager.MainInstance.Equip)
            {
                _animator.Play("UnEquipWP");
            }
        }
    }

    public void ShowHandWP()
    {
        if (handsWeapon.Length == 0) return;  //��ֹ�����ô���
        foreach (var wp in handsWeapon)
        {
            wp.gameObject.SetActive(true);
        }
        _isShow = true;
        _animator.SetBool(AnimationID.ShowWPID, _isShow);
        hipWeapon.gameObject.SetActive(false);
    }
    public void HideHandWP()
    {
        if (handsWeapon.Length == 0) return;
        foreach (var wp in handsWeapon)
        {
            wp.gameObject.SetActive(false);
        }
        _isShow = false;
        _animator.SetBool(AnimationID.ShowWPID, _isShow);
        hipWeapon.gameObject.SetActive(true);
    }
}
