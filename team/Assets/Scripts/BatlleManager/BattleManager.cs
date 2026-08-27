using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public enum BattleState { Ongoing, Win, Lose }

    private class BossEntry
    {
        public EnemyData data;
        public Element subElement;
    }

    public int maxPlayerHp = 100;
    public int playerHp;
    public SkillData playerSkill;
    public Arrribute arribute;
    public Enemytester enemy;
    public EnemySpawner spawner;
    public List<EnemyData> bossList;

    public TextMeshProUGUI playerHpText;
    public TextMeshProUGUI enemyHpText;
    public TextMeshProUGUI battleLogText;
    public TextMeshProUGUI bossInfoText;
    public BattleState currentState = BattleState.Ongoing;
    public GameObject restartButton; 

    private bool isProcessingTurn = false;
    private bool waitingForClick = false;
    private Queue<BossEntry> bossQueue;
    public IReadOnlyList<SkillData> availableSkills;
    public GameObject skillButtonPrefab;
    public Transform skillButtonParent;
    private Dictionary<SkillData, int> skillAP = new Dictionary<SkillData, int>();
    private List<BattleSkillButton> skillButtons = new List<BattleSkillButton>();
    void Start()
    {
        if (SkillSelectionManager.Instance != null)
        {
            availableSkills = SkillSelectionManager.Instance.SelectedSkills;
        }
        playerHp = maxPlayerHp;
        restartButton.SetActive(false); 
        SetupBossQueue();
        ShowBossInfo();
        SpawnNextBoss();
        UpdateHpUI();
        InitializeSkillAP();
        CreateSkillButtons();
    }


    private void InitializeSkillAP()
    {
        skillAP.Clear();
        if (availableSkills == null) return;

        foreach (SkillData skill in availableSkills)
        {
            skillAP[skill] = skill.AP;
        }
    }

    

    private void SetupBossQueue()
    {
        bossQueue = new Queue<BossEntry>();

        if (BossPreviewData.bossPlan != null && BossPreviewData.bossPlan.Count > 0)
        {
            foreach (BossPlanEntry planEntry in BossPreviewData.bossPlan)
            {
                BossEntry entry = new BossEntry();
                entry.data = planEntry.data;
                entry.subElement = planEntry.subElement;
                bossQueue.Enqueue(entry);
            }
            BossPreviewData.bossPlan = null; // 使い終わったらクリア
        }
        else
        {
            // SkillSceneを経由しない単体テスト用
            List<EnemyData> shuffled = new List<EnemyData>(bossList);
            for (int i = 0; i < shuffled.Count; i++)
            {
                int rand = Random.Range(i, shuffled.Count);
                (shuffled[i], shuffled[rand]) = (shuffled[rand], shuffled[i]);
            }
            foreach (EnemyData data in shuffled)
            {
                BossEntry entry = new BossEntry();
                entry.data = data;
                entry.subElement = spawner.DetermineSubElement(data.enemyElement);
                bossQueue.Enqueue(entry);
            }
        }
    }

    private void ShowBossInfo()
    {
        string info = "出現するボス:\n";
        foreach (BossEntry entry in bossQueue)
        {
            string sub = entry.subElement != null ? ElementToJapanese(entry.subElement.enemyElement) : "なし";
            info += $"{entry.data.EnemyName}(主属性: {ElementToJapanese(entry.data.enemyElement)} / 副属性: {sub})\n";
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

    private void SpawnNextBoss()
    {
        if (bossQueue.Count == 0)
        {
            currentState = BattleState.Win;
            AddLog("全てのボスを倒した！クリア！");
            return;
        }

        BossEntry nextBoss = bossQueue.Dequeue();
        enemy = spawner.SpawnSpecificEnemy(nextBoss.data, nextBoss.subElement);
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

        Debug.Log(skill.SkillName + " の残りAP: " + (skillAP.ContainsKey(skill) ? skillAP[skill].ToString() : "辞書に存在しない"));

        if (!HasAP(skill))
        {
            string msg = skill.SkillName + " はAPがない！";
            Debug.Log(msg);
            AddLog(msg);
            return;
        }

        skillAP[skill]--;
      

        playerSkill = skill;
        Debug.Log("選択した技: " + skill.SkillName);
        StartCoroutine(ExecuteTurn());
    }
    private bool HasAP(SkillData skill)
    {
        return skillAP.ContainsKey(skill) && skillAP[skill] > 0;
    }
    private IEnumerator WaitForClick()
    {
        waitingForClick = true;
        yield return null;

        while (!(Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame))
        {
            yield return null;
        }

        waitingForClick = false;
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
            yield return StartCoroutine(WaitForClick());

            AttackEnemyOnlyWithLog();
            yield return StartCoroutine(WaitForClick());

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
            string playerMsg = $"プレイヤー: {playerSkill.SkillName}！ {damageToEnemy}ダメージ \n{playerEffect}";
            Debug.Log(playerMsg);
            AddLog(playerMsg);
            UpdateHpUI();
            yield return StartCoroutine(WaitForClick());

            if (!IsEnemyDead())
            {
                List<AttributeType> playerAttributes = new List<AttributeType> { playerSkill.attribute };
                float damageToPlayer = calculator.CalculateDamage(enemyAttackAttribute, enemyDistance, enemySkill.Damage, playerAttributes, playerSkill.distance, out string enemyEffect);
                playerHp -= (int)damageToPlayer;
                string enemyMsg = $"敵: {enemySkill.SkillName}！ {damageToPlayer}ダメージ \n{enemyEffect}";
                Debug.Log(enemyMsg);
                AddLog(enemyMsg);
                UpdateHpUI();
                yield return StartCoroutine(WaitForClick());
            }
        }
        else
        {
            List<AttributeType> playerAttributesForEnemyAttack = new List<AttributeType> { playerSkill.attribute };
            float damageToPlayer = calculator.CalculateDamage(enemyAttackAttribute, enemyDistance, enemySkill.Damage, playerAttributesForEnemyAttack, playerSkill.distance, out string enemyEffect);
            playerHp -= (int)damageToPlayer;
            string enemyMsg = $"敵: {enemySkill.SkillName}！ {damageToPlayer}ダメージ \n{enemyEffect}";
            Debug.Log(enemyMsg);
            AddLog(enemyMsg);
            UpdateHpUI();
            yield return StartCoroutine(WaitForClick());

            if (playerHp > 0)
            {
                float damageToEnemy = calculator.CalculateDamage(playerSkill, enemyAttributes, enemyDistance, out string playerEffect);
                enemy.TakeDamage((int)damageToEnemy);
                string playerMsg = $"プレイヤー: {playerSkill.SkillName}！ {damageToEnemy}ダメージ \n{playerEffect}";
                Debug.Log(playerMsg);
                AddLog(playerMsg);
                UpdateHpUI();
                yield return StartCoroutine(WaitForClick());
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
        string msg = $"プレイヤー: {playerSkill.SkillName}！ {damageToEnemy}ダメージ \n{effect}";
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
            restartButton.SetActive(true);
            return;
        }

        if (IsEnemyDead())
        {
            if (bossQueue.Count == 0)
            {
                currentState = BattleState.Win;
                AddLog("全てのボスを倒した！クリア！");
                restartButton.SetActive(true);
            }
            else
            {
                HealPlayer(0.3f);
                AddLog("ボスを倒した！\nHPが30%回復した");
                SpawnNextBoss();
            }
        }
    }
    private void CreateSkillButtons()
    {
        if (availableSkills == null) return;

        skillButtons.Clear();
        foreach (SkillData skill in availableSkills)
        {
            GameObject buttonObj = Instantiate(skillButtonPrefab, skillButtonParent);
            BattleSkillButton skillButton = buttonObj.GetComponent<BattleSkillButton>();
            skillButton.Setup(skill, this);
            skillButtons.Add(skillButton);
        }
    }

    
    public void RestartBattle()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void AddLog(string message)
    {
        battleLogText.text = message;
    }
    
}