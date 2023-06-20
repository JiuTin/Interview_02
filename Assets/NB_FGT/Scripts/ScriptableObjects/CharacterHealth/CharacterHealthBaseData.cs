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
        //初始最大生命值
        public float MaxHP => _maxHP;
        //初始最大体力值
        public float MaxStreath => _maxStrength;
    }

}
