using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    public enum BattleState { Ongoing, Win, Lose }

    public int playerHp;
    public SkillData playerSkill;
    public Arrribute arribute;
    public Enemytester enemy;

    public TextMeshProUGUI playerHpText;
    public TextMeshProUGUI enemyHpText;
    public TextMeshProUGUI battleLogText;
    public BattleState currentState = BattleState.Ongoing;

    [SerializeField] private float messageWaitTime = 1.0f; // メッセージの表示間隔
    private bool isProcessingTurn = false; // 演出中の二重操作防止

    void Start()
    {
        UpdateHpUI();
    }

    public void SelectPlayerSkill(SkillData skill)
    {
        if (isProcessingTurn)
            return; // 演出中はボタン操作を受け付けない

        playerSkill = skill;
        Debug.Log("Selected skill: " + skill.SkillName);
        StartCoroutine(ExecuteTurn());
    }

    private IEnumerator ExecuteTurn()
    {
        if (currentState != BattleState.Ongoing)
            yield break;

        isProcessingTurn = true;

        if (enemy == null)
        {
            currentState = BattleState.Win;
            AddLog("Victory (no enemy)");
            isProcessingTurn = false;
            yield break;
        }

        EnemySkillData enemySkill = enemy.UseSkillForBattle();
        if (enemySkill == null)
        {
            AddLog("Enemy could not act (out of PP)");
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
            float damageToEnemy = calculator.CalculateDamage(playerSkill, enemyAttributes, enemyDistance);
            enemy.TakeDamage((int)damageToEnemy);
            string playerMsg = $"Player: {playerSkill.SkillName}! {damageToEnemy} damage";
            Debug.Log(playerMsg);
            AddLog(playerMsg);
            UpdateHpUI();
            yield return new WaitForSeconds(messageWaitTime);

            if (!IsEnemyDead())
            {
                List<AttributeType> playerAttributes = new List<AttributeType> { playerSkill.attribute };
                float damageToPlayer = calculator.CalculateDamage(enemyAttackAttribute, enemyDistance, enemySkill.Damage, playerAttributes, playerSkill.distance);
                playerHp -= (int)damageToPlayer;
                string enemyMsg = $"Enemy: {enemySkill.SkillName}! {damageToPlayer} damage";
                Debug.Log(enemyMsg);
                AddLog(enemyMsg);
                UpdateHpUI();
                yield return new WaitForSeconds(messageWaitTime);
            }
        }
        else
        {
            List<AttributeType> playerAttributesForEnemyAttack = new List<AttributeType> { playerSkill.attribute };
            float damageToPlayer = calculator.CalculateDamage(enemyAttackAttribute, enemyDistance, enemySkill.Damage, playerAttributesForEnemyAttack, playerSkill.distance);
            playerHp -= (int)damageToPlayer;
            string enemyMsg = $"Enemy: {enemySkill.SkillName}! {damageToPlayer} damage";
            Debug.Log(enemyMsg);
            AddLog(enemyMsg);
            UpdateHpUI();
            yield return new WaitForSeconds(messageWaitTime);

            if (playerHp > 0)
            {
                float damageToEnemy = calculator.CalculateDamage(playerSkill, enemyAttributes, enemyDistance);
                enemy.TakeDamage((int)damageToEnemy);
                string playerMsg = $"Player: {playerSkill.SkillName}! {damageToEnemy} damage";
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
        playerHpText.text = "Player HP: " + playerHp;

        if (enemy != null)
        {
            enemyHpText.text = "Enemy HP: " + enemy.NowEnemyHP;
        }
        else
        {
            enemyHpText.text = "Enemy HP: -";
        }
    }

    private void AttackEnemyOnlyWithLog()
    {
        List<AttributeType> enemyAttributes = GetEnemyAttributes();
        DistanceType enemyDistance = EnemyConverter.ToDistanceType(SkillType.CloseWeak);

        DamageCalculator calculator = new DamageCalculator();
        calculator.arribute = arribute;

        float damageToEnemy = calculator.CalculateDamage(playerSkill, enemyAttributes, enemyDistance);
        enemy.TakeDamage((int)damageToEnemy);
        string msg = $"Player: {playerSkill.SkillName}! {damageToEnemy} damage";
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
            AddLog("Defeat...");
        }
        else if (IsEnemyDead())
        {
            currentState = BattleState.Win;
            AddLog("Victory!");
        }
    }

    private void AddLog(string message)
    {
        battleLogText.text = message;
    }
    public void ExecuteTurnFromDebug()
    {
        if (isProcessingTurn)
            return;

        StartCoroutine(ExecuteTurn());
    }
}