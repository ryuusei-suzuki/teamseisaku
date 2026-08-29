using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


public class TrialBattleManager : MonoBehaviour
{
    [Header("バトル対象")]
    public Enemytester enemy;            // Tutorial Sceneに配置済みのEnemyを直接アサイン
    public Arrribute arribute;           // 相性テーブル(Arrribute Compatibility)

    public EnemyData trialEnemyData;
    public Element mainElement;
    public Element subElement; // 複属性にしない場合はNoneのままでOK

    [Header("プレイヤー")]
    public int maxPlayerHp = 100;
    private int playerHp;
    

    [Header("試練用の固定スキル(火弱近・水弱近・風弱近・水弱遠・ヒールの5種)")]
    public List<SkillData> trialSkills;
    public GameObject skillButtonPrefab;
    public Transform skillButtonParent;

    [Header("UI")]
    public TextMeshProUGUI playerHpText;
    public TextMeshProUGUI enemyHpText;
    public TextMeshProUGUI logText;

    [Header("遷移先")]
    
    public string nextSceneName;

    
    public bool autoStart = false;

    public bool IsFinished { get; private set; }

    private bool isProcessingTurn = false;
    private readonly List<BattleSkillButton> skillButtons = new List<BattleSkillButton>();

    void Start()
    {
        InitEnemyIfNeeded();

        playerHp = maxPlayerHp;
        UpdateHpUI();

        if (autoStart)
        {
            StartBattle();
        }
    }

    // EnemyGameObjectがまだ誰にもInitされていない(HP0のまま)場合、ここで初期化する
    private void InitEnemyIfNeeded()
    {
        if (enemy == null || trialEnemyData == null) return;

        if (enemy.enemyData == null || enemy.MaxEnemyHP <= 0)
        {
            enemy.Init(trialEnemyData, mainElement, subElement);
        }
    }

    public void StartBattle()
    {
        IsFinished = false;
        CreateSkillButtons();
    }

    private void CreateSkillButtons()//スキルボタンを作るやつ
    {
        foreach (BattleSkillButton existing in skillButtons)
        {
            if (existing != null)
            {
                Destroy(existing.gameObject);
            }
        }
        skillButtons.Clear();

        if (trialSkills == null) return;

        foreach (SkillData skill in trialSkills)
        {
            if (skill == null)
            {
                Debug.LogWarning("umete");
                continue;
            }

            GameObject buttonObj = Instantiate(skillButtonPrefab, skillButtonParent);
            BattleSkillButton skillButton = buttonObj.GetComponent<BattleSkillButton>();
            skillButton.Setup(skill, SelectSkill);
            skillButtons.Add(skillButton);
        }
    }

    // スキルボタンのクリックから呼ばれる
    public void SelectSkill(SkillData skill)
    {
        if (isProcessingTurn || IsFinished || enemy == null)
            return;

        StartCoroutine(ExecuteTurn(skill));
    }

    private IEnumerator WaitForClick()
    {
        yield return null;

        while (!(Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame))
        {
            yield return null;
        }
    }

    private IEnumerator ExecuteTurn(SkillData playerSkill)
    {
        isProcessingTurn = true;

        EnemySkillData enemySkill = enemy.UseSkillForBattle();

        if (enemySkill == null)
        {
            AddLog("敵は技を出せなかった(PP切れ)");
            yield return StartCoroutine(WaitForClick());

            AttackEnemyOnly(playerSkill);
            yield return StartCoroutine(WaitForClick());

            isProcessingTurn = false;
            CheckEnemyDefeated();
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
            AddLog($"プレイヤー: {playerSkill.SkillName}！ {damageToEnemy}ダメージ \n{playerEffect}");
            UpdateHpUI();
            yield return StartCoroutine(WaitForClick());

            if (enemy.NowEnemyHP > 0)
            {
                List<AttributeType> playerAttributes = new List<AttributeType> { playerSkill.attribute };
                float damageToPlayer = calculator.CalculateDamage(enemyAttackAttribute, enemyDistance, enemySkill.Damage, playerAttributes, playerSkill.distance, out string enemyEffect);
                playerHp = Mathf.Max(0, playerHp - (int)damageToPlayer);
                AddLog($"敵: {enemySkill.SkillName}！ {damageToPlayer}ダメージ \n{enemyEffect}");
                UpdateHpUI();
                yield return StartCoroutine(WaitForClick());
            }
        }
        else
        {
            List<AttributeType> playerAttributesForEnemyAttack = new List<AttributeType> { playerSkill.attribute };
            float damageToPlayer = calculator.CalculateDamage(enemyAttackAttribute, enemyDistance, enemySkill.Damage, playerAttributesForEnemyAttack, playerSkill.distance, out string enemyEffect);
            playerHp = Mathf.Max(0, playerHp - (int)damageToPlayer);
            AddLog($"敵: {enemySkill.SkillName}！ {damageToPlayer}ダメージ \n{enemyEffect}");
            UpdateHpUI();
            yield return StartCoroutine(WaitForClick());

            float damageToEnemy = calculator.CalculateDamage(playerSkill, enemyAttributes, enemyDistance, out string playerEffect);
            enemy.TakeDamage((int)damageToEnemy);
            AddLog($"プレイヤー: {playerSkill.SkillName}！ {damageToEnemy}ダメージ \n{playerEffect}");
            UpdateHpUI();
            yield return StartCoroutine(WaitForClick());
        }

        isProcessingTurn = false;
        CheckEnemyDefeated();
    }

    private void AttackEnemyOnly(SkillData playerSkill)
    {
        List<AttributeType> enemyAttributes = GetEnemyAttributes();
        DistanceType enemyDistance = EnemyConverter.ToDistanceType(SkillType.CloseWeak);

        DamageCalculator calculator = new DamageCalculator();
        calculator.arribute = arribute;

        float damageToEnemy = calculator.CalculateDamage(playerSkill, enemyAttributes, enemyDistance, out string effect);
        enemy.TakeDamage((int)damageToEnemy);
        AddLog($"プレイヤー: {playerSkill.SkillName}！ {damageToEnemy}ダメージ \n{effect}");
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

    // 敵のHPが0になっていたら、ログを出してから次のシーンへ遷移する
    private void CheckEnemyDefeated()
    {
        if (IsFinished) return;
        if (enemy == null || enemy.NowEnemyHP > 0) return;

        StartCoroutine(FinishBattle());
    }

    private IEnumerator FinishBattle()
    {
        IsFinished = true;

        string enemyName = enemy.enemyData != null ? enemy.enemyData.EnemyName : "敵";
        AddLog($"{enemyName} を倒した！");
        yield return StartCoroutine(WaitForClick());

        GoToNextScene();
    }

    private void GoToNextScene()
    {
        if (string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogWarning("TrialBattleManager: nextSceneNameが設定されていないため遷移をスキップしました。");
            return;
        }

        // TODO: タイトル→TTRと同じ矢印ワイプ演出に差し替える(今は仮でシーンを直接ロード)
        SceneManager.LoadScene(nextSceneName);
    }

    private void UpdateHpUI()
    {
        if (playerHpText != null)
        {
            playerHpText.text = "プレイヤーHP: " + playerHp;
        }

        if (enemyHpText != null)
        {
            enemyHpText.text = enemy != null ? "敵HP: " + enemy.NowEnemyHP : "敵HP: -";
        }
    }

    private void AddLog(string message)
    {
        if (logText != null)
        {
            logText.text = message;
        }
    }
}