using UnityEngine;
using System.Collections.Generic;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    // 敵データ
    public EnemyData[] enemyDataArray;

    // 属性データ
    public Element[] elementDatas;

    public void SpawnRandomEnemy()
    {
        int elementIndex = Random.Range(0, elementDatas.Length);
        Element selectedElement = elementDatas[elementIndex];

        List<EnemyData> candidates = new List<EnemyData>();//candidates

        foreach (EnemyData data in enemyDataArray)
        {
            if (data.enemyElement == selectedElement.enemyElement)
            {
                candidates.Add(data);
            }
        }

        EnemyData selectedEnemy = candidates[Random.Range(0, candidates.Count)];

        GameObject newEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);

        Enemytester tester = newEnemy.GetComponent<Enemytester>();
        if (tester != null)
        {
            tester.Init(selectedEnemy, selectedElement);
        }
    }

    //初期化するやつ
    private void Start()
    {
        SpawnRandomEnemy();
    }
}