using UnityEngine;

public enum SkillType
{
    CloseWeak,
    CloseStrong,
    LongWeak,
    LongStrong,
    Guard,
    HealÅ@
}


[CreateAssetMenu(menuName = "Enemy/EnemySkillData")]
public class EnemySkillData : ScriptableObject
{
    public EnemyElement skillElement;
    public string SkillName;
    public int PP;
    public int Damage;
    public bool Preemptive;

    public SkillType skillType;

}

