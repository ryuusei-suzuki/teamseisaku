using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class SkillDescriptionUI : MonoBehaviour
{
    [SerializeField] private GameObject descriptionPanel;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI remainingAPText;
    [SerializeField] private BattleManager battleManager;

    private void Start()
    {
        Hide();
    }

    // スキル詳細を表示する
    public void Show(SkillData skill)
    {
        if (skill == null) return;

        descriptionPanel.SetActive(true);

  
        descriptionText.text = skill.Description;

        int remainingAP = battleManager.GetRemainingAP(skill);
        remainingAPText.text = $"残りAP：{remainingAP}";
    }

    // スキル詳細を非表示にする
    public void Hide()
    {
        descriptionPanel.SetActive(false);
    }
}
