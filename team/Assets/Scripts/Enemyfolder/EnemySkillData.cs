using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/EnemySkillData")]
public class EnemySkillData : ScriptableObject
{
    public string SkillName;
    public int PP;
    public int Damage;
    public bool Preemptive;
}
