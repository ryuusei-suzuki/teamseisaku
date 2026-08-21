using System.Collections.Generic;
using UnityEngine;

public class DamageCalculator
{
    public Arrribute arribute;

    public float CalculateDamage(SkillData skill, List<AttributeType> defenderAttributes, DistanceType defenderDistance, out string effectivenessText)
    {
        return CalculateDamage(skill.attribute, skill.distance, skill.Power, defenderAttributes, defenderDistance, out effectivenessText);
    }

    public float CalculateDamage(AttributeType attackerAttribute, DistanceType attackerDistance, float power, List<AttributeType> defenderAttributes, DistanceType defenderDistance, out string effectivenessText)
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
        effectivenessText = ScoreToText(totalScore);

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

    private string ScoreToText(int score)
    {
        if (score > 0)
            return "Ç±Ç§Ç©ÇÕ ÇŒÇ¬ÇÆÇÒÇæÅI";
        else if (score < 0)
            return "Ç±Ç§Ç©ÇÕ Ç¢Ç‹Ç–Ç∆Ç¬ÇÃ ÇÊÇ§ÇæÅc";
        else
            return "";
    }
}