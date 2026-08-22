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

    public void Setup(SkillData skill, BattleManager manager)
    {
        skillData = skill;
        battleManager = manager;

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
        battleManager.SelectPlayerSkill(skillData);
    }
}