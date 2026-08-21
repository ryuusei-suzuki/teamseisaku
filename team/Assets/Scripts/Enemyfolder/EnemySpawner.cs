using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    // 敵データ
    public EnemyData[] enemyDataArray;

    // 属性データ
    public Element[] elementDatas;

    [Range(0f, 1f)]
    public float noSubElementChance = 0.3f; // とりあえず30%で副属性なし

    public void SpawnRandomEnemy()
    {
        // 主属性を選ぶ
        List<Element> mainCandidates = new();

        foreach (Element e in elementDatas)
        {
            if (e.enemyElement != EnemyElement.None)
            {
                mainCandidates.Add(e);
            }
        }

        if (mainCandidates.Count == 0)
        {
            Debug.LogError("主属性候補がありません！");
            return;
        }

        Element mainElement = mainCandidates[Random.Range(0, mainCandidates.Count)];

        // その属性の敵を探す
        List<EnemyData> enemyCandidates = new();

        foreach (EnemyData data in enemyDataArray)
        {
            if (data.enemyElement == mainElement.enemyElement)
            {
                enemyCandidates.Add(data);
            }
        }

        if (enemyCandidates.Count == 0)
        {
            Debug.LogWarning("対応する敵がいない");
            return;
        }

        EnemyData selectedEnemy =
            enemyCandidates[Random.Range(0, enemyCandidates.Count)];

        // 副属性を決める
        Element subElement = null;

        // 一定確率で副属性を付ける
        if (Random.value > noSubElementChance)
        {
            List<Element> subCandidates = new();

            foreach (Element e in elementDatas)
            {
                if (e.enemyElement != EnemyElement.None && e.enemyElement != mainElement.enemyElement)
                {
                    subCandidates.Add(e);
                }
            }

            if (subCandidates.Count > 0)
            {
                subElement = subCandidates[Random.Range(0, subCandidates.Count)];
            }
        }


        // 敵生成
        GameObject newEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);

        Enemytester tester = newEnemy.GetComponent<Enemytester>();

        if (tester != null)
        {
            tester.Init(selectedEnemy, mainElement, subElement);
        }

        string subName =
            subElement != null ? subElement.enemyElement.ToString(): "None";

        Debug.Log($"主属性:{mainElement.enemyElement} / 副属性:{subName}");
        FindFirstObjectByType<BattleManager>().enemy = tester;
    }

    // 指定したEnemyDataで敵を生成する(3体連戦用)
    public Enemytester SpawnSpecificEnemy(EnemyData data)
    {
        Element mainElement = null;
        foreach (Element e in elementDatas)
        {
            if (e.enemyElement == data.enemyElement)
            {
                mainElement = e;
                break;
            }
        }

        if (mainElement == null)
        {
            Debug.LogError("対応するElementが見つかりません: " + data.EnemyName);
            return null;
        }

        Element subElement = null;
        if (Random.value > noSubElementChance)
        {
            List<Element> subCandidates = new();
            foreach (Element e in elementDatas)
            {
                if (e.enemyElement != EnemyElement.None && e.enemyElement != mainElement.enemyElement)
                    subCandidates.Add(e);
            }
            if (subCandidates.Count > 0)
                subElement = subCandidates[Random.Range(0, subCandidates.Count)];
        }

        GameObject newEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        Enemytester tester = newEnemy.GetComponent<Enemytester>();
        if (tester != null)
        {
            tester.Init(data, mainElement, subElement);
        }
        return tester;
    }

    private void Start()
    {
  
       
    }
}