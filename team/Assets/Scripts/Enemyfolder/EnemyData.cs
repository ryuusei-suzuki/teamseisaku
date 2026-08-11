using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    public EnemyElement enemyElement;
    public string EnemyName;
    public int EnemyHP;
    //g‚¤‚©‚í‚©‚ç‚È‚¢‚¯‚Ç
    public int EnemyAttackDamage;//ƒGƒlƒ~[UŒ‚—Í
    public int EnemyDefense;//ƒGƒlƒ~[–hŒä—Í

    [Header("HP‚‚¢‚ÌŒXŒü")]
    public int highHpCloseWeakWeight;
    public int highHpCloseStrongWeight;
    public int highHpLongWeakWeight;
    public int highHpLongStrongWeight;

    [Header("HP’†ŠÔ‚ÌŒXŒü")]
    public int midHpCloseWeakWeight;
    public int midHpCloseStrongWeight;
    public int midHpLongWeakWeight;
    public int midHpLongStrongWeight;

    [Header("HP’á‚¢‚ÌŒXŒü")]
    public int lowHpCloseStrongWeight;
    public int lowHpCloseWeakWeight;
    public int lowHpLongStrongWeight;
    public int lowHpLongWeakWeight;

    [Header("“Áê‹Z")]
    public EnemySkillData GuardSkill;
    public EnemySkillData HealSkill;

    [Header("“Áê‹Z‚ÌŒXŒü")]
    public int highHpGuardWeight;
    public int midHpGuardWeight;
    public int lowHpGuardWeight;
    public int highHpHealWeight;
    public int midHpHealWeight;
    public int lowHpHealWeight;

    [Header("‘®«‘I‘ğ‚Ìd‚İ")]
    public int mainElementWeight = 8;
    public int subElementWeight = 2;
}

