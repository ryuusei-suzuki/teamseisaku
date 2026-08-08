using System;
using System.Collections.Generic;
using UnityEngine;

public class DamageCalculator
{
    public Arrribute arribute;


    public float CalculateDamage(SkillData skill, List<AttributeType> defenderAttributes, DistanceType defenderDistance)
    {
        TurnOrder turnOrder = new TurnOrder();

        // 各属性の有利不利を点数化して合計する(有利+1、不利-1、等倍0)
        int totalScore = 0;
        foreach (AttributeType defenderAttribute in defenderAttributes)
        {
            float multiplier = arribute.GetMultiplier(skill.attribute, defenderAttribute);

            if (multiplier > 1.0f)
            {
                totalScore = totalScore + 1; // 有利
            }
            else if (multiplier < 1.0f)
            {
                totalScore = totalScore - 1; // 不利
            }
            // 等倍(1.0f)のときは加算しない
        }

        // 合計点数から、最終的な属性倍率を決定する
        float attributeMultiplier;
        switch (totalScore)
        {
            case 2:
                attributeMultiplier = 1.44f; // 超有利
                break;
            case 1:
                attributeMultiplier = 1.2f;  // 有利
                break;
            case 0:
                attributeMultiplier = 1.0f;  // 等倍
                break;
            case -1:
                attributeMultiplier = 0.5f;  // 不利
                break;
            case -2:
                attributeMultiplier = 0.25f; // 超不利
                break;
            default:
                attributeMultiplier = 1.0f;  // 属性が3つ以上等になった場合の保険
                break;
        }

        float distanceMultiplier = turnOrder.GetDistanceMultiplier(skill.distance, defenderDistance);

        float damage = attributeMultiplier * distanceMultiplier * skill.Power;
        return damage;
    }
}
