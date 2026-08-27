using UnityEngine;
using System.Collections.Generic;
using TMPro;
public class SkillSceneBossPreviw : MonoBehaviour
{
    public List<EnemyData> bossList;
    public EnemySpawner spawner; // 副属性抽選用。実際に敵は生成しないので Enemy Prefab は空でも可
    public TextMeshProUGUI bossInfoText;

    private List<BossPlanEntry> bossPlan;

    void Start()
    {
        GenerateBossPlan();
        ShowBossInfo();
        BossPreviewData.bossPlan = bossPlan; // ここで保管庫に詰めておく
    }

    private void GenerateBossPlan()
    {
        List<EnemyData> shuffled = new List<EnemyData>(bossList);
        for (int i = 0; i < shuffled.Count; i++)
        {
            int rand = Random.Range(i, shuffled.Count);
            (shuffled[i], shuffled[rand]) = (shuffled[rand], shuffled[i]);
        }

        bossPlan = new List<BossPlanEntry>();
        foreach (EnemyData data in shuffled)
        {
            BossPlanEntry entry = new BossPlanEntry();
            entry.data = data;
            entry.subElement = spawner.DetermineSubElement(data.enemyElement);
            bossPlan.Add(entry);
        }
    }

    private void ShowBossInfo()
    {
        string info = "出現するボス:\n";
        foreach (BossPlanEntry entry in bossPlan)
        {
            
            info += $"{entry.data.EnemyName}(主属性: {ElementToJapanese(entry.data.enemyElement)})\n";
        }
        bossInfoText.text = info;
    }

    private string ElementToJapanese(EnemyElement element)
    {
        switch (element)
        {
            case EnemyElement.fire: return "火";
            case EnemyElement.Bubble: return "水";
            case EnemyElement.wind: return "風";
            default: return "無";
        }
    }
}
