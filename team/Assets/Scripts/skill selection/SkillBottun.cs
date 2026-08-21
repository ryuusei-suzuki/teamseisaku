using UnityEngine;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    [SerializeField] private SkillData skillData;
    [SerializeField] private SkillSelectionUI selectionUI;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        if (skillData == null)
        {
            Debug.LogWarning("SkillData‚ªİ’è‚³‚ê‚Ä‚¢‚Ü‚¹‚ñB");
            return;
        }

        if (selectionUI == null)
        {
            Debug.LogWarning("SkillSelectionUI‚ªİ’è‚³‚ê‚Ä‚¢‚Ü‚¹‚ñB");
            return;
        }

        selectionUI.SelectSkill(skillData);
    }
}