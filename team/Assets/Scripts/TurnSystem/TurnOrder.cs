using UnityEngine;

public class TurnOrder
{
    private DistanceRelation GetRelation(DistanceType playerDistance, DistanceType enemyDistance)
    {
        if (playerDistance == DistanceType.Melee && enemyDistance == DistanceType.Ranged)
        {
            return DistanceRelation.EnemyRanged;
        }
        else if (playerDistance == DistanceType.Ranged && enemyDistance == DistanceType.Melee)
        {
            return DistanceRelation.PlayerRanged;
        }
        else
        {
            return DistanceRelation.Same;
        }
    }

    public bool IsPlayerFirst(DistanceType playerDistance, DistanceType enemyDistance)
    {
        DistanceRelation relation = GetRelation(playerDistance, enemyDistance);

        switch (relation)
        {
            case DistanceRelation.PlayerRanged:
                return true;
            case DistanceRelation.EnemyRanged:
                return false;
            case DistanceRelation.Same:
                return Random.value < 0.5f; // 50%’Š‘I‚É•ÏX
            default:
                return true;
        }
    }

    public float GetDistanceMultiplier(DistanceType attackerDistance, DistanceType defenderDistance)
    {
        DistanceRelation relation = GetRelation(attackerDistance, defenderDistance);

        switch (relation)
        {
            case DistanceRelation.PlayerRanged:
                return 1.0f;
            case DistanceRelation.EnemyRanged:
                return 1.5f;
            case DistanceRelation.Same:
                return 1.0f;
            default:
                return 1.0f;
        }
    }
}