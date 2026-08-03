using UnityEngine;

[CreateAssetMenu(menuName = "Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    public EnemyElement enemyElement;
    public string EnemyName;
    public int EnemyHP;
    //使うかわからないけど
    public int EnemyAttackDamage;//エネミー攻撃力
    public int EnemyDefense;//エネミー防御力

    [Header("HP高い時の傾向")]
    public int highHpCloseWeakWeight;
    public int highHpCloseStrongWeight;
    public int highHpLongWeakWeight;
    public int highHpLongStrongWeight;

    [Header("HP中間時の傾向")]
    public int midHpCloseWeakWeight;
    public int midHpCloseStrongWeight;
    public int midHpLongWeakWeight;
    public int midHpLongStrongWeight;

    [Header("HP低い時の傾向")]
    public int lowHpCloseStrongWeight;
    public int lowHpCloseWeakWeight;
    public int lowHpLongStrongWeight;
    public int lowHpLongWeakWeight;

}

