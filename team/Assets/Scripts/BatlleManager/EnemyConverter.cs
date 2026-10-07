using UnityEngine;

public static class EnemyConverter
{
    // “G‘¤‚Ì‘®«‚ğA‚±‚¿‚ç‚ÌAttributeType‚É•ÏŠ·‚·‚é
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
                // EnemyElement.None(ƒK[ƒhEƒq[ƒ‹‚È‚Ç–³‘®«‹Z)‚Ìê‡‚Í“™”{(–³‘®«)‚Æ‚µ‚Äˆµ‚¤
                return AttributeType.none;
        }
    }

    // “GƒXƒLƒ‹‚ÌSkillType‚ğA‚±‚¿‚ç‚ÌDistanceType‚É•ÏŠ·‚·‚é
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
                // Guard/Heal‚È‚ÇAUŒ‚‹ZˆÈŠO‚ª—ˆ‚½ê‡‚Ìb’è‘Î‰
                return DistanceType.Melee;
        }
    }
}