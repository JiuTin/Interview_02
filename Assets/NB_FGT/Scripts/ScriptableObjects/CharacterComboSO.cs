using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NB_FGT.ComboData
{
    [CreateAssetMenu(fileName = "Combo", menuName = "Create/Character/Combo", order = 0)]
    public class CharacterComboSO : ScriptableObject
    {
        [SerializeField] private List<CharacterComboDataSO> _allComboData = new List<CharacterComboDataSO>();

        public string TryGetOneComboAction(int index)
        {
            if (_allComboData.Count == 0) return null; //
            return _allComboData[index].ComboName;
        }
        public string TryGetOneHitName(int index, int hitIndex)
        {
            if (_allComboData.Count == 0) return null;    //���б�ɶ��û�У�����null
            if (_allComboData[index].GetHitAndParryNameMaxCount() == 0) return null; //���õ�ʱ���������ˣ����±�������
            return _allComboData[index].ComboHitName[hitIndex];
        }
        public string TryGetOneParryName(int index, int hitIndex)
        {
            if (_allComboData.Count == 0) return null;    //���б�ɶ��û�У�����null
            if (_allComboData[index].GetHitAndParryNameMaxCount() == 0) return null; //���õ�ʱ���������ˣ����±�������
            return _allComboData[index].ComboParryName[hitIndex];
        }
        public float TryGetDamage(int index)
        {
            if (_allComboData.Count == 0) return 0f;
            return _allComboData[index].Damage;
        }
        public float TryGetColdTime(int index)
        {
            if (_allComboData.Count == 0) return 0f;
            return _allComboData[index].ColdTime;
        }
        public float TryGetComboPositionOffset(int index)
        {
            if (_allComboData.Count == 0) return 0f;
            return _allComboData[index].ComboPositionOffset;
        }
        public int TryGetHitOrParryMaxCount(int index) => _allComboData[index].GetHitAndParryNameMaxCount();
        public int TryGetComboMaxCount() => _allComboData.Count;
    }

}
