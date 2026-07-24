using UnityEngine;

public enum EnemyElement
{
    fire,
    Bubble,
    wind
}

[CreateAssetMenu(menuName = "Enemy/EnemyElement")]
public class Element : ScriptableObject
{
    public EnemyElement enemyElement;

    public EnemySkillData CloseWaekAttack;
    public EnemySkillData CloseStrongAttack;
    public EnemySkillData LongWaekAttack;
    public EnemySkillData LongStrongAttack;


}
