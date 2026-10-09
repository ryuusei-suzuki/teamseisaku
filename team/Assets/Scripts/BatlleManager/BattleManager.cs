using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    [Header("攻撃時のエフェクト")]
    public GameObject playerAttackEffectPrefab;
    public float playerAttackEffectLifetime = 2f;
    [Header("回復時のエフェクト")]
    public GameObject playerHealEffectPrefab;
    public float playerHealEffectLifetime = 2f;
    [Header("ガード時のエフェクト")]
    public GameObject playerGuardEffectPrefab;
    public float playerGuardEffectLifetime = 2f;

    public TextMeshProUGUI playerHpText;
    public TextMeshProUGUI enemyHpText;
    public Image playerHpFillImage;
    public Image enemyHpFillImage;
    public TextMeshProUGUI battleLogText;
    public TextMeshProUGUI playerActionText;
    public TextMeshProUGUI enemyActionText;
    public Image playerSkillIconImage;
    public Image enemySkillIconImage;
    public TextMeshProUGUI bossInfoText;
    public BattleState currentState = BattleState.Ongoing;
    public GameObject restartButton;
    public SlideObject slideObject;

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

    // スキルのヒール用(固定量回復)
    private void HealPlayerFlat(int amount)
    {
        playerHp = Mathf.Min(maxPlayerHp, playerHp + amount);
    }

    // Guard/Healなど、必ず先制になる特殊技かどうか
    private bool IsPreemptiveSkill(SkillType skillType)
    {
        return skillType == SkillType.Guard || skillType == SkillType.Heal;
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

        // 今何が起こっているか分かりやすくするため、選択されたスキルのアイコンを画面中央(プレイヤー/敵の間)に即時表示する
        if (playerSkillIconImage != null && playerSkill != null && playerSkill.iconImage != null)
        {
            playerSkillIconImage.sprite = playerSkill.iconImage;
            playerSkillIconImage.gameObject.SetActive(true);
        }
        if (enemySkillIconImage != null)
        {
            if (enemySkill != null && enemySkill.icon != null)
            {
                enemySkillIconImage.sprite = enemySkill.icon;
                enemySkillIconImage.gameObject.SetActive(true);
            }
            else
            {
                enemySkillIconImage.gameObject.SetActive(false);
            }
        }

        DamageCalculator calculator = new DamageCalculator();
        calculator.arribute = arribute;
        List<AttributeType> enemyAttributes = GetEnemyAttributes(enemySkill);

        if (enemySkill == null)
        {
            AddLog("敵は技を出せなかった(PP切れ)", true);
            yield return StartCoroutine(WaitForClick());

            DistanceType fallbackEnemyDistance = EnemyConverter.ToDistanceType(SkillType.CloseWeak);
            yield return StartCoroutine(ExecutePlayerAction(calculator, enemyAttributes, fallbackEnemyDistance));

            CheckBattleEnd();
            UpdateHpUI();
            isProcessingTurn = false;
            if (currentState == BattleState.Ongoing)
            {
                AddLog("スキルを選んでください");
                SetSkillButtonsInteractable(true);
                playerActionText.text = "";
                enemyActionText.text = "";
                if (playerSkillIconImage != null) playerSkillIconImage.gameObject.SetActive(false);
                if (enemySkillIconImage != null) enemySkillIconImage.gameObject.SetActive(false);
            }
            yield break;
        }

        DistanceType enemyDistance = EnemyConverter.ToDistanceType(enemySkill.skillType);
        AttributeType enemyAttackAttribute = EnemyConverter.ToAttributeType(enemySkill.skillElement);

        bool playerIsPreemptive = IsPreemptiveSkill(playerSkill.skillType);
        bool enemyIsPreemptive = IsPreemptiveSkill(enemySkill.skillType);

        TurnOrder turnOrder = new TurnOrder();
        bool isPlayerFirst;
        if (playerIsPreemptive != enemyIsPreemptive)
        {
            // Guard/Healは必ず先制
            isPlayerFirst = playerIsPreemptive;
        }
        else
        {
            isPlayerFirst = turnOrder.IsPlayerFirst(playerSkill.distance, enemyDistance);
        }

        AddLog(isPlayerFirst ? "プレイヤーが先制した！" : "敵が先制した！", true);
        yield return StartCoroutine(WaitForClick());

        if (isPlayerFirst)
        {
            yield return StartCoroutine(ExecutePlayerAction(calculator, enemyAttributes, enemyDistance));
            bool playerGuarded = playerSkill.skillType == SkillType.Guard;

            if (!IsEnemyDead())
            {
                yield return StartCoroutine(ExecuteEnemyAction(calculator, enemySkill, enemyAttackAttribute, enemyDistance, playerGuarded));
            }
        }
        else
        {
            yield return StartCoroutine(ExecuteEnemyAction(calculator, enemySkill, enemyAttackAttribute, enemyDistance, false));
            bool enemyGuarded = enemySkill.skillType == SkillType.Guard;

            if (playerHp > 0)
            {
                yield return StartCoroutine(ExecutePlayerAction(calculator, enemyAttributes, enemyDistance, enemyGuarded));
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
            if (playerSkillIconImage != null) playerSkillIconImage.gameObject.SetActive(false);
            if (enemySkillIconImage != null) enemySkillIconImage.gameObject.SetActive(false);
        }
    }

    // プレイヤーの行動を実行する(通常攻撃/ガード/ヒール)
    // blockedByEnemyGuard: 敵が先にガードしていて、プレイヤーの攻撃が防がれる場合はtrue
    private IEnumerator ExecutePlayerAction(DamageCalculator calculator,List<AttributeType> enemyAttributes,DistanceType enemyDistance,bool blockedByEnemyGuard = false)
    {
        if (playerSkill.skillType == SkillType.Heal)
        {
            PlayPlayerHealEffect();
            HealPlayerFlat(40);
            string healMsg = $"プレイヤー: {playerSkill.SkillName}！ HPが40回復した";
            Debug.Log(healMsg);
            AddPlayerActionLog(healMsg);
            UpdateHpUI();
            yield return StartCoroutine(WaitForClick());
            ShowPlayerIdlePose();
        }
        else if (playerSkill.skillType == SkillType.Guard)
        {
            PlayPlayerGuardEffect();
            string guardMsg = $"プレイヤー: {playerSkill.SkillName}！ 身を守っている";
            Debug.Log(guardMsg);
            AddPlayerActionLog(guardMsg);
            yield return StartCoroutine(WaitForClick());
            ShowPlayerIdlePose();
        }
        else
        {
            ShowPlayerAttackPose(playerSkill.distance);
            PlayPlayerAttackSEAndEffect();
            float damageToEnemy = calculator.CalculateDamage(playerSkill,enemyAttributes,enemyDistance,out string playerEffect );
            if (blockedByEnemyGuard)
            {
                string blockedMsg =$"プレイヤー: {playerSkill.SkillName}！ しかし敵が防いだ！ 0ダメージ";
                Debug.Log(blockedMsg);
                AddPlayerActionLog(blockedMsg);
            }
            else
            {
                enemy.TakeDamage((int)damageToEnemy);
                string playerMsg =$"プレイヤー: {playerSkill.SkillName}！ {(int)damageToEnemy}ダメージ\n{playerEffect}";
                Debug.Log(playerMsg);
                AddPlayerActionLog(playerMsg);
            }
            UpdateHpUI();
            yield return StartCoroutine(WaitForClick());
            ShowPlayerIdlePose();
        }
    }

    // 敵の行動を実行する(通常攻撃/ガード/ヒール)
    // blockedByPlayerGuard: プレイヤーが先にガードしていて、敵の攻撃が防がれる場合はtrue
    private IEnumerator ExecuteEnemyAction(DamageCalculator calculator, EnemySkillData enemySkill, AttributeType enemyAttackAttribute, DistanceType enemyDistance, bool blockedByPlayerGuard = false)
    {
        if (enemySkill.skillType == SkillType.Heal)
        {
            enemy.ShowAttackPose(enemySkill, GetPlayerEffectPosition());
            enemy.HealSelf(40);
            string healMsg = $"敵: {enemySkill.SkillName}！ HPが40回復した";
            Debug.Log(healMsg);
            AddEnemyActionLog(healMsg);
            UpdateHpUI();
            yield return StartCoroutine(WaitForClick());
            enemy.ShowIdlePose();
        }
        else if (enemySkill.skillType == SkillType.Guard)
        {
            enemy.ShowAttackPose(enemySkill, GetPlayerEffectPosition());
            string guardMsg = $"敵: {enemySkill.SkillName}！ 身を守っている";
            Debug.Log(guardMsg);
            AddEnemyActionLog(guardMsg);
            yield return StartCoroutine(WaitForClick());
            enemy.ShowIdlePose();
        }
        else
        {
            enemy.ShowAttackPose(enemySkill, GetPlayerEffectPosition());
            List<AttributeType> playerAttributes = new List<AttributeType> { playerSkill.attribute };
            float damageToPlayer = calculator.CalculateDamage(enemyAttackAttribute, enemyDistance, enemySkill.Damage, playerAttributes, playerSkill.distance, out string enemyEffect);

            if (blockedByPlayerGuard)
            {
                string blockedMsg = $"敵: {enemySkill.SkillName}！ しかしプレイヤーが防いだ！ 0ダメージ";
                Debug.Log(blockedMsg);
                AddEnemyActionLog(blockedMsg);
            }
            else
            {
                playerHp -= (int)damageToPlayer;
                string enemyMsg = $"敵: {enemySkill.SkillName}！ {(int)damageToPlayer}ダメージ \n{enemyEffect}";
                Debug.Log(enemyMsg);
                AddEnemyActionLog(enemyMsg);
            }

            UpdateHpUI();
            yield return StartCoroutine(WaitForClick());
            enemy.ShowIdlePose();
        }
    }

    private void PlayPlayerHealEffect()
    {
        if (playerHealEffectPrefab == null)
            return;
        AudioManager.Instance.PlaySE(playerSkill.skillSE);

        Vector3 spawnPos = playerSpriteRenderer.transform.position;
        spawnPos.y -= 3f;
        GameObject effect = Instantiate( playerHealEffectPrefab, spawnPos, Quaternion.identity);
        Destroy(effect, playerHealEffectLifetime);
    }

    private void PlayPlayerGuardEffect()
    {
        if (playerGuardEffectPrefab == null)
            return;
        if (playerSpriteRenderer == null)
            return;
        AudioManager.Instance.PlaySE(playerSkill.skillSE);

        Vector3 spawnPos = playerSpriteRenderer.transform.position;
        GameObject effect = Instantiate( playerGuardEffectPrefab, spawnPos, Quaternion.identity);
        Destroy(effect, playerGuardEffectLifetime);
    }

    private void UpdateHpUI()
    {
        playerHpText.text = "プレイヤーHP: " + playerHp;

        if (playerHpFillImage != null)
        {
            float playerRate = maxPlayerHp > 0 ? (float)playerHp / maxPlayerHp : 0f;
            playerHpFillImage.fillAmount = Mathf.Clamp01(playerRate);
        }

        if (enemy != null)
        {
            enemyHpText.text = "敵HP: " + enemy.NowEnemyHP;

            if (enemyHpFillImage != null)
            {
                enemyHpFillImage.fillAmount = Mathf.Clamp01(enemy.GetHPRate());
            }
        }
        else
        {
            enemyHpText.text = "敵HP: -";

            if (enemyHpFillImage != null)
            {
                enemyHpFillImage.fillAmount = 0f;
            }
        }
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
        Sprite attackSprite = distance == DistanceType.Ranged ? playerLongAttackSprite: playerCloseAttackSprite;
        if (attackSprite == null) return;
        if (playerIdleSprite != null && playerIdleSprite.pixelsPerUnit > 0)
        {
            float ratio = attackSprite.pixelsPerUnit / playerIdleSprite.pixelsPerUnit;
            playerSpriteRenderer.transform.localScale = playerIdleScale * ratio;
        }
        playerSpriteRenderer.sprite = attackSprite;
    }

    // 画像切り替えと同時にSEとエフェクトを再生する(エフェクトは敵の位置に出す)
    private void PlayPlayerAttackSEAndEffect()
    {
        if (playerSkill != null && playerSkill.skillSE != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySE(playerSkill.skillSE);
        }

        if (playerAttackEffectPrefab != null)
        {
            Vector3 spawnPos = GetEnemyEffectPosition();
            GameObject effect = Instantiate(playerAttackEffectPrefab, spawnPos, Quaternion.identity);
            Vector3 scale = effect.transform.localScale;
            scale.x *= -1f;
            effect.transform.localScale = scale;
            effect.transform.position += Vector3.left * 1f;
            if (playerSkill != null) 
            {
                AttributeColorUtility.ApplyAttributeColor(effect, playerSkill.attribute); 
            }
            Destroy(effect, playerAttackEffectLifetime);
        }
    }

    // エフェクトを出す位置(敵側)
    private Vector3 GetEnemyEffectPosition()
    {
        if (enemy != null && enemy.spriteRenderer != null)
        {
            return enemy.spriteRenderer.transform.position;
        }
        if (enemy != null)
        {
            return enemy.transform.position;
        }
        return transform.position;
    }

    // エフェクトを出す位置(プレイヤー側)
    private Vector3 GetPlayerEffectPosition()
    {
        return playerSpriteRenderer != null ? playerSpriteRenderer.transform.position : transform.position;
    }

    // 通常の画像に戻す
    private void ShowPlayerIdlePose()
    {
        if (playerSpriteRenderer == null || playerIdleSprite == null) return;

        playerSpriteRenderer.sprite = playerIdleSprite;
        playerSpriteRenderer.transform.localScale = playerIdleScale;
    }

    // 「技を使ったらその属性になる」という仕様を敵の防御側にも適用する。
    // プレイヤーの攻撃を受ける時も、敵がこのターンに選んだ技の属性で効果判定する
    // (敵の攻撃を受ける時のenemyAttackAttributeと同じ考え方)。
    // enemySkillがnull(PP切れで技を出せなかった)の時だけ、敵の主属性にフォールバックする。
    private List<AttributeType> GetEnemyAttributes(EnemySkillData enemySkill)
    {
        AttributeType attribute = enemySkill != null
            ? EnemyConverter.ToAttributeType(enemySkill.skillElement)
            : EnemyConverter.ToAttributeType(enemy.MainEnemyElement);

        return new List<AttributeType> { attribute };
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

            if (blackCover != null)
            {
                blackCover.ShowBlackCover();
            }
            else
            {
                // Black Coverが未設定でも、ゲームオーバー画面には必ず遷移する
                Debug.LogWarning("BlackCoverが設定されていません。InspectorのBattleManagerにBlack Coverを割り当てると、暗転演出付きでゲームオーバーに遷移します。");
                SceneManager.LoadScene("GameOver");
            }
            return;
        }

        if (IsEnemyDead())
        {
            if (bossQueue.Count == 0)
            {
                currentState = BattleState.Win;
                AddLog("全てのボスを倒した！");
                AddLog("クリア！！");
                StartCoroutine(PlaySlideAfterDelay(3f));
            }
            else
            {
                HealPlayer(0.3f);
                AddLog("ボスを倒した！\nHPが30%回復した");
                SpawnNextBoss();
            }
        }
    }

    private IEnumerator PlaySlideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        slideObject.Play();
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