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

    public SpriteRenderer playerSpriteRenderer;
    public Sprite playerCloseAttackSprite;
    public Sprite playerLongAttackSprite;

    public TextMeshProUGUI playerHpText;
    public TextMeshProUGUI enemyHpText;
    public TextMeshProUGUI battleLogText;
    public TextMeshProUGUI playerActionText;
    public TextMeshProUGUI enemyActionText;
    public TextMeshProUGUI bossInfoText;
    public BattleState currentState = BattleState.Ongoing;
    public GameObject restartButton; 

    private bool isProcessingTurn = false;
    private bool waitingForClick = false;
    private Queue<BossEntry> bossQueue;
    public IReadOnlyList<SkillData> availableSkills;
    public GameObject skillButtonPrefab;
    public Transform skillButtonParent;
    public BlackCover blackCover;
    private Dictionary<SkillData, int> skillAP = new Dictionary<SkillData, int>();
    private List<BattleSkillButton> skillButtons = new List<BattleSkillButton>();
    private Sprite playerIdleSprite;
    private Vector3 playerIdleScale;

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
        AddLog("スキルを選んでください");
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
        SetSkillButtonsInteractable(false);

        if (enemy == null)
        {
            isProcessingTurn = false;
            yield break;
        }

        EnemySkillData enemySkill = enemy.UseSkillForBattle();
        if (enemySkill == null)
        {
            AddLog("敵は技を出せなかった(PP切れ)", true);
            yield return StartCoroutine(WaitForClick());

            AttackEnemyOnlyWithLog();
            yield return StartCoroutine(WaitForClick());
            ShowPlayerIdlePose();

            CheckBattleEnd();
            UpdateHpUI();
            isProcessingTurn = false;
            if (currentState == BattleState.Ongoing)
            {
                AddLog("スキルを選んでください");
                SetSkillButtonsInteractable(true);
                playerActionText.text = "";
                enemyActionText.text = "";
            }
            yield break;
        }

        DistanceType enemyDistance = EnemyConverter.ToDistanceType(enemySkill.skillType);
        AttributeType enemyAttackAttribute = EnemyConverter.ToAttributeType(enemySkill.skillElement);

        List<AttributeType> enemyAttributes = GetEnemyAttributes();

        TurnOrder turnOrder = new TurnOrder();
        bool isPlayerFirst = turnOrder.IsPlayerFirst(playerSkill.distance, enemyDistance);

        AddLog(isPlayerFirst ? "プレイヤーが先制した！" : "敵が先制した！", true);
        yield return StartCoroutine(WaitForClick());

        DamageCalculator calculator = new DamageCalculator();
        calculator.arribute = arribute;

        if (isPlayerFirst)
        {
            ShowPlayerAttackPose(playerSkill.distance);
            float damageToEnemy = calculator.CalculateDamage(playerSkill, enemyAttributes, enemyDistance, out string playerEffect);
            enemy.TakeDamage((int)damageToEnemy);
            string playerMsg = $"プレイヤー: {playerSkill.SkillName}！ {(int)damageToEnemy}ダメージ \n{playerEffect}";
            Debug.Log(playerMsg);
            AddPlayerActionLog(playerMsg);
            UpdateHpUI();
            yield return StartCoroutine(WaitForClick());
            ShowPlayerIdlePose();

            if (!IsEnemyDead())
            {
                enemy.ShowAttackPose();
                List<AttributeType> playerAttributes = new List<AttributeType> { playerSkill.attribute };
                float damageToPlayer = calculator.CalculateDamage(enemyAttackAttribute, enemyDistance, enemySkill.Damage, playerAttributes, playerSkill.distance, out string enemyEffect);
                playerHp -= (int)damageToPlayer;
                string enemyMsg = $"敵: {enemySkill.SkillName}！ {(int)damageToPlayer}ダメージ \n{enemyEffect}";
                Debug.Log(enemyMsg);
                AddEnemyActionLog(enemyMsg);
                UpdateHpUI();
                yield return StartCoroutine(WaitForClick());
                enemy.ShowIdlePose();
            }
        }
        else
        {
            enemy.ShowAttackPose();
            List<AttributeType> playerAttributesForEnemyAttack = new List<AttributeType> { playerSkill.attribute };
            float damageToPlayer = calculator.CalculateDamage(enemyAttackAttribute, enemyDistance, enemySkill.Damage, playerAttributesForEnemyAttack, playerSkill.distance, out string enemyEffect);
            playerHp -= (int)damageToPlayer;
            string enemyMsg = $"敵: {enemySkill.SkillName}！ {(int)damageToPlayer}ダメージ \n{enemyEffect}";
            Debug.Log(enemyMsg);
            AddEnemyActionLog(enemyMsg);
            UpdateHpUI();
            yield return StartCoroutine(WaitForClick());
            enemy.ShowIdlePose();

            if (playerHp > 0)
            {
                ShowPlayerAttackPose(playerSkill.distance);
                float damageToEnemy = calculator.CalculateDamage(playerSkill, enemyAttributes, enemyDistance, out string playerEffect);
                enemy.TakeDamage((int)damageToEnemy);
                string playerMsg = $"プレイヤー: {playerSkill.SkillName}！ {(int)damageToEnemy}ダメージ \n{playerEffect}";
                Debug.Log(playerMsg);
                AddPlayerActionLog(playerMsg);
                UpdateHpUI();
                yield return StartCoroutine(WaitForClick());
                ShowPlayerIdlePose();
            }
        }

        CheckBattleEnd();
        UpdateHpUI();
        isProcessingTurn = false;
        if (currentState == BattleState.Ongoing)
        {
            AddLog("スキルを選んでください");
            SetSkillButtonsInteractable(true);
            playerActionText.text = "";
            enemyActionText.text = "";
        }
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

        ShowPlayerAttackPose(playerSkill.distance);
        float damageToEnemy = calculator.CalculateDamage(playerSkill, enemyAttributes, enemyDistance, out string effect);
        enemy.TakeDamage((int)damageToEnemy);
        string msg = $"プレイヤー: {playerSkill.SkillName}！ {(int)damageToEnemy}ダメージ \n{effect}";
        Debug.Log(msg);
        AddPlayerActionLog(msg);
        UpdateHpUI();
    }

    // 攻撃時に攻撃ポーズの画像に切り替える(PPUが違う画像でも見た目のサイズが変わらないよう補正する)
    private void ShowPlayerAttackPose(DistanceType distance)
    {
        if (playerSpriteRenderer == null) return;

        if (playerIdleSprite == null)
        {
            playerIdleSprite = playerSpriteRenderer.sprite;
            playerIdleScale = playerSpriteRenderer.transform.localScale;
        }

        Sprite attackSprite = distance == DistanceType.Ranged ? playerLongAttackSprite : playerCloseAttackSprite;
        if (attackSprite == null) return;

        if (playerIdleSprite != null && playerIdleSprite.pixelsPerUnit > 0)
        {
            float ratio = attackSprite.pixelsPerUnit / playerIdleSprite.pixelsPerUnit;
            playerSpriteRenderer.transform.localScale = playerIdleScale * ratio;
        }

        playerSpriteRenderer.sprite = attackSprite;
    }

    // 通常の画像に戻す
    private void ShowPlayerIdlePose()
    {
        if (playerSpriteRenderer == null || playerIdleSprite == null) return;

        playerSpriteRenderer.sprite = playerIdleSprite;
        playerSpriteRenderer.transform.localScale = playerIdleScale;
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

            blackCover.ShowBlackCover();
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
            skillButton.Setup(skill, SelectPlayerSkill);
            skillButtons.Add(skillButton);
        }
    }

    // 自分のターンが始まるまで(演出を見終わるまで)はスキルボタンを押せないようにする
    private void SetSkillButtonsInteractable(bool interactable)
    {
        foreach (BattleSkillButton skillButton in skillButtons)
        {
            if (skillButton != null)
            {
                skillButton.SetInteractable(interactable);
            }
        }
    }

    
    public void RestartBattle()
    {
        if (currentState == BattleState.Win)
        {
            // クリア後の「もう一度」はスキル選択シーンへ
            SceneManager.LoadScene("Skill selection");
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void AddLog(string message, bool showClickHint = false)
    {
        battleLogText.text = showClickHint ? message + "\n(クリックで進む)" : message;
    }

    private void AddPlayerActionLog(string message)
    {
        playerActionText.text = message;
    }

    private void AddEnemyActionLog(string message)
    {
        enemyActionText.text = message;
    }
    
}