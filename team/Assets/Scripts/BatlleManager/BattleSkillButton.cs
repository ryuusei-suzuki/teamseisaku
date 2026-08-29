using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BattleSkillButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    [SerializeField] private Image iconImage;
    private Button button;
    private SkillData skillData;
    private BattleManager battleManager;
    private Action<SkillData> onSelectCallback; // BattleManager以外(TrialBattleManagerなど)からも使えるようにするコールバック

    public void Setup(SkillData skill, BattleManager manager)
    {
        battleManager = manager;
        Setup(skill, manager.SelectPlayerSkill);
    }

    // BattleManagerに依存しない汎用セットアップ(TrialBattleManagerなどから利用)
    public void Setup(SkillData skill, Action<SkillData> onSelect)
    {
        skillData = skill;
        onSelectCallback = onSelect;

        if (label != null)
        {
            label.text = skill.SkillName;
        }

        if (iconImage != null && skill.iconImage != null)
        {
            iconImage.sprite = skill.iconImage;
        }

        button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        onSelectCallback?.Invoke(skillData);
    }

    public SkillData GetSkillData()
    {
        return skillData;
    }
}
