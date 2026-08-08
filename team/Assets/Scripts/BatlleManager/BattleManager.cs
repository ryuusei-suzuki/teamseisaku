using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public int playerHp;
    public int enemyHp;
    public SkillData playerSkill;
    public SkillData enemySkill;
    public List<AttributeType> enemyAttributes;
    public Arrribute arribute;

    public void ExecuteTurn()
    {
        TurnOrder turnOrder = new TurnOrder();
        bool isPlayerFirst = turnOrder.IsPlayerFirst(playerSkill.distance, enemySkill.distance);

        DamageCalculator calculator = new DamageCalculator();
        calculator.arribute = arribute;

        if (isPlayerFirst)
        {
            // ƒvƒŒƒCƒ„[‚ªæ§
            float damageToEnemy = calculator.CalculateDamage(playerSkill, enemyAttributes, enemySkill.distance);
            enemyHp -= (int)damageToEnemy;

            if (enemyHp > 0)
            {
                // “G‚ª¶‚«‚Ä‚¢‚ê‚Î”½Œ‚
                List<AttributeType> playerAttributes = new List<AttributeType> { playerSkill.attribute };
                float damageToPlayer = calculator.CalculateDamage(enemySkill, playerAttributes, playerSkill.distance);
                playerHp -= (int)damageToPlayer;
            }
        }
        else
        {
            // “G‚ªæ§
            List<AttributeType> playerAttributesForEnemyAttack = new List<AttributeType> { playerSkill.attribute };
            float damageToPlayer = calculator.CalculateDamage(enemySkill, playerAttributesForEnemyAttack, playerSkill.distance);
            playerHp -= (int)damageToPlayer;

            if (playerHp > 0)
            {
                float damageToEnemy = calculator.CalculateDamage(playerSkill, enemyAttributes, enemySkill.distance);
                enemyHp -= (int)damageToEnemy;
            }
        }
    }
}