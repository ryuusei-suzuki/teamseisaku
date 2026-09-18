using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemytester : MonoBehaviour
{
    [Header("敵の基本情報")]
    public EnemyData enemyData;
    public int MaxEnemyHP;
    public int NowEnemyHP;

    [Header("属性")]
    public EnemyElement MainEnemyElement;
    public EnemyElement SubEnemyElement;

    [Header("使用可能な技")]
    public List<EnemySkillData> allSkills = new();

    [Header("スキルのPP管理")]
    private Dictionary<EnemySkillData, int> skillPP = new();//PPを管理するための辞書

    [SerializeField] private List<string> debugPP = new();
    public SpriteRenderer spriteRenderer;

    private Sprite idleSprite;
    private Sprite attackSprite;

    // 2枚目のフレーム(未設定ならアニメーションせず1枚絵のまま)
    private Sprite idleSprite2;
    private Sprite attackSprite2;
    private Coroutine poseAnimCoroutine;
    private const float PoseFrameInterval = 0.45f; // 1コマの表示時間(秒)


    public void Init(EnemyData data, Element mainData, Element subData)
    {
        enemyData = data;

        MaxEnemyHP = enemyData.EnemyHP;
        NowEnemyHP = enemyData.EnemyHP;

        MainEnemyElement = mainData.enemyElement;

        // 見た目を主属性の画像に差し替える
        idleSprite = mainData.enemySprite;
        attackSprite = mainData.enemyAttackSprite;

        if (spriteRenderer != null && idleSprite != null)
        {
            spriteRenderer.sprite = idleSprite;
        }


        // 技リストを空にする
        allSkills.Clear();

        // 主属性の4技を追加
        AddElementSkills(mainData);

        // 副属性がある時だけ追加
        if (subData != null)
        {
            AddElementSkills(subData);
            SubEnemyElement = subData.enemyElement;
        }
        else
        {
            SubEnemyElement = EnemyElement.None;
        }

        // 属性に関係なく必ず追加
        AddSkill(enemyData.GuardSkill);
        AddSkill(enemyData.HealSkill);

        // PP初期化
        skillPP.Clear();

        foreach (var skill in allSkills)
        {
            if (skill != null)
            {
                skillPP[skill] = skill.PP;
            }
        }

        RefreshDebugPP();

        Debug.Log($"主属性:{MainEnemyElement} / 副属性:{SubEnemyElement}");
        Debug.Log($"総技数:{allSkills.Count}");

        foreach (var skill in allSkills)
        {
            Debug.Log($"使える技: {skill.SkillName}");
        }
    }

    void AddElementSkills(Element elementData)
    {
        AddSkill(elementData.CloseWeakAttack);
        AddSkill(elementData.CloseStrongAttack);
        AddSkill(elementData.LongWeakAttack);
        AddSkill(elementData.LongStrongAttack);
    }

    void AddSkill(EnemySkillData skill)
    {
        if (skill != null && !allSkills.Contains(skill))
        {
            allSkills.Add(skill);
        }
    }

    // チュートリアルなど、Elementの画像とは別の画像を使いたい場合に呼ぶ。
    // idle2/attack2は2コマアニメーションさせたい場合だけ指定する(不要ならnullでOK)
    public void SetPoseSprites(Sprite idle, Sprite attack, Sprite idle2 = null, Sprite attack2 = null)
    {
        if (idle != null) idleSprite = idle;
        if (attack != null) attackSprite = attack;
        idleSprite2 = idle2;
        attackSprite2 = attack2;

        if (spriteRenderer != null && idleSprite != null)
        {
            spriteRenderer.sprite = idleSprite;
        }
    }

    // 攻撃時に攻撃ポーズの画像に切り替える
    public void ShowAttackPose()
    {
        StopPoseAnimation();

        if (spriteRenderer != null && attackSprite != null)
        {
            spriteRenderer.sprite = attackSprite;
        }

        if (attackSprite2 != null)
        {
            poseAnimCoroutine = StartCoroutine(AnimatePose(attackSprite, attackSprite2, loop: false));
        }
    }

    // 通常の画像に戻す
    public void ShowIdlePose()
    {
        StopPoseAnimation();

        if (spriteRenderer != null && idleSprite != null)
        {
            spriteRenderer.sprite = idleSprite;
        }

        if (idleSprite2 != null)
        {
            poseAnimCoroutine = StartCoroutine(AnimatePose(idleSprite, idleSprite2, loop: true));
        }
    }

    private void StopPoseAnimation()
    {
        if (poseAnimCoroutine != null)
        {
            StopCoroutine(poseAnimCoroutine);
            poseAnimCoroutine = null;
        }
    }

    // frame1とframe2を一定間隔で交互に表示する簡易パラパラアニメ
    // loop=true : 待機用。ポーズを切り替えている間はずっと往復し続ける
    // loop=false: 攻撃用。ずっと往復すると不自然なので、1回だけ切り替えて止める
    private IEnumerator AnimatePose(Sprite frame1, Sprite frame2, bool loop)
    {
        bool showFirst = true;

        while (true)
        {
            yield return new WaitForSeconds(PoseFrameInterval);
            showFirst = !showFirst;

            if (spriteRenderer != null)
            {
                spriteRenderer.sprite = showFirst ? frame1 : frame2;
            }

            if (!loop)
            {
                yield break;
            }
        }
    }

    public float GetHPRate()
    {
        //ここでHPが何割か返す
        return (float)NowEnemyHP / MaxEnemyHP;
    }

    public void TakeDamage(int damage)
    {
        NowEnemyHP -= damage;

        if (NowEnemyHP < 0)
        {
            NowEnemyHP = 0;
        }

        Debug.Log($"{enemyData.EnemyName} に {damage} ダメージ！ 残りHP:{NowEnemyHP}/{MaxEnemyHP}");

        if (NowEnemyHP <= 0)
        {
            EnemyDie();
        }
    }

    void EnemyDie()
    {
        Debug.Log(enemyData.EnemyName + " を倒した！");
        Destroy(gameObject);
    }

    void AddWeightSkill(List<EnemySkillData> list, EnemySkillData skill, int weight)
    {
        if (!HasPP(skill))
            return;

        for (int i = 0; i < weight; i++)
        {
            list.Add(skill);
        }
    }

    public EnemySkillData ChooseSkill()
    {
        float hpRatio = GetHPRate();

        bool useMain = ChooseMainElement();


        // 副属性を持たない敵は、常にメイン属性を使う
        if (SubEnemyElement == EnemyElement.None)
        {
            useMain = true;
        }


        EnemyElement targetElement = useMain ? MainEnemyElement : SubEnemyElement;

        List<EnemySkillData> candidates = new();

        foreach (var skill in allSkills)
        {
            if (skill.skillElement != EnemyElement.None && skill.skillElement != targetElement)
            {
                continue;
            }

            int weight = 0;

            switch (skill.skillType)
            {
                case SkillType.CloseWeak:

                    if (hpRatio > 0.7f)
                        weight = enemyData.highHpCloseWeakWeight;

                    else if (hpRatio > 0.3f)
                        weight = enemyData.midHpCloseWeakWeight;

                    else
                        weight = enemyData.lowHpCloseWeakWeight;

                    break;

                case SkillType.CloseStrong:

                    if (hpRatio > 0.7f)
                        weight = enemyData.highHpCloseStrongWeight;

                    else if (hpRatio > 0.3f)
                        weight = enemyData.midHpCloseStrongWeight;

                    else
                        weight = enemyData.lowHpCloseStrongWeight;

                    break;

                case SkillType.LongWeak:

                    if (hpRatio > 0.7f)
                        weight = enemyData.highHpLongWeakWeight;

                    else if (hpRatio > 0.3f)
                        weight = enemyData.midHpLongWeakWeight;

                    else
                        weight = enemyData.lowHpLongWeakWeight;

                    break;

                case SkillType.LongStrong:

                    if (hpRatio > 0.7f)
                        weight = enemyData.highHpLongStrongWeight;

                    else if (hpRatio > 0.3f)
                        weight = enemyData.midHpLongStrongWeight;

                    else
                        weight = enemyData.lowHpLongStrongWeight;

                    break;
                case SkillType.Guard:

                    if (hpRatio > 0.7f)
                        weight = enemyData.highHpGuardWeight;

                    else if (hpRatio > 0.3f)
                        weight = enemyData.midHpGuardWeight;

                    else
                        weight = enemyData.lowHpGuardWeight;

                    break;

                case SkillType.Heal:

                    if (hpRatio > 0.7f)
                        weight = enemyData.highHpHealWeight;

                    else if (hpRatio > 0.3f)
                        weight = enemyData.midHpHealWeight;

                    else
                        weight = enemyData.lowHpHealWeight;

                    break;
            }

            AddWeightSkill(candidates, skill, weight);
        }

        if (candidates.Count == 0)
            return null;

        return candidates[Random.Range(0, candidates.Count)];
    }

    bool ConsumeSkillPP(EnemySkillData skill)
    {
        if (!skillPP.ContainsKey(skill))
            return false;

        if (skillPP[skill] <= 0)
        {
            Debug.Log(skill.SkillName + " はPP切れ！");
            return false;
        }

        skillPP[skill]--;

        Debug.Log($"{skill.SkillName} 残りPP:{skillPP[skill]}/{skill.PP}");
        RefreshDebugPP();
        return true;
    }


    bool HasPP(EnemySkillData skill)//PPが残っているか確認
    {
        return skill != null && skillPP.ContainsKey(skill) &&  skillPP[skill] > 0;
    }


    public void EnemyAttack()
    {
        EnemySkillData skill = ChooseSkill();

        if (skill == null)
        {
            Debug.Log(enemyData.EnemyName + " は使える技がない！");
            return;
        }

        if (!ConsumeSkillPP(skill))
            return;

        switch (skill.skillType)
        {
            case SkillType.CloseWeak:
            case SkillType.CloseStrong:
            case SkillType.LongWeak:
            case SkillType.LongStrong:

                Debug.Log($"{enemyData.EnemyName} の {skill.SkillName}！");

                // 後で攻撃処理を書く
                break;

            case SkillType.Guard:

                Debug.Log($"{enemyData.EnemyName} はガードした！");

                // 攻撃1回無効処理を後で書く
                break;

            case SkillType.Heal:

                Debug.Log($"{enemyData.EnemyName} は回復を使った！");
                
                NowEnemyHP += 40;

                //回復処理を後で書く
                break;
        }
    }

    void RefreshDebugPP()
    {
        debugPP.Clear();

        foreach (var pair in skillPP)
        {
            debugPP.Add($"{pair.Key.SkillName} : {pair.Value}/{pair.Key.PP}");
        }
    }

    bool ChooseMainElement()
    {
        int total =
            enemyData.mainElementWeight +
            enemyData.subElementWeight;

        int rand = Random.Range(0, total);

        return rand < enemyData.mainElementWeight;
    }

    public EnemySkillData UseSkillForBattle()
    {
        EnemySkillData skill = ChooseSkill();
        if (skill != null)
        {
            ConsumeSkillPP(skill);
        }
        return skill;
    }
}