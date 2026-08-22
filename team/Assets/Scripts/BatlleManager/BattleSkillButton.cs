using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class BattleSkillButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI label;
    private Button button;
    private SkillData skillData;
    private BattleManager battleManager;

    public void Setup(SkillData skill, BattleManager manager)
    {
        skillData = skill;
        battleManager = manager;
        label.text = skill.SkillName;

        button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        battleManager.SelectPlayerSkill(skillData);
    }
}
