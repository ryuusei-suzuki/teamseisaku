using System.Collections.Generic;
using UnityEngine;

public class SkillSelectionManager : MonoBehaviour
{
    public static SkillSelectionManager Instance { get; private set; }

    public const int MaxSkillCount = 5;

    private List<SkillData> selectedSkills = new List<SkillData>();

    public IReadOnlyList<SkillData> SelectedSkills => selectedSkills;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        // シーンを移動しても消さない
        DontDestroyOnLoad(gameObject);
    }

    public bool SelectSkill(SkillData skill)
    {
        if (skill == null)
        {
            return false;
        }

        // 5個以上選択できない
        if (selectedSkills.Count >= MaxSkillCount)
        {
            Debug.Log("スキルは最大5個まで選択できます。");
            return false;
        }

        // 同じスキルを2回選択できない
        if (selectedSkills.Contains(skill))
        {
            Debug.Log("このスキルはすでに選択されています。");
            return false;
        }

        selectedSkills.Add(skill);

        Debug.Log(
            "スキル選択: " +
            skill.SkillName +
            " (" +
            selectedSkills.Count +
            "/5)"
        );

        return true;
    }

    public void ClearSkills()
    {
        selectedSkills.Clear();
    }

    public bool IsFull()
    {
        return selectedSkills.Count >= MaxSkillCount;
    }
}
