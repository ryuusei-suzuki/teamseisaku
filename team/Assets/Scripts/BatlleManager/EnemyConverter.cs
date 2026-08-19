using UnityEngine;

public static class EnemyConverter
{
    // 敵側の属性を、こちらのAttributeTypeに変換する
    public static AttributeType ToAttributeType(EnemyElement element)
    {
        switch (element)
        {
            case EnemyElement.fire:
                return AttributeType.Fire;
            case EnemyElement.Bubble:
                return AttributeType.Water;
            case EnemyElement.wind:
                return AttributeType.Wind;
            default:
                // EnemyElement.Noneが来た場合の暫定対応。要確認。
                return AttributeType.Fire;
        }
    }

    // 敵スキルのSkillTypeを、こちらのDistanceTypeに変換する
    public static DistanceType ToDistanceType(SkillType skillType)
    {
        switch (skillType)
        {
            case SkillType.CloseWeak:
            case SkillType.CloseStrong:
                return DistanceType.Melee;
            case SkillType.LongWeak:
            case SkillType.LongStrong:
                return DistanceType.Ranged;
            default:
                // Guard/Healなど、攻撃技以外が来た場合の暫定対応
                return DistanceType.Melee;
        }
    }
}