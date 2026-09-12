using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// 試練の間(TTR)専用の簡易バトルループ。
// 固定5種スキル・敵1体のみを想定していて、BattleManagerのボス連戦/AP管理は持たない。
// 敵を倒したらログを表示し、クリック待ちしてから次のシーン(スキル選択画面)へ遷移する。
public class TrialBattleManager : MonoBehaviour
{
    [Header("バトル対象")]
    public Enemytester enemy;            // Tutorial Sceneに配置済みのEnemyを直接アサイン
    public Arrribute arribute;           // 相性テーブル(Arrribute Compatibility)

    [Header("敵の初期化データ(EnemyGameObjectがまだInitされていない場合はここで初期化する)")]
    [Tooltip("試練の敵として使うEnemyDataを指定してください。指定するとStart()時にenemy.Init()を自動で呼び、HPやスプライトを設定します。すでに別の場所でInit済みならNoneのままでOKです。")]
    public EnemyData trialEnemyData;
    public Element mainElement;
    public Element subElement; // 複属性にしない場合はNoneのままでOK

    [Header("プレイヤー")]
    public int maxPlayerHp = 100;
    private int playerHp;
    // 注:試練の敵は倒される想定の弱敵のため、敗北時の処理(リトライなど)はまだ実装していません。
    // 必要になったらBattleManager.CheckBattleEndのLose分岐を参考に追加してください。

    [Header("試練用の固定スキル(火弱近・水弱近・風弱近・水弱遠・ヒールの5種)")]
    public List<SkillData> trialSkills;
    public GameObject skillButtonPrefab;
    public Transform skillButtonParent;

    [Header("UI")]
    public TextMeshProUGUI playerHpText;
    public TextMeshProUGUI enemyHpText;
    public TextMeshProUGUI logText;

    [Header("遷移先")]
    [Tooltip("敵を倒した後にロードするシーン名。演出(矢印ワイプ)は未実装で、今はここに設定したシーンへ即ロードします。")]
    public string nextSceneName;

    [Tooltip("trueの場合、このコンポーネントのStart()時点で自動的にスキルボタンを生成してバトルを開始します。TextWriter側から開始タイミングを制御したい場合はfalseにしてStartBattle()を呼んでください。")]
    public bool autoStart = true;

    public bool IsFinished { get; private set; }

    private bool isProcessingTurn = false;
    private readonly List<BattleSkillButton> skillButtons = new List<BattleSkillButton>();

    // 敵を倒したかどうかのフラグ。
    // enemy.TakeDamage()の内部でHP0になった瞬間にDestroy(gameObject)されるため、
    // WaitForClickで数フレーム待った後に enemy.NowEnemyHP や enemy == null を読み直すと
    // 「最初から敵が設定されていない」場合と区別がつかなくなり、撃破を検知できない。
    // そのため、ダメージを与えた"その場"(敵がまだ壊れていない同一フレーム内)で撃破判定を確定させ、
    // ここに保存しておく。
    private bool pendingEnemyDefeat = false;
    private string cachedEnemyName = "敵";

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
        pendingEnemyDefeat = false;
        CreateSkillButtons();
    }

    private void CreateSkillButtons()
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
                Debug.LogWarning("TrialBattleManager: trialSkillsに未設定(None)の要素があるためスキップしました。Inspectorで全て埋めてください。");
                continue;
            }

            GameObject buttonObj = Instantiate(skillButtonPrefab, skillButtonParent);
            BattleSkillButton skillButton = buttonObj.GetComponent<BattleSkillButton>();
            skillButton.Setup(skill, SelectSkill);
            skillButtons.Add(skillButton);
        }
    }

    // スキルボタンのクリックから呼ばれる(BattleManager.SelectPlayerSkillに相当)
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
        pendingEnemyDefeat = false;

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
        bool isPlayerFirst = playerSkill.skillType == SkillType.Guard
            ? true
            : turnOrder.IsPlayerFirst(playerSkill.distance, enemyDistance);

        DamageCalculator calculator = new DamageCalculator();
        calculator.arribute = arribute;

        if (isPlayerFirst)
        {
            float damageToEnemy = calculator.CalculateDamage(playerSkill, enemyAttributes, enemyDistance, out string playerEffect);
            ApplyDamageToEnemy(playerSkill, damageToEnemy, playerEffect);
            yield return StartCoroutine(WaitForClick());

            // 直前の攻撃で倒していなければ、敵の反撃を処理する
            if (!pendingEnemyDefeat)
            {
                List<AttributeType> playerAttributes = new List<AttributeType> { playerSkill.attribute };
                float damageToPlayer = calculator.CalculateDamage(enemyAttackAttribute, enemyDistance, enemySkill.Damage, playerAttributes, playerSkill.distance, out string enemyEffect);
                damageToPlayer = ApplyPlayerDamageReduction(damageToPlayer, playerSkill);
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
            damageToPlayer = ApplyPlayerDamageReduction(damageToPlayer, playerSkill);
            playerHp = Mathf.Max(0, playerHp - (int)damageToPlayer);
            AddLog($"敵: {enemySkill.SkillName}！ {damageToPlayer}ダメージ \n{enemyEffect}");
            UpdateHpUI();
            yield return StartCoroutine(WaitForClick());

            float damageToEnemy = calculator.CalculateDamage(playerSkill, enemyAttributes, enemyDistance, out string playerEffect);
            ApplyDamageToEnemy(playerSkill, damageToEnemy, playerEffect);
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
        ApplyDamageToEnemy(playerSkill, damageToEnemy, effect);
    }

    // 敵にダメージを与える共通処理。
    // enemy.TakeDamage()がHP0になった瞬間に敵を破壊する可能性があるため、
    // 破壊されるより前の"今このフレーム"のうちに撃破判定をpendingEnemyDefeatへ保存しておく。
    private void ApplyDamageToEnemy(SkillData playerSkill, float damageToEnemy, string effect)
    {
        if (enemy != null && enemy.enemyData != null)
        {
            cachedEnemyName = enemy.enemyData.EnemyName;
        }

        enemy.TakeDamage((int)damageToEnemy);

        if (enemy == null || enemy.NowEnemyHP <= 0)
        {
            pendingEnemyDefeat = true;
        }

        AddLog($"プレイヤー: {playerSkill.SkillName}！ {damageToEnemy}ダメージ \n{effect}");
        UpdateHpUI();
    }

    // ガード使用時: 相手の攻撃を完全に無効化(0ダメージ)
    // 近距離の弱攻撃(CloseWeak)使用時: 受けるダメージを0.5倍に軽減
    private float ApplyPlayerDamageReduction(float damageToPlayer, SkillData skill)
    {
        if (skill.skillType == SkillType.Guard)
        {
            return 0f;
        }
        if (skill.skillType == SkillType.CloseWeak)
        {
            return damageToPlayer * 0.5f;
        }
        return damageToPlayer;
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

    // 敵を倒していたら(pendingEnemyDefeat)、ログを出してから次のシーンへ遷移する
    private void CheckEnemyDefeated()
    {
        if (IsFinished) return;
        if (!pendingEnemyDefeat) return;

        StartCoroutine(FinishBattle());
    }

    private IEnumerator FinishBattle()
    {
        IsFinished = true;

        AddLog($"{cachedEnemyName} を倒した！");
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

        //  タイトル→TTRと同じ矢印ワイプ演出に差し替える(今は仮でシーンを直接ロード)
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