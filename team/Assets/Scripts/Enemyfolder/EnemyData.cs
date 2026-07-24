using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string EnemyName;
    public int EnemyHP;
    //使うかわからないけど
    public int EnemyAttackDamage;//エネミー攻撃力
    public int EnemyDefense;//エネミー防御力

}

