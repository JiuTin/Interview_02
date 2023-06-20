using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace NB_FGT.ComboData
{ 
    [CreateAssetMenu(fileName ="ComboData",menuName ="Create/Character/ComboData",order =0)]
    public class CharacterComboDataSO : ScriptableObject
    {
        //��ʽ����
        [SerializeField] private string _comboName;
        [SerializeField] private float _damage;
        [SerializeField] private string[] _comboHitName;   //���˶�������
        [SerializeField] private string[] _comboParryName; //�񵲶���
        [SerializeField] private float _coldTime;         //�ν���һ�ι����ļ��ʱ��
        [SerializeField] private float _comboPositionOffset;                   //����ι�����Ŀ��֮�䱣����Ѿ���

        public string ComboName => _comboName;
        public string[] ComboHitName => _comboHitName;
        public string[] ComboParryName => _comboParryName;
        public float Damage => _damage;
        public float ColdTime => _coldTime;
        public float ComboPositionOffset => _comboPositionOffset;

        /// <summary>
        /// ��ȡ��ǰ�����������˶�������
        /// </summary>
        /// <returns></returns>
        public int GetHitAndParryNameMaxCount() => _comboHitName.Length;
    }

}
