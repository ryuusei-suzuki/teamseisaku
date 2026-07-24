using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/EnemySkillData")]
public class EnemySkillData : ScriptableObject
{
    public string SkillName;
    public string PP;
    public int Damege;
    public bool Preemptive;
}
