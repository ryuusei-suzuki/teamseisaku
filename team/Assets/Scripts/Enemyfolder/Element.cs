using UnityEngine;

public enum EnemyElement
{
    None,
    fire,
    Bubble,
    wind
}

[CreateAssetMenu(menuName = "Enemy/EnemyElement")]
public class Element : ScriptableObject
{
    public EnemyElement enemyElement;

    public EnemySkillData CloseWeakAttack;
    public EnemySkillData CloseStrongAttack;
    public EnemySkillData LongWeakAttack;
    public EnemySkillData LongStrongAttack;

    public Sprite enemySprite;
}
