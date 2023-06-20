using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NB_FGT.HealthData
{
    [CreateAssetMenu(fileName = "BaseHealthData", menuName = "Create/Character/HealthData/CharacterHealthInfo", order = 0)]
    public class CharacterHealthInfo : ScriptableObject
    {
        

        //1.最大生命值
        //2.最大体力值
        //3.当前生命值
        //4.当前体力值
        //5.是否死亡
        //6.当前体立是否充沛
        private float _currentHP;
        private float _currentStrength;
        private float _maxHP;
        private float _maxStrength;
        private bool _strengthFull;
        private bool _isDie => (_currentHP <= 0);


        public float CurrentHP => _currentHP;
        public float CurrentStrength => _currentStrength;
        public float MaxHP => _maxHP;
        public float MaxStrength => _maxStrength;
        public bool StrengthFull => _strengthFull;
        public bool IsDie => _isDie;

        [SerializeField] private CharacterHealthBaseData _characterHealthBase;
        public void InitCharacterHealthInfo()
        {
            _maxHP = _characterHealthBase.MaxHP;
            _maxStrength = _characterHealthBase.MaxStreath;
            _currentHP = _characterHealthBase.MaxHP;
            _currentStrength = _characterHealthBase.MaxStreath;
            _strengthFull = true;
        }

        public void Damage(float damage,bool hasParry=false)
        {
            //1.如果体力充沛，那么要扣除体力值
            //2.敌人现在正在进行攻击动作中，还没打到玩家，而被玩家打到。
            if (_strengthFull && hasParry)
            {
                _currentStrength = Clamp(_currentStrength, damage, 0, _maxStrength);
                if (_currentStrength <= 0)
                    _strengthFull = false;
            }
            else
            { 
                _currentHP = Clamp(_currentHP, damage, 0, _maxStrength);
                
            }
        }
        public void DamageToStrength(float damage)
        {
            if (_strengthFull)
            { 
                _currentStrength = Clamp(_currentStrength, damage, 0f, _maxStrength);
                if (_currentStrength <= 0)
                    _strengthFull = false;
            }
        }
        public void AddHP(float hp)
        {
            _currentHP = Clamp(_currentHP, hp, 0, _maxHP,true);
        }
        public void AddStrength(float strength)
        {
            _currentStrength = Clamp(_currentStrength, strength, 0, _maxStrength, true);
            if (_currentStrength >= _maxStrength)
                _strengthFull = true;
        }
        private float Clamp(float value,float offsetValue,float min, float max,bool Add=false)
        {
            return Mathf.Clamp((Add) ? value + offsetValue : value - offsetValue, min, max);
        }

    }

}
