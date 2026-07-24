using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;

    // インスペクターで複数のEnemyDataを登録できるように配列にする []
    public EnemyData[] enemyDataArray;

    public void SpawnRandomEnemy()
    {
        // 1. 配列の中からランダムに1つの要素を選ぶ
        int randomIndex = Random.Range(0, enemyDataArray.Length);
        EnemyData selectedData = enemyDataArray[randomIndex];

        // 2. プレハブを生成
        GameObject newEnemy = Instantiate(enemyPrefab, transform.position, Quaternion.identity);

        // 3. ランダムに選んだデータを渡して初期化！
        Enemytester tester = newEnemy.GetComponent<Enemytester>();
        if (tester != null)
        {
            tester.Init(selectedData);
        }
    }

    private void Start()
    {
        // ゲーム開始時にランダムな敵をスポーン
        SpawnRandomEnemy();
    }

}