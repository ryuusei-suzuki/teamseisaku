using System.Collections.Generic;
using UnityEngine;

public class DamageCalculator
{
    public Arrribute arribute;

    // SkillData版(プレイヤー用、今まで通り)
    public float CalculateDamage(SkillData skill, List<AttributeType> defenderAttributes, DistanceType defenderDistance)
    {
        return CalculateDamage(skill.attribute, skill.distance, skill.Power, defenderAttributes, defenderDistance);
    }

    // 値を直接渡す版(敵スキルなど、SkillData以外から呼ぶ用)
    public float CalculateDamage(AttributeType attackerAttribute, DistanceType attackerDistance, float power, List<AttributeType> defenderAttributes, DistanceType defenderDistance)
    {
        TurnOrder turnOrder = new TurnOrder();

        int totalScore = 0;
        foreach (AttributeType defenderAttribute in defenderAttributes)
        {
            float multiplier = arribute.GetMultiplier(attackerAttribute, defenderAttribute);

            if (multiplier > 1.0f)
                totalScore = totalScore + 1;
            else if (multiplier < 1.0f)
                totalScore = totalScore - 1;
        }

        float attributeMultiplier = ScoreToMultiplier(totalScore);
        float distanceMultiplier = turnOrder.GetDistanceMultiplier(attackerDistance, defenderDistance);

        return attributeMultiplier * distanceMultiplier * power;
    }

    private float ScoreToMultiplier(int score)
    {
        switch (score)
        {
            case 2: return 1.44f;
            case 1: return 1.2f;
            case 0: return 1.0f;
            case -1: return 0.5f;
            case -2: return 0.25f;
            default: return 1.0f;
        }
    }
}