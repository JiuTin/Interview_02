using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace NB_FGT.ComboData
{ 
    [CreateAssetMenu(fileName ="ComboData",menuName ="Create/Character/ComboData",order =0)]
    public class CharacterComboDataSO : ScriptableObject
    {
        //招式名称
        [SerializeField] private string _comboName;
        [SerializeField] private float _damage;
        [SerializeField] private string[] _comboHitName;   //受伤动画名称
        [SerializeField] private string[] _comboParryName; //格挡动画
        [SerializeField] private float _coldTime;         //衔接下一段攻击的间隔时间
        [SerializeField] private float _comboPositionOffset;                   //让这段攻击与目标之间保持最佳距离

        public string ComboName => _comboName;
        public string[] ComboHitName => _comboHitName;
        public string[] ComboParryName => _comboParryName;
        public float Damage => _damage;
        public float ColdTime => _coldTime;
        public float ComboPositionOffset => _comboPositionOffset;

        /// <summary>
        /// 获取当前动作最大的受伤动画数量
        /// </summary>
        /// <returns></returns>
        public int GetHitAndParryNameMaxCount() => _comboHitName.Length;
    }

}
