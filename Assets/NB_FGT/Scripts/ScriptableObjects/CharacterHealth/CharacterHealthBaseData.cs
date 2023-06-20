using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NB_FGT.HealthData
{ 
    [CreateAssetMenu(fileName ="BaseHealthData",menuName ="Create/Character/HealthData/BaseData",order =0)]
    public class CharacterHealthBaseData:ScriptableObject
    {
        [SerializeField] private float _maxHP;
        [SerializeField] private float _maxStrength;
        //��ʼ�������ֵ
        public float MaxHP => _maxHP;
        //��ʼ�������ֵ
        public float MaxStreath => _maxStrength;
    }

}
