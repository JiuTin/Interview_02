using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "BaseHealthData", menuName = "Create/Character/SkillData/SkillDataInfoBase", order = 0)]
public class SkillDataInfoBase : ScriptableObject
{
    public uint _skillId;
    public string _skillName;
    public float _skillLP;
    public float _skillCollTime;
    public float _skillDamage;
    //TODO ·â±Õ×Ö¶Î
}
