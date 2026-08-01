using System.Collections.Generic;
using UnityEngine;

public class Enemytester : MonoBehaviour
{
    [Header("敵の基本情報")]
    public EnemyData enemyData;
    public int MaxEnemyHP;
    public int NowEnemyHP;

    [Header("属性")]
    public EnemyElement enemyElement;

    [Header("スキル")]
    public EnemySkillData closeWeak;
    public EnemySkillData closeStrong;
    public EnemySkillData longWeak;
    public EnemySkillData longStrong;

    [Header("スキルのPP管理")]
    private Dictionary<EnemySkillData, int> skillPP = new();//PPを管理するための辞書

    public void Init(EnemyData data, Element elementData)
    {
        enemyData = data;
        MaxEnemyHP = enemyData.EnemyHP;
        NowEnemyHP = enemyData.EnemyHP;

        enemyElement = elementData.enemyElement;

        closeWeak = elementData.CloseWaekAttack;
        closeStrong = elementData.CloseStrongAttack;
        longWeak = elementData.LongWaekAttack;
        longStrong = elementData.LongStrongAttack;

        //ここでPPを初期化する
        skillPP.Clear();

        skillPP[closeWeak] = closeWeak.PP;
        skillPP[closeStrong] = closeStrong.PP;
        skillPP[longWeak] = longWeak.PP;
        skillPP[longStrong] = longStrong.PP;
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

    public EnemySkillData ChooseSkill()
    {
        float hpRatio = GetHPRate();

        List<EnemySkillData> candidates = new();//使用可能なスキルを格納するリスト

        // HP70%以上    弱攻撃多め
        if (hpRatio > 0.7f)
        {
            if (HasPP(closeWeak)) candidates.Add(closeWeak);
            if (HasPP(closeWeak)) candidates.Add(closeWeak);
            if (HasPP(longWeak)) candidates.Add(longWeak);
        }

        // HP30%以上    バランス
        else if (hpRatio > 0.3f)
        {
            if (HasPP(closeWeak)) candidates.Add(closeWeak);
            if (HasPP(closeStrong)) candidates.Add(closeStrong);
            if (HasPP(longWeak)) candidates.Add(longWeak);
            if (HasPP(longStrong)) candidates.Add(longStrong);
        }

        else
        {
            if (HasPP(closeStrong)) candidates.Add(closeStrong);
            if (HasPP(closeStrong)) candidates.Add(closeStrong);
            if (HasPP(longStrong)) candidates.Add(longStrong);

            // 強攻撃が全部PP切れなら弱攻撃も候補にする
            if (candidates.Count == 0)
            {
                if (HasPP(closeWeak)) candidates.Add(closeWeak);
                if (HasPP(longWeak)) candidates.Add(longWeak);
            }
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

        Debug.Log($"{enemyData.EnemyName} の {skill.SkillName}！");

        if (!ConsumeSkillPP(skill))
            return;
        Debug.Log($"{enemyData.EnemyName} の {skill.SkillName}！");

        // ここでプレイヤーにダメージ減るように
        // player.TakeDamage(skill.Damage);
    }
}