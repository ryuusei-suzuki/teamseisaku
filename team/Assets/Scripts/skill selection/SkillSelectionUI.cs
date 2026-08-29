using UnityEngine;
using UnityEngine.UI;

public class SkillSelectionUI : MonoBehaviour
{
    [SerializeField] private Image[] skillIcons;

    // 何も選択されていないときに表示する黒いスロット画像
    [SerializeField] private Sprite emptySlotSprite;

    private void Start()
    {
        UpdateUI();
    }

    public void SelectSkill(SkillData skill)
    {
        bool success =
            SkillSelectionManager.Instance.SelectSkill(skill);

        if (success)
        {
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        if (SkillSelectionManager.Instance == null)
        {
            Debug.LogError("SkillSelectionManagerが存在しません。");
            return;
        }

        var selectedSkills =
            SkillSelectionManager.Instance.SelectedSkills;

        for (int i = 0; i < skillIcons.Length; i++)
        {
            if (i < selectedSkills.Count)
            {
                // 選択されたスキルのアイコンを表示
                skillIcons[i].sprite =
                    selectedSkills[i].iconImage;
            }
            else
            {
                // 未選択なら黒いスロットを表示
                skillIcons[i].sprite =
                    emptySlotSprite;
            }

            // 常に表示する
            skillIcons[i].enabled = true;
        }
    }

    public void OnBackButton()
    {
        SkillSelectionManager.Instance.ClearSkills();
        UpdateUI();
    }
}