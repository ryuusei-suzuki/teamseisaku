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
    private Action<SkillData> onSelect;

    // BattleManager(通常戦)とTrialBattleManager(チュートリアル)の両方から
    // 使い回せるように、呼び出し先を直接の型ではなくコールバックで受け取る。
    public void Setup(SkillData skill, Action<SkillData> onSelectCallback)
    {
        skillData = skill;
        onSelect = onSelectCallback;

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
        if (skillData != null && skillData.skillSE != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySE(skillData.skillSE);
        }
        onSelect?.Invoke(skillData); 
    }

    public SkillData GetSkillData()
    {
        return skillData;
    }

    // 自分のターンが始まるまで(演出を見終わるまで)はクリックできないようにするためのロック
    public void SetInteractable(bool interactable)
    {
        if (button != null)
        {
            button.interactable = interactable;
        }
    }
}