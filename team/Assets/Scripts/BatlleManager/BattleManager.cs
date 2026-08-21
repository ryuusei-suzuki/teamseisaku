using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public enum BattleState { Ongoing, Win, Lose }

    public int maxPlayerHp = 100;
    public int playerHp;
    public SkillData playerSkill;
    public Arrribute arribute;
    public Enemytester enemy;
    public EnemySpawner spawner;
    public List<EnemyData> bossList; // ヒトカゲ・ゼニガメ・フシギダネをInspectorで割り当てる

    public TextMeshProUGUI playerHpText;
    public TextMeshProUGUI enemyHpText;
    public TextMeshProUGUI battleLogText;
    public BattleState currentState = BattleState.Ongoing;

    [SerializeField] private float messageWaitTime = 1.0f;
    private bool isProcessingTurn = false;
    private Queue<EnemyData> bossQueue;

    void Start()
    {
        playerHp = maxPlayerHp;
        SetupBossQueue();
        SpawnNextBoss();
        UpdateHpUI();
    }

    private void SetupBossQueue()
    {
        List<EnemyData> shuffled = new List<EnemyData>(bossList);
        for (int i = 0; i < shuffled.Count; i++)
        {
            int rand = Random.Range(i, shuffled.Count);
            (shuffled[i], shuffled[rand]) = (shuffled[rand], shuffled[i]);
        }
        bossQueue = new Queue<EnemyData>(shuffled);
    }

    private void SpawnNextBoss()
    {
        if (bossQueue.Count == 0)
        {
            currentState = BattleState.Win;
            AddLog("全てのボスを倒した！クリア！");
            return;
        }

        EnemyData nextBoss = bossQueue.Dequeue();
        enemy = spawner.SpawnSpecificEnemy(nextBoss);
        currentState = BattleState.Ongoing;
        UpdateHpUI();
    }

    private void HealPlayer(float percent)
    {
        int healAmount = Mathf.RoundToInt(maxPlayerHp * percent);
        playerHp = Mathf.Min(maxPlayerHp, playerHp + healAmount);
    }

    public void SelectPlayerSkill(SkillData skill)
    {
        if (isProcessingTurn || currentState != BattleState.Ongoing)
            return;

        playerSkill = skill;
        Debug.Log("選択した技: " + skill.SkillName);
        StartCoroutine(ExecuteTurn());
    }

    private IEnumerator ExecuteTurn()
    {
        isProcessingTurn = true;

        if (enemy == null)
        {
            isProcessingTurn = false;
            yield break;
        }

        EnemySkillData enemySkill = enemy.UseSkillForBattle();
        if (enemySkill == null)
        {
            AddLog("敵は技を出せなかった(PP切れ)");
            yield return new WaitForSeconds(messageWaitTime);

            AttackEnemyOnlyWithLog();
            yield return new WaitForSeconds(messageWaitTime);

            CheckBattleEnd();
            UpdateHpUI();
            isProcessingTurn = false;
            yield break;
        }

        DistanceType enemyDistance = EnemyConverter.ToDistanceType(enemySkill.skillType);
        AttributeType enemyAttackAttribute = EnemyConverter.ToAttributeType(enemySkill.skillElement);

        List<AttributeType> enemyAttributes = GetEnemyAttributes();

        TurnOrder turnOrder = new TurnOrder();
        bool isPlayerFirst = turnOrder.IsPlayerFirst(playerSkill.distance, enemyDistance);

        DamageCalculator calculator = new DamageCalculator();
        calculator.arribute = arribute;

        if (isPlayerFirst)
        {
            float damageToEnemy = calculator.CalculateDamage(playerSkill, enemyAttributes, enemyDistance, out string playerEffect);
            enemy.TakeDamage((int)damageToEnemy);
            string playerMsg = $"プレイヤー: {playerSkill.SkillName}！ {damageToEnemy}ダメージ {playerEffect}";
            Debug.Log(playerMsg);
            AddLog(playerMsg);
            UpdateHpUI();
            yield return new WaitForSeconds(messageWaitTime);

            if (!IsEnemyDead())
            {
                List<AttributeType> playerAttributes = new List<AttributeType> { playerSkill.attribute };
                float damageToPlayer = calculator.CalculateDamage(enemyAttackAttribute, enemyDistance, enemySkill.Damage, playerAttributes, playerSkill.distance, out string enemyEffect);
                playerHp -= (int)damageToPlayer;
                string enemyMsg = $"敵: {enemySkill.SkillName}！ {damageToPlayer}ダメージ {enemyEffect}";
                Debug.Log(enemyMsg);
                AddLog(enemyMsg);
                UpdateHpUI();
                yield return new WaitForSeconds(messageWaitTime);
            }
        }
        else
        {
            List<AttributeType> playerAttributesForEnemyAttack = new List<AttributeType> { playerSkill.attribute };
            float damageToPlayer = calculator.CalculateDamage(enemyAttackAttribute, enemyDistance, enemySkill.Damage, playerAttributesForEnemyAttack, playerSkill.distance, out string enemyEffect);
            playerHp -= (int)damageToPlayer;
            string enemyMsg = $"敵: {enemySkill.SkillName}！ {damageToPlayer}ダメージ {enemyEffect}";
            Debug.Log(enemyMsg);
            AddLog(enemyMsg);
            UpdateHpUI();
            yield return new WaitForSeconds(messageWaitTime);

            if (playerHp > 0)
            {
                float damageToEnemy = calculator.CalculateDamage(playerSkill, enemyAttributes, enemyDistance, out string playerEffect);
                enemy.TakeDamage((int)damageToEnemy);
                string playerMsg = $"プレイヤー: {playerSkill.SkillName}！ {damageToEnemy}ダメージ {playerEffect}";
                Debug.Log(playerMsg);
                AddLog(playerMsg);
                UpdateHpUI();
                yield return new WaitForSeconds(messageWaitTime);
            }
        }

        CheckBattleEnd();
        UpdateHpUI();
        isProcessingTurn = false;
    }

    private void UpdateHpUI()
    {
        playerHpText.text = "プレイヤーHP: " + playerHp;

        if (enemy != null)
        {
            enemyHpText.text = "敵HP: " + enemy.NowEnemyHP;
        }
        else
        {
            enemyHpText.text = "敵HP: -";
        }
    }

    private void AttackEnemyOnlyWithLog()
    {
        List<AttributeType> enemyAttributes = GetEnemyAttributes();
        DistanceType enemyDistance = EnemyConverter.ToDistanceType(SkillType.CloseWeak);

        DamageCalculator calculator = new DamageCalculator();
        calculator.arribute = arribute;

        float damageToEnemy = calculator.CalculateDamage(playerSkill, enemyAttributes, enemyDistance, out string effect);
        enemy.TakeDamage((int)damageToEnemy);
        string msg = $"プレイヤー: {playerSkill.SkillName}！ {damageToEnemy}ダメージ {effect}";
        Debug.Log(msg);
        AddLog(msg);
        UpdateHpUI();
    }
    private List<AttributeType> GetEnemyAttributes()
    {
        List<AttributeType> list = new List<AttributeType>
        {
            EnemyConverter.ToAttributeType(enemy.MainEnemyElement)
        };
        if (enemy.SubEnemyElement != EnemyElement.None)
        {
            list.Add(EnemyConverter.ToAttributeType(enemy.SubEnemyElement));
        }
        return list;
    }

    private bool IsEnemyDead()
    {
        return enemy == null || enemy.NowEnemyHP <= 0;
    }

    private void CheckBattleEnd()
    {
        if (playerHp <= 0)
        {
            currentState = BattleState.Lose;
            AddLog("敗北...");
            return;
        }

        if (IsEnemyDead())
        {
            if (bossQueue.Count == 0)
            {
                currentState = BattleState.Win;
                AddLog("全てのボスを倒した！クリア！");
            }
            else
            {
                HealPlayer(0.3f);
                AddLog("ボスを倒した！HPが30%回復した");
                SpawnNextBoss();
            }
        }
    }

    private void AddLog(string message)
    {
        battleLogText.text = message;
    }
}