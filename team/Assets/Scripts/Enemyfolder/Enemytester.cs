using UnityEngine;

public class Enemytester : MonoBehaviour
{
    public EnemyData enemyData;
    
    public int NowEnemyHP;

    public EnemyElement enemyElement;

    public Element[] elementDatas;

    public EnemySkillData closeWeak;
    public EnemySkillData closeStrong;
    public EnemySkillData longWeak;
    public EnemySkillData longStrong;
    public void Init(EnemyData data)
    {
        enemyData = data;
        NowEnemyHP = enemyData.EnemyHP;

        if (elementDatas != null && elementDatas.Length > 0)
        {
            int randomIndex = Random.Range(0, elementDatas.Length);
            Element selectedElement = elementDatas[randomIndex];

            enemyElement = selectedElement.enemyElement;

            closeWeak = selectedElement.CloseWaekAttack;
            closeStrong = selectedElement.CloseStrongAttack;
            longWeak = selectedElement.LongWaekAttack;
            longStrong = selectedElement.LongStrongAttack;
        }
    }

}
